using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Parser;

namespace QplexTests.CommunicationTests.HandlersTests
{
    [TestClass]
    public class DispatcherTests
    {
        private IDispatcher _dispatcher;
        private Mock<IDispatcherThread> _dispatcherThread;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _dispatcher = new Dispatcher<QueueMessagesIterator>("Some string");
            _dispatcherThread = new Mock<IDispatcherThread>();
            _message = new MockMessage();
            _dispatcherThread.SetupGet(thread => thread.Name).Returns("");
            OverrideDisaptcherThread(_dispatcher);
        }

        #region Start

        [TestMethod]
        public void StartReturnsTrueWhenThreadReturnTrue()
        {
            _dispatcherThread.Setup(thread => thread.Start()).Returns(true).Verifiable();
            Assert.IsTrue(_dispatcher.Start());

            _dispatcherThread.Verify();
        }

        [TestMethod]
        public void StartReturnsFalseWhenThreadFails()
        {
            _dispatcherThread.Setup(thread => thread.Start()).Returns(false).Verifiable();
            Assert.IsFalse(_dispatcher.Start());

            _dispatcherThread.Verify();
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopCallsThread()
        {
            _dispatcherThread.Setup(thread => thread.Stop()).Verifiable();
            _dispatcher.Stop();

            _dispatcherThread.Verify();
        }

        #endregion

        #region AddHandler

        [TestMethod]
        public void AddHandlerCallsThread()
        {
            _dispatcherThread.Setup(thread =>
                thread.AddHandler(It.Is<Type>(type => type == typeof (ParserConnectionErrorMessage)),
                    It.IsAny<Action<Message>>())).Verifiable();
            _dispatcher.AddHandler(typeof(ParserConnectionErrorMessage), message => {});

            _dispatcherThread.Verify();
        }

        [TestMethod]
        public void AddHandlerDoesntCallThread()
        {
            _dispatcherThread.Setup(thread => thread.AddHandler(It.IsAny<Type>(), It.IsAny<Action<Message>>()))
                .Throws(new Exception("Thread's AddHandler should not be called"));
            _dispatcher.AddHandler(typeof(ParserConnectionErrorMessage), message => { }, "NotDefaultThread");
        }

        #endregion

        #region Dispatch

        [TestMethod]
        public void DispatchDoesntCallThread()
        {
            _dispatcherThread.Setup(thread => thread.Dispatch(It.IsAny<Message>()))
                .Throws(new Exception("Thread's Dispatch should not be called"));

            _dispatcher.Dispatch(_message);
        }

        #endregion

        #region IsHandled

        [TestMethod]
        public void IsHandledReturnsFalse()
        {
            Assert.IsFalse(_dispatcher.IsHandled(typeof(MockMessage)));
        }

        #endregion

        #region Integration

        [TestMethod]
        public void DispatchToThreadAfterAddingHandler()
        {
            _dispatcherThread.Setup(thread => thread.Dispatch(It.Is<MockMessage>(message => message == _message))).Verifiable();
            _dispatcherThread.Setup(thread => thread.GetHandledMessages())
                .Returns(new List<Type> {typeof (MockMessage)});
            _dispatcher.AddHandler(typeof(MockMessage), message => {});
            _dispatcher.Dispatch(_message);

            _dispatcherThread.Verify(thread => thread.Dispatch(It.IsAny<Message>()), Times.Once);
        }

        [TestMethod]
        public void IsHandledReturnsTrueAfterAddingHandler()
        {
            _dispatcherThread.Setup(thread => thread.GetHandledMessages())
                .Returns(new List<Type> { typeof(MockMessage) });
            _dispatcher.AddHandler(typeof(MockMessage), message => { });
            Assert.IsTrue(_dispatcher.IsHandled(typeof(MockMessage)));
        }

        #endregion

        #region Helpers

        private void OverrideDisaptcherThread(IDispatcher instance)
        {
            Helper.SetInstanceField(typeof (Dispatcher<QueueMessagesIterator>), instance, "_threadsList",
                new List<IDispatcherThread> {_dispatcherThread.Object});
        }

        #endregion
    }
}