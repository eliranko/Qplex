using Qplex.Messages.Networking.Parser;

namespace Qplex.Messages.Networking.Agent
{
    /// <summary>
    /// New incoming message
    /// </summary>
    public class NewIncomingMessage : ParserUnframedBufferMessage
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NewIncomingMessage(Message message) : base(message)
        {
        }
    }
}
