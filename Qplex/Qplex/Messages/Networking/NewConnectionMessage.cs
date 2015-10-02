using Qplex.Networking;

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
        public Agent Agent { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="agent">Agent</param>
        public NewConnectionMessage(Agent agent)
        {
            Agent = agent;
        }
    }
}
