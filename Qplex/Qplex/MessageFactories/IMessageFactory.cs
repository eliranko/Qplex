using Qplex.Messages;

namespace Qplex.MessageFactories
{
    /// <summary>
    /// Message factory serializes and deserializes Message objects
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Serialize message to bytes
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Byte array</returns>
        byte[] Serialize(Message message);

        /// <summary>
        /// Deserialize byte to message
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Message</returns>
        Message Deserialize(byte[] bytes);
    }
}