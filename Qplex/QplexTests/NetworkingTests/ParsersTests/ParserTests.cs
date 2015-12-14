using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Channels;
using Qplex.Messages;
using Qplex.Messages.Networking.Connection;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Connection;
using Qplex.Networking.FramingAlgorithms;
using Qplex.Networking.MessageFactories;
using Qplex.Networking.Parsers;

namespace QplexTests.NetworkingTests.ParsersTests
{
    [TestClass]
    public class ParserTests
    {
        private IParser _parser;
        private Mock<IConnection> _connectionMock;
        private Mock<IMessageFactory> _factoryMock;
        private Mock<IFramingAlgorithm> _framingMock;
        private Mock<IInternalChannel> _channelMock;

        [TestInitialize]
        public void TestInit()
        {
            _connectionMock = new Mock<IConnection>();
            _factoryMock = new Mock<IMessageFactory>();
            _framingMock = new Mock<IFramingAlgorithm>();
            _channelMock = new Mock<IInternalChannel>();

            _parser = new Parser(_connectionMock.Object, _factoryMock.Object, _framingMock.Object);
            _parser.SubscribeToChannel(_channelMock.Object);
        }

        #region Start

        [TestMethod]
        public void StartFailsWhenConnectionFails()
        {
            _connectionMock.Setup(connection => connection.ConnectAndReceive())
                .Returns(ConnectionConnectStatus.SocketError)
                .Verifiable();
            Assert.IsFalse(_parser.Start());

            _connectionMock.Verify();
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopClosesConnection()
        {
            _connectionMock.Setup(connection => connection.Close()).Verifiable();
            _parser.Stop();

            _connectionMock.Verify();
        }

        #endregion

        #region Send

        [TestMethod]
        public void SendCallsRelevantFunctions()
        {
            _connectionMock.Setup(connection => connection.Send(It.IsAny<byte[]>())).Verifiable();
            _framingMock.Setup(algorithm => algorithm.FrameBuffer(It.IsAny<Frame>())).Returns(new byte[0]).Verifiable();
            _factoryMock.Setup(factory => factory.Serialize(It.IsAny<Message>()))
                .Returns(new Frame(new byte[0]))
                .Verifiable();
            _parser.Send(new MockMessage());

            _connectionMock.Verify();
            _framingMock.Verify();
            _factoryMock.Verify();
        }

        #endregion

        #region RetrieveConnection

        [TestMethod]
        public void RetrieveConnectionCallsConnectAndReceive()
        {
            _connectionMock.Setup(connection => connection.ConnectAndReceive()).Verifiable();
            _parser.RetrieveConnection();

            _connectionMock.Verify();
        }

        #endregion

        #region HandleConnectionBufferReceivedMessage

        [TestMethod]
        public void HandleConnectionBufferReceivedMessageBroadcastsNewIncomingMessageOnSuccess()
        {
            _framingMock.Setup(algorithm => algorithm.UnframeBuffer(It.IsAny<byte[]>()))
                .Returns(new Frame(new byte[0]))
                .Verifiable();
            _factoryMock.Setup(factory => factory.Deserialize(It.IsAny<Frame>()))
                .Returns(new MockMessage())
                .Verifiable();
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<NewIncomingMessage>(), It.IsAny<Guid>()))
                .Verifiable();
            _parser.HandleConnectionBufferReceivedMessage(
                new ConnectionBufferReceivedMessage(ConnectionSocketStatus.Success, new byte[0], null, null));

            _framingMock.Verify();
            _factoryMock.Verify();
            _channelMock.Verify();
        }

        #endregion

        #region HandleConnectionSendStatusMessage

        [TestMethod]
        public void HandleConnectionSendStatusMessageBroadcastErrorOnClientDisposed()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ParserConnectionErrorMessage>(), It.IsAny<Guid>()))
                .Verifiable();
            _parser.HandleConnectionSendStatusMessage(
                new ConnectionSendStatusMessage(ConnectionSocketStatus.ClientDisposed));

            _channelMock.Verify();
        }

        [TestMethod]
        public void HandleConnectionSendStatusMessageBroadcastErrorOnError()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ParserConnectionErrorMessage>(), It.IsAny<Guid>()))
                .Verifiable();
            _parser.HandleConnectionSendStatusMessage(
                new ConnectionSendStatusMessage(ConnectionSocketStatus.Error));

            _channelMock.Verify();
        }

        [TestMethod]
        public void HandleConnectionSendStatusMessageBroadcastErrorOnSocketClosed()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ParserConnectionErrorMessage>(), It.IsAny<Guid>()))
                .Verifiable();
            _parser.HandleConnectionSendStatusMessage(
                new ConnectionSendStatusMessage(ConnectionSocketStatus.SocketClosed));

            _channelMock.Verify();
        }

        #endregion
    }
}
