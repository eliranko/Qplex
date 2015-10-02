using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace Qplex.Networking
{
    public class Protocol : Communicator
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
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        public void Send(Message message)
        {
            _agent.Send(message);
        }
    }
}
