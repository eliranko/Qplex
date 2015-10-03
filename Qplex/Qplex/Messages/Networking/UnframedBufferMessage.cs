namespace Qplex.Messages.Networking
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
        /// <param name="message">Message</param>
        public UnframedBufferMessage(Message message)
        {
            Message = message;
        }
    }
}
