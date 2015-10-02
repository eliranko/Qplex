namespace Qplex.Messages.Networking
{
    /// <summary>
    /// A new message was received
    /// </summary>
    public class SerializedMessageReceivedMessage : Message
    {
        /// <summary>
        /// Message array
        /// </summary>
        public byte[] MessageArray { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messageArray">Message array</param>
        public SerializedMessageReceivedMessage(byte[] messageArray)
        {
            MessageArray = messageArray;
        }
    }
}
