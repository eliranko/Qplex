namespace Qplex.FramingAlgorithms
{
    /// <summary>
    /// Framing algorithm frames buffers with a specific header
    /// </summary>
    public interface IFramingAlgorithm
    {
        /// <summary>
        /// Frame buffer
        /// </summary>
        /// <param name="frame">Frame</param>
        /// <returns>Framed buffer</returns>
        byte[] FrameBuffer(Frame frame);

        /// <summary>
        /// Unframe buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Frame</returns>
        Frame UnframeBuffer(byte[] buffer);
    }
}
