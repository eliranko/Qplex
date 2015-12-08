using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Messages;
using Qplex.Messages.Networking.Parser;
using Qplex.Networking.Parsers;
using Qplex.Networking.Protocols;

namespace QplexTests.NetworkingTests.ProtocolsTests
{
    [TestClass]
    public class ProtocolTests
    {
        private IProtocol _protocol;
        private Mock<IParser> _parserMock;

        [TestInitialize]
        public void TestInit()
        {
            _parserMock = new Mock<IParser>();
            _protocol = new Protocol(_parserMock.Object);
        }

        #region Start

        [TestMethod]
        public void StartReturnsFalseWhenParserFails()
        {
            _parserMock.Setup(parser => parser.Start()).Returns(false).Verifiable();
            
            Assert.IsFalse(_protocol.Start());
            _parserMock.Verify();
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopCallsParsersStop()
        {
            _parserMock.Setup(parser => parser.Stop()).Verifiable();
            _protocol.Stop();

            _parserMock.Verify();
        }

        #endregion

        #region Send

        [TestMethod]
        public void SendCallsParsersSend()
        {
            _parserMock.Setup(parser => parser.Send(It.IsAny<Message>())).Verifiable();
            _protocol.Send(new MockMessage());

            _parserMock.Verify();
        }

        #endregion

        #region HandleConnectionSocketErrorMessage

        [TestMethod]
        public void HandleConnectionSocketErrorMessageTriesToRetreievConnection()
        {
            _parserMock.Setup(parser => parser.RetrieveConnection()).Verifiable();
            _protocol.HandleConnectionSocketErrorMessage(new ParserConnectionErrorMessage());

            _parserMock.Verify();
        }

        #endregion
    }
}
