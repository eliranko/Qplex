namespace Qplex.Messages.Networking.Parser
{
    /// <summary>
    /// Unframed buffer message
    /// </summary>
    public class UnframedBufferMessage : Message
    {
        /// <summary>
        /// Message
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public UnframedBufferMessage(Message message)
        {
            Message = message;
        }
    }
}
