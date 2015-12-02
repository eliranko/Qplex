using Qplex.Messages;
using Qplex.Networking.FramingAlgorithms;

namespace Qplex.Networking.MessageFactories
{
    /// <summary>
    /// Message factory serializes and deserializes Message objects
    /// </summary>
    public interface IMessageFactory
    {
        /// <summary>
        /// Serialize message to frame
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Frame</returns>
        Frame Serialize(Message message);

        /// <summary>
        /// Deserialize frame to message
        /// </summary>
        /// <param name="frame">Frame</param>
        /// <returns>Message</returns>
        Message Deserialize(Frame frame);
    }
}