using Microsoft.VisualStudio.TestTools.UnitTesting;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace QplexTests.MessagesTests.HandlersTests
{
    [TestClass]
    public class QueueMessagesIteratorTests
    {
        private IMessagesIterator _messagesIterator;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _messagesIterator = new QueueMessagesIterator();
            _message = new MockMessage();
        }

        #region HasNext

        [TestMethod]
        public void HasNextReturnsFalse()
        {
            Assert.IsFalse(_messagesIterator.HasNext());
        }

        #endregion

        #region Integration

        [TestMethod]
        public void AddAddsToQueue()
        {
            _messagesIterator.Add(_message);
            Assert.IsTrue(_messagesIterator.HasNext());
        }

        [TestMethod]
        public void NextRemovesElementFromQueue()
        {
            _messagesIterator.Add(_message);
            Assert.AreEqual(_message, _messagesIterator.Next());
            Assert.IsFalse(_messagesIterator.HasNext());
        }

        [TestMethod]
        public void AddAddsElementAfterNext()
        {
            _messagesIterator.Add(_message);
            Assert.AreEqual(_message, _messagesIterator.Next());
            _messagesIterator.Add(_message);
            Assert.IsTrue(_messagesIterator.HasNext());
        }

        #endregion
    }
}
