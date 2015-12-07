using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qplex.Networking.FramingAlgorithms;

namespace QplexTests.NetworkingTests.FramingAlgorithmsTests
{
    [TestClass]
    public class FramingAlgorithmInt32Tests
    {
        private IFramingAlgorithm _framingAlgorithm;
        private Frame _frame;

        [TestInitialize]
        public void TestInit()
        {
            _framingAlgorithm = new FramingAlgorithmInt32();
            _frame = new Frame(new[] {(byte) 1, (byte) 2});
        }

        #region FrameBuffer

        [TestMethod]
        public void FrameBufferReturnsEmptyBufferWhenReceivingNull()
        {
            Assert.IsFalse(_framingAlgorithm.FrameBuffer(null).Any());
        }

        [TestMethod]
        public void FrameBufferReturnsCorrectBuffer()
        {
            var buffer = _framingAlgorithm.FrameBuffer(_frame);
            Assert.IsTrue(new byte[] {2, 0, 0, 0, 1, 2}.SequenceEqual(buffer));
        }

        #endregion

        #region UnframeBuffer

        [TestMethod]
        public void UnframeBufferReturnsEmptyFrameWhenReceivingNull()
        {
            Assert.IsFalse(_framingAlgorithm.UnframeBuffer(null).Buffer.Any());
        }

        [TestMethod]
        public void UnframeBufferReturnsCorrectFrame()
        {
            var frame = _framingAlgorithm.UnframeBuffer(new byte[] {2, 0, 0, 0, 1, 2});
            Assert.AreEqual(_frame, frame);
        }

        #endregion
    }
}
