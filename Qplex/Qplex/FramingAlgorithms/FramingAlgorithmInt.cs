using System;
using System.Linq;

namespace Qplex.FramingAlgorithms
{
    /// <summary>
    /// Integer framing algorithm
    /// </summary>
    public abstract class FramingAlgorithmInt : IFramingAlgorithm
    {
        /// <summary>
        /// Header size
        /// </summary>
        public abstract int HeaderSize { get; }

        /// <summary>
        /// Frame buffer
        /// </summary>
        /// <param name="frame">Frame</param>
        /// <returns>Framed buffer</returns>
        public byte[] FrameBuffer(Frame frame)
        {
            return WrapBufferWithHeader(frame.Buffer);
        }

        /// <summary>
        /// Unframe buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Frame</returns>
        public Frame UnframeBuffer(byte[] buffer)
        {
            return new Frame(UnwrapBufferFromHeader(buffer));
        }

        /// <summary>
        /// Unwrap buffer
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Unwrapped buffer</returns>
        private byte[] UnwrapBufferFromHeader(byte[] buffer)
        {
            var unwrappedBufferSize = buffer.Length - HeaderSize;
            var unwrappedBuffer = new byte[unwrappedBufferSize];
            Array.Copy(buffer, HeaderSize, unwrappedBuffer, 0, unwrappedBufferSize);

            return unwrappedBuffer;
        }

        /// <summary>
        /// Wrap buffer with header
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Wrapped buffer</returns>
        protected abstract byte[] WrapBufferWithHeader(byte[] buffer);
    }

    /// <summary>
    /// Int32 framing algorithm
    /// </summary>
    public class FramingAlgorithmInt32 : FramingAlgorithmInt
    {
        /// <summary>
        /// Header size
        /// </summary>
        public override int HeaderSize { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public FramingAlgorithmInt32()
        {
            HeaderSize = 4;
        }

        /// <summary>
        /// Wrap buffer with header
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Wrapped buffer</returns>
        protected override byte[] WrapBufferWithHeader(byte[] buffer)
        {
            var wrappedBuffer = new byte[HeaderSize + buffer.Length];
            var header = BitConverter.GetBytes(buffer.Length);

            //Copy arrays
            header.CopyTo(wrappedBuffer, 0);
            buffer.CopyTo(wrappedBuffer, HeaderSize);

            return wrappedBuffer;
        }
    }

    /// <summary>
    /// Int16 framing algorithm
    /// </summary>
    public class FramingAlgorithmInt16 : FramingAlgorithmInt
    {
        /// <summary>
        /// Header size
        /// </summary>
        public override int HeaderSize { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public FramingAlgorithmInt16()
        {
            HeaderSize = 2;
        }

        /// <summary>
        /// Wrap buffer with header
        /// </summary>
        /// <param name="buffer">Buffer</param>
        /// <returns>Wrapped buffer</returns>
        protected override byte[] WrapBufferWithHeader(byte[] buffer)
        {
            var wrappedBuffer = new byte[HeaderSize + buffer.Length];
            var header = BitConverter.GetBytes((short)buffer.Length);

            //Copy arrays
            header.CopyTo(wrappedBuffer, 0);
            buffer.CopyTo(wrappedBuffer, HeaderSize);

            return wrappedBuffer;
        }
    }
}
