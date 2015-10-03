using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Qplex.FramingAlgorithms;
using Qplex.Messages;

namespace Qplex.MessageFactories
{
    /// <summary>
    /// Message factory which serializes and deserialize Message object as is
    /// </summary>
    public class MessageObjectMessageFactory : IMessageFactory
    {
        /// <summary>
        /// Serialize message to frame
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Frame</returns>
        public Frame Serialize(Message message)
        {
            var memoryStream = new MemoryStream();
            new BinaryFormatter().Serialize(memoryStream, message);
            return new Frame(memoryStream.ToArray());
        }

        /// <summary>
        /// Deserialize frame to message
        /// </summary>
        /// <param name="frame">Frame</param>
        /// <returns>Message</returns>
        public Message Deserialize(Frame frame)
        {
            var memoryStream = new MemoryStream(frame.Buffer);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return (Message) new BinaryFormatter().Deserialize(memoryStream);
        }
    }
}
