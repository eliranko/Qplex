namespace Qplex.Messages.Networking.Parser
{
    /// <summary>
    /// Unframed buffer message
    /// </summary>
    public class ParserUnframedBufferMessage : Message
    {
        /// <summary>
        /// Message
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public ParserUnframedBufferMessage(Message message)
        {
            Message = message;
        }
    }
}
