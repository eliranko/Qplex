using NLog;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Networking.Protocols;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Client net service
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public class ClientNetService<TIterator> : NetService<TIterator>
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Network protocl
        /// </summary>
        private readonly IProtocol _protocol;

        /// <summary>
        /// Ctor
        /// </summary>
        public ClientNetService(IProtocol protocol)
        {
            _protocol = protocol;
            _protocol.SubscribeToChannel(ServiceToProtocolChannel);
        }

        /// <summary>
        /// Connect client
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            if (_protocol.Start() && base.Start())
            {
                Log(LogLevel.Trace, $"ClientNetService: {Name} started successfully");
                return true;
            }

            Log(LogLevel.Fatal, "ClientNetService failed to start");
            return false;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public override void Stop()
        {
            _protocol.Stop();
            base.Stop();
            Log(LogLevel.Trace, $"ClientNetService: {Name} stopped successfully");
        }

        /// <summary>
        /// Send message
        /// </summary>
        public override void Send(Message message)
        {
            Log(LogLevel.Debug, $"Sending message:{message.Name}");
            _protocol.Send(message);
        }
    }

    /// <summary>
    /// Client net service implements using queueu messages iterator
    /// </summary>
    public class ClientNetService : ClientNetService<QueueMessagesIterator>
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="protocol">Protocol</param>
        public ClientNetService(IProtocol protocol) : base(protocol)
        {
        }
    }
}
