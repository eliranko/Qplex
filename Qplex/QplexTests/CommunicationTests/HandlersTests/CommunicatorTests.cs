using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace QplexTests.CommunicationTests.HandlersTests
{
    class MockMessage2 : Message
    {
         
    }

    class SomeCommunicator : Communicator<QueueMessagesIterator>
    {
        [MessageHandler]
        public void SomeHandler(MockMessage message)
        {

        }

        [MessageHandler]
        public void AnotherHandler(MockMessage2 message)
        {

        }
    }

    class ZeroHandlersCommunicator : Communicator<QueueMessagesIterator>
    {
        
    }

    [TestClass]
    public class CommunicatorTests
    {
        private ICommunicator _communicator;
        private Mock<IDispatcher> _dispatcher;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _message = new MockMessage();
            _dispatcher = new Mock<IDispatcher>();
            _communicator = new Communicator<QueueMessagesIterator>();
            OverrideDisaptcher(_communicator);
        }

        #region Start

        [TestMethod]
        public void StartCallsDispatchersStartAndReturnsTrue()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Start()).Returns(true).Verifiable();
            Assert.IsTrue(_communicator.Start());

            _dispatcher.Verify();
        }

        [TestMethod]
        public void StartCallsDispatchersStartAndReturnsFalse()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Start()).Returns(false).Verifiable();
            Assert.IsFalse(_communicator.Start());

            _dispatcher.Verify();
        }

        [TestMethod]
        public void MessageHandlerIsAddedToDispatcher()
        {
            _dispatcher.Setup(
                dispatcher => dispatcher.AddHandler(It.IsAny<Type>(), It.IsAny<Action<Message>>(), It.IsAny<string>()))
                .Verifiable();

            var someCommunicator = new SomeCommunicator();
            OverrideDisaptcher(someCommunicator);
            someCommunicator.Start();
            _dispatcher.Verify(
                dispatcher => dispatcher.AddHandler(It.IsAny<Type>(), It.IsAny<Action<Message>>(), It.IsAny<string>()),
                Times.Exactly(2));
        }

        [TestMethod]
        public void NoMessageHandlerAddedToDispatcher()
        {
            _dispatcher.Setup(
                dispatcher => dispatcher.AddHandler(It.IsAny<Type>(), It.IsAny<Action<Message>>(), It.IsAny<string>()))
                .Verifiable();

            var someCommunicator = new ZeroHandlersCommunicator();
            OverrideDisaptcher(someCommunicator);
            someCommunicator.Start();
            _dispatcher.Verify(
                dispatcher => dispatcher.AddHandler(It.IsAny<Type>(), It.IsAny<Action<Message>>(), It.IsAny<string>()),
                Times.Never);
        }

        #endregion

        #region Stop

        [TestMethod]
        public void StopCallsDispatchersStop()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Stop()).Verifiable();
            _communicator.Stop();

            _dispatcher.Verify();
        }

        #endregion

        #region NewMessage

        [TestMethod]
        public void NewMessageDispatchesMessageToDispatcher()
        {
            VerifiableDispatch();
            _communicator.NewMessage(_message);
            _dispatcher.Verify();
        }

        #endregion

        #region Notify

        [TestMethod]
        public void NotifyReachesDispatcherDispatch()
        {
            VerifiableDispatch();
            _communicator.Notify(_message);
            _dispatcher.Verify();
        }

        #endregion

        #region TunnelMessage

        [TestMethod]
        public void TunnelMessageHandledReachesDispatcherDispatch()
        {
            VerifiableDispatch();
            _dispatcher.Setup(dispatcher => dispatcher.IsHandled(It.IsAny<Type>())).Returns(true).Verifiable();
            _communicator.TunnelMessage(_message, message => { });

            _dispatcher.Verify();
        }

        #endregion

        #region Expect

        [TestMethod]
        public void ExpectDoesntInvokeActionWhenMessageArrivesBeforeTimeoutExpires()
        {
            var action = new Action<Message>(message => Assert.Fail("Action should not be called!"));
            _communicator.Expect<MockMessage>(300, new MockMessage2(), action);

            Thread.Sleep(100);
            _communicator.NewMessage(new MockMessage());
            Thread.Sleep(200);
        }

        [TestMethod]
        public void ExpectInvokesActionWhenTimeoutExceeds()
        {
            var actionCalled = false;
            var action = new Action<Message>(message => actionCalled = true);
            _communicator.Expect<MockMessage>(5, new MockMessage2(), action);

            //TODO: Find a better way to test time-dependant methods
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(200);
                Assert.IsTrue(actionCalled);
            });
        }

        #endregion

        #region Integration

        [TestMethod]
        public void DispatcherDispatchIsntCalledWhenInitStackIsNotEmpty()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>()))
                .Throws(new Exception("Dispatch should not be called"));

            _communicator.SetInitMessages(typeof(MockMessage2));
            _communicator.NewMessage(_message);
        }

        [TestMethod]
        public void DispatcherCalledWhenNoInitMessagesAreLeft()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>())).Verifiable();
            _communicator.SetInitMessages(typeof(MockMessage2));
            _communicator.NewMessage(_message);
            _communicator.NewMessage(new MockMessage2());

            _dispatcher.Verify(dispatcher => dispatcher.Dispatch(It.IsAny<MockMessage2>()), Times.Once);
            _dispatcher.Verify(
                dispatcher => dispatcher.Dispatch(It.Is<MockMessage>(message => message == _message)),
                Times.Once);
        }

        [TestMethod]
        public void DispatcherNotRedundantlyCalledWhenNotInitMessagesLeft()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>())).Verifiable();
            _communicator.SetInitMessages(typeof(MockMessage2));
            _communicator.SetInitMessages(typeof(MockMessage));
            _communicator.NewMessage(_message);
            _communicator.NewMessage(new MockMessage2());

            _dispatcher.Verify(dispatcher => dispatcher.Dispatch(It.IsAny<Message>()), Times.Exactly(2));
        }


        [TestMethod]
        public void NotifyEmptiesInitMessagesStack()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>())).Verifiable();
            _communicator.SetInitMessages(typeof(MockMessage2));
            _communicator.Notify(new MockMessage2());
            _communicator.NewMessage(_message);

            _dispatcher.Verify(dispatcher => dispatcher.Dispatch(It.IsAny<Message>()), Times.Exactly(2));
        }
        #endregion

        #region Helpers

        private void VerifiableDispatch()
        {
            _dispatcher.Setup(
                dispatcher => dispatcher.Dispatch(It.Is<MockMessage>(message1 => message1 == _message)))
                .Verifiable();
        }

        private void OverrideDisaptcher(ICommunicator instance)
        {
            Helper.SetInstanceField(typeof(Communicator<QueueMessagesIterator>), instance, "_dispatcher", _dispatcher.Object);
        }

        #endregion
    }
}
