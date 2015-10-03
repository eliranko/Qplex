namespace Qplex.Messages.Networking
{
    /// <summary>
    /// New buffer received over socket message
    /// </summary>
    public class BufferReceivedMessage : Message
    {
        /// <summary>
        /// Buffer
        /// </summary>
        public byte[] Buffer { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="buffer">Buffer</param>
        public BufferReceivedMessage(byte[] buffer)
        {
            Buffer = buffer;
        }
    }
}
