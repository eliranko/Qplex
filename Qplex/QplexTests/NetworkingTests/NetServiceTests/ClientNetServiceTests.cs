using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Channels;
using Qplex.Messages;
using Qplex.Networking.NetService;
using Qplex.Networking.Protocols;

namespace QplexTests.NetworkingTests.NetServiceTests
{
    [TestClass]
    public class ClientNetServiceTests
    {
        private ClientNetService _clientNetService;
        private Mock<IProtocol> _protocolMock;
        private Mock<IInternalChannel> _channelMock;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _protocolMock = new Mock<IProtocol>();
            _clientNetService = new ClientNetService(_protocolMock.Object);
            _channelMock = new Mock<IInternalChannel>();
            _message = new MockMessage();
            _clientNetService.SubscribeToChannel(_channelMock.Object);
        }

        #region Start

        [TestMethod]
        public void StartReturnsFalseWhenProtocolFails()
        {
            _protocolMock.Setup(protocol => protocol.Start()).Returns(false).Verifiable();

            Assert.IsFalse(_clientNetService.Start());
            _protocolMock.Verify();
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopCallsProtocolStop()
        {
            _protocolMock.Setup(protocol => protocol.Stop()).Verifiable();
            _clientNetService.Stop();

            _protocolMock.Verify();
        }

        #endregion

        #region Send

        [TestMethod]
        public void SendSendsToProtocol()
        {
            _protocolMock.Setup(protocol => protocol.Send(It.Is<MockMessage>(message => message == _message)))
                .Verifiable();
            _clientNetService.Send(_message);

            _protocolMock.Verify();
        }

        #endregion
    }
}
