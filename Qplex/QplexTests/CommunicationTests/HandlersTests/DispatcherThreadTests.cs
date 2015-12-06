using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace QplexTests.CommunicationTests.HandlersTests
{
    [TestClass]
    public class DispatcherThreadTests
    {
        private IDispatcherThread _dispatcherThread;
        private Mock<IMessagesIterator> _messageIteratorMock;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _messageIteratorMock = new Mock<IMessagesIterator>();
            _dispatcherThread = new DispatcherThread("Some name", _messageIteratorMock.Object);
            _message = new MockMessage();
        }

        #region GetHandledMessages

        [TestMethod]
        public void GetHandledMessagesReturnsEmptyIEnumerable()
        {
            Assert.IsFalse(_dispatcherThread.GetHandledMessages().Any());
        }

        #endregion

        #region Dispatch

        [TestMethod]
        public void DispatchAddsMessageToIterator()
        {
            _messageIteratorMock.Setup(iterator => iterator.Add(It.Is<MockMessage>(message => message == _message)))
                .Verifiable();
            
            _dispatcherThread.Dispatch(_message);
            _messageIteratorMock.Verify();
        }

        #endregion

        #region Integration

        [TestMethod]
        public void GetHandledMessagesReturnsCorrectIEnumerableAfterAddingHandler()
        {
            _dispatcherThread.AddHandler(_message.GetType(), message => { });
            var handlers = _dispatcherThread.GetHandledMessages().ToList();

            Assert.AreEqual(1, handlers.Count());
            Assert.AreEqual(_message.GetType(), handlers.First());
        }

        #endregion
    }
}
