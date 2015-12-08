using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Messages;
using Qplex.Messages.Networking;
using Qplex.Networking.Listeners;
using Qplex.Networking.NetService;
using Qplex.Networking.Parsers;
using Qplex.Networking.Protocols;

namespace QplexTests.NetworkingTests.NetServiceTests
{
    [TestClass]
    public class ListenerNetServiceTests
    {
        private IListenerNetService _listenerNetService;
        private Mock<IListener> _listenerMock;
        private Mock<IParser> _parserMock;

        [TestInitialize]
        public void TestInit()
        {
            _listenerMock = new Mock<IListener>();
            _parserMock = new Mock<IParser>();
            _listenerNetService = new ListenerNetService<Protocol>(_listenerMock.Object);
        }

        #region Start

        [TestMethod]
        public void StartReturnsFalseWhenListenerFails()
        {
            _listenerMock.Setup(listener => listener.Start()).Returns(false).Verifiable();
            Assert.IsFalse(_listenerNetService.Start());

            _listenerMock.Verify();
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopCallsListenersStop()
        {
            _listenerMock.Setup(listener => listener.Stop()).Verifiable();
            _listenerNetService.Stop();

            _listenerMock.Verify();
        }

        #endregion

        #region Intergation

        [TestMethod]
        public void SendReachesPrasersSend()
        {
            _parserMock.Setup(parser => parser.Send(It.IsAny<Message>())).Verifiable();
            _listenerNetService.HandleNewConnectionMessage(new NewConnectionMessage(_parserMock.Object));
            _listenerNetService.Send(new MockMessage());

            _parserMock.Verify();
        }

        #endregion
    }
}
