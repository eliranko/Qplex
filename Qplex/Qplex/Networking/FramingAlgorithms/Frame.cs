using System.Linq;

namespace Qplex.Networking.FramingAlgorithms
{
    /// <summary>
    /// Frame contians buffer with metadata
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Buffer
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="buffer">Buffer</param>
        public Frame(byte[] buffer)
        {
            Buffer = buffer;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var frame = obj as Frame;
            return frame != null && Buffer.SequenceEqual(frame.Buffer);
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

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Frame with: {string.Join(",", Buffer)}";
        }
    }
}
