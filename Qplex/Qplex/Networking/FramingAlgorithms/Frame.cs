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
    }
}
