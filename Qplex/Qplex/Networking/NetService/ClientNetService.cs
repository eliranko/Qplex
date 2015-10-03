using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Networking.NetService
{
    /// <summary>
    /// Client net service
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public class ClientNetService<TIterator> : NetService<TIterator> where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Network protocl
        /// </summary>
        private readonly Protocol _protocol;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="protocol">Protocol</param>
        protected ClientNetService(Protocol protocol)
        {
            _protocol = protocol;
        }

        /// <summary>
        /// Connect client
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            var status = base.Start();
            _protocol.Connect();

            return status;
        }

        /// <summary>
        /// Close connection
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            _protocol.Close();
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public override void Send(Message message)
        {
            _protocol.Send(message);
        }
    }
}
