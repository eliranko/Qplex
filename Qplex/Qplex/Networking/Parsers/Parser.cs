﻿using NLog;
using Qplex.Attributes;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.FramingAlgorithms;
using Qplex.MessageFactories;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Connection;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Connection;

namespace Qplex.Networking.Parsers
{
    /// <summary>
    /// Parser sends and receives Message objects.
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Parser<TIterator> : Communicator<TIterator>, IParser
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Connectoin
        /// </summary>
        private readonly IConnection _connection;

        /// <summary>
        /// Message factory
        /// </summary>
        private readonly IMessageFactory _messageFactory;

        /// <summary>
        /// Framing algorithm
        /// </summary>
        private readonly IFramingAlgorithm _framingAlgorithm;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="messageFactory">Message factory</param>
        /// <param name="framingAlgorithm">Framing algorithm</param>
        public Parser(IConnection connection, IMessageFactory messageFactory, IFramingAlgorithm framingAlgorithm)
        {
            _connection = connection;
            _connection.HeaderSize = framingAlgorithm.HeaderSize;
            _messageFactory = messageFactory;
            _framingAlgorithm = framingAlgorithm;
            var channel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToConnectionChannel");
            SubscribeToChannel(channel);
            _connection.SubscribeToChannel(channel);
        }

        /// <summary>
        /// Start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            if (_connection.ConnectAndReceive() == ConnectionConnectStatus.Success && base.Start())
            {
                Log(LogLevel.Trace, "Parser started successfully");
                return true;
            }

            Log(LogLevel.Fatal, "Parser failed to start");
            return false;
        }

        /// <summary>
        /// Stopping connection
        /// </summary>
        public override void Stop()
        {
            _connection.Close();
            base.Stop();
            Log(LogLevel.Trace, "Parser stopped successfully");
        }

        /// <summary>
        /// Send serialized message
        /// </summary>
        public void Send(Message message)
        {
            Log(LogLevel.Trace, "Sending message through connection");
            _connection.Send(_framingAlgorithm.FrameBuffer(_messageFactory.Serialize(message)));
        }

        /// <summary>
        /// Retrieve connection
        /// </summary>
        public void RetrieveConnection()
        {
            Log(LogLevel.Trace, "Retrieving connection...");
            _connection.ConnectAndReceive();
        }

        /// <summary>
        /// Handle received buffer from connection
        /// </summary>
        [MessageHandler]
        public void HandleConnectionBufferReceivedMessage(ConnectionBufferReceivedMessage message)
        {
            Log(LogLevel.Trace, $"Handling new buffer of size:{message.Buffer.Length}");
            Broadcast(new ParserUnframedBufferMessage(
                _messageFactory.Deserialize(_framingAlgorithm.UnframeBuffer(message.Buffer))));
        }

        /// <summary>
        /// Handle send status received from the connection
        /// </summary>
        [MessageHandler]
        public void HandleConnectionSendStatusMessage(ConnectionSendStatusMessage message)
        {
            Log(LogLevel.Trace, "Handling ConnectionSendStatusMessage message");

            if (message.ConnectionSocketStatus == ConnectionSocketStatus.Success) return;
            Log(LogLevel.Error, "Connection socket has failed, notifying agent...");
            Broadcast(new ParserConnectionErrorMessage());
        }
    }

    /// <summary>
    /// Parser implemented using queue message iterator
    /// </summary>
    public class Parser : Parser<QueueMessagesIterator>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <param name="messageFactory">Message factory</param>
        /// <param name="framingAlgorithm">Framing algorithm</param>
        public Parser(IConnection connection, IMessageFactory messageFactory, IFramingAlgorithm framingAlgorithm) 
            : base(connection, messageFactory, framingAlgorithm)
        {
        }
    }
}
