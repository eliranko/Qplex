namespace Qplex.Messages.Networking
{
    /// <summary>
    /// Deserialized message
    /// </summary>
    public class DeserializedMessage : Message
    {
        /// <summary>
        /// Message
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message">Message</param>
        public DeserializedMessage(Message message)
        {
            Message = message;
        }
    }
}
