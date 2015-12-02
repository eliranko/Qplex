namespace Qplex.Messages.Networking.Parser
{
    /// <summary>
    /// New message received from IConnection
    /// </summary>
    public class NewIncomingMessage : Message
    {
        /// <summary>
        /// Message
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public NewIncomingMessage(Message message)
        {
            Message = message;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"NewIncomingMessage with message: {Message}";
        }
    }
}
