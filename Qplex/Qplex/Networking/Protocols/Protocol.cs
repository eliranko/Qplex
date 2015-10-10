using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Networking.Agents;

namespace Qplex.Networking.Protocols
{
    /// <summary>
    /// Protocol wraps agent and handles low level network message, that the layer isn't
    /// interested about (such as keep-alive).
    /// </summary>
    /// <typeparam name="TIterator">Messages iterator</typeparam>
    public class Protocol<TIterator> : Communicator<TIterator>, IProtocol where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Network agent
        /// </summary>
        private IAgent _agent;

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
        /// Ctor
        /// </summary>
        /// <param name="agent">Agent</param>
        public Protocol(IAgent agent)
        {
            _agent = agent;
            _protocolToAgentChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToAgentChannel");
            SubscribeToChannel(_protocolToAgentChannel);
            _agent.SubscribeToChannel(_protocolToAgentChannel);
        }

        /// <summary>
        /// Set agent
        /// </summary>
        /// <param name="agent">Network agent</param>
        public void SetAgent(IAgent agent)
        {
            _agent?.UnsubscribeFromChannel(_protocolToAgentChannel);
            _agent = agent;
            _agent.SubscribeToChannel(_protocolToAgentChannel);
        }

        /// <summary>
        /// Start receiving messages
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            return _agent.Start() && base.Start();
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
        /// <summary>
        /// Ctor
        /// </summary>
        public Protocol()
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="agent">Agent</param>
        public Protocol(IAgent agent)
            : base(agent)
        {
        }
    }
}
