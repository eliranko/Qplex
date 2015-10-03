using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Networking
{
    /// <summary>
    /// Protocol wraps agent and handles low level network message, that the layer isn't
    /// interested about (such as keep-alive).
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Protocol<TIterator> : Communicator<TIterator> where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Network agent
        /// </summary>
        private Agent _agent;

        /// <summary>
        /// Protocol to agent channel
        /// </summary>
        private readonly InternalChannel _protocolToAgentChannel;

        /// <summary>
        /// Ctor
        /// </summary>
        public Protocol()
        {
            _protocolToAgentChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToAgentChannel");
            SubscribeToChannel(_protocolToAgentChannel);
        }

        /// <summary>
        /// Set agent
        /// </summary>
        /// <param name="agent">Network agent</param>
        public void SetAgent(Agent agent)
        {
            _agent?.UnsubscribeFromChannel(_protocolToAgentChannel);
            _agent = agent;
            _agent.SubscribeToChannel(_protocolToAgentChannel);
        }

        /// <summary>
        /// Connect
        /// </summary>
        public void Connect()
        {
            _agent.Connect();
        }

        /// <summary>
        /// Close conneciton
        /// </summary>
        public void Close()
        {
            _agent.Close();
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            _agent.Send(message);
        }
    }

    /// <summary>
    /// Protocol implemented with queue messgea iterator
    /// </summary>
    public class Protocol : Protocol<QueueMessagesIterator>
    {
        
    }
}
