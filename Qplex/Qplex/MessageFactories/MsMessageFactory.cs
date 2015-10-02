using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Qplex.Messages;

namespace Qplex.MessageFactories
{
    /// <summary>
    /// Memory stream message factory
    /// </summary>
    public class MsMessageFactory : IMessageFactory
    {
        /// <summary>
        /// Serialize message to bytes
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Byte array</returns>
        public byte[] Serialize(Message message)
        {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, message);
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Deserialize byte to message
        /// </summary>
        /// <param name="bytes">Bytes</param>
        /// <returns>Message</returns>
        public Message Deserialize(byte[] bytes)
        {
            var memoryStream = new MemoryStream(bytes);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (Message) new BinaryFormatter().Deserialize(memoryStream);
        }
    }
}
