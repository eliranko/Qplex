using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Channels;
using Qplex.Layers;
using Qplex.Networking.NetService;

namespace QplexTests.LayersTests
{
    class NetworkLayerTest : NetworkLayer
    {
        public Mock<INetService> NetServiceMock { get; }

        public NetworkLayerTest()
        {
            NetServiceMock = new Mock<INetService>();
        }

        public override bool Init()
        {
            AddService(NetServiceMock.Object);

            return true;
        }
    }

    [TestClass]
    public class NetworkLayerTests
    {
        private NetworkLayerTest _layer;

        [TestInitialize]
        public void TestInit()
        {
            _layer = new NetworkLayerTest();
        }

        #region AddService

        [TestMethod]
        public void AddServiceSubscribesServiceToChannel()
        {
            _layer.NetServiceMock.Setup(service => service.SubscribeToChannel(It.IsAny<IInternalChannel>()))
                .Verifiable();
            _layer.Init();

            _layer.NetServiceMock.Verify();
        }

        #endregion

        #region Start

        [TestMethod]
        public void StartReturnsFalseWhenNetSerivceReturnsFalse()
        {
            _layer.NetServiceMock.Setup(service => service.Start()).Returns(false).Verifiable();
            _layer.Init();
            Assert.IsFalse(_layer.Start());

            _layer.NetServiceMock.Verify();
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopCallsServicesStop()
        {
            _layer.NetServiceMock.Setup(service => service.Stop()).Verifiable();
            _layer.Init();
            _layer.Stop();

            _layer.NetServiceMock.Verify();
        }

        #endregion
    }
}
