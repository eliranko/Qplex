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

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var newMessage = obj as NewIncomingMessage;
            return newMessage != null && Message.Equals(newMessage.Message);
        }

        /// <summary>
        /// Hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
            return base.GetHashCode();
        }
    }
}
