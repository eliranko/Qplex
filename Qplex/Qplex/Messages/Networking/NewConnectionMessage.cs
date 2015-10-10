using Qplex.Networking.Agents;

namespace Qplex.Messages.Networking
{
    /// <summary>
    /// New connection message
    /// </summary>
    public class NewConnectionMessage : Message
    {
        /// <summary>
        /// Network agent
        /// </summary>
        public IAgent Agent { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="agent">Agent</param>
        public NewConnectionMessage(IAgent agent)
        {
            Agent = agent;
        }
    }
}
