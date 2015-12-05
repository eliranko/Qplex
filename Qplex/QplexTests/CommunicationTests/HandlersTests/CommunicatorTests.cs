using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;
using Qplex.Messages.Networking.Parser;

namespace QplexTests.CommunicationTests.HandlersTests
{
    class SomeCommunicator : Communicator<QueueMessagesIterator>
    {
        [MessageHandler]
        public void SomeHandler(ParserConnectionErrorMessage message)
        {

        }

        [MessageHandler]
        public void AnotherHandler(NewIncomingMessage message)
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
            _message = new ParserConnectionErrorMessage();
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

        #region Integration

        [TestMethod]
        public void DispatcherDispatchIsntCalledWhenInitStackIsNotEmpty()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>()))
                .Throws(new Exception("Dispatch should not be called"));

            _communicator.SetInitMessages(typeof(NewIncomingMessage));
            _communicator.NewMessage(_message);
        }

        [TestMethod]
        public void DispatcherCalledWhenNoInitMessagesAreLeft()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>())).Verifiable();
            _communicator.SetInitMessages(typeof(NewIncomingMessage));
            _communicator.NewMessage(_message);
            _communicator.NewMessage(new NewIncomingMessage(_message));

            _dispatcher.Verify(dispatcher => dispatcher.Dispatch(It.IsAny<NewIncomingMessage>()), Times.Once);
            _dispatcher.Verify(
                dispatcher => dispatcher.Dispatch(It.Is<ParserConnectionErrorMessage>(message => message == _message)),
                Times.Once);
        }

        [TestMethod]
        public void DispatcherNotRedundantlyCalledWhenNotInitMessagesLeft()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>())).Verifiable();
            _communicator.SetInitMessages(typeof(NewIncomingMessage));
            _communicator.SetInitMessages(typeof(ParserConnectionErrorMessage));
            _communicator.NewMessage(_message);
            _communicator.NewMessage(new NewIncomingMessage(_message));

            _dispatcher.Verify(dispatcher => dispatcher.Dispatch(It.IsAny<Message>()), Times.Exactly(2));
        }


        [TestMethod]
        public void NotifyEmptiesInitMessagesStack()
        {
            _dispatcher.Setup(dispatcher => dispatcher.Dispatch(It.IsAny<Message>())).Verifiable();
            _communicator.SetInitMessages(typeof(NewIncomingMessage));
            _communicator.Notify(new NewIncomingMessage(_message));
            _communicator.NewMessage(_message);

            _dispatcher.Verify(dispatcher => dispatcher.Dispatch(It.IsAny<Message>()), Times.Exactly(2));
        }
        #endregion

        #region Helpers

        private void VerifiableDispatch()
        {
            _dispatcher.Setup(
                dispatcher => dispatcher.Dispatch(It.Is<ParserConnectionErrorMessage>(message1 => message1 == _message)))
                .Verifiable();
        }

        private void OverrideDisaptcher(ICommunicator instance)
        {
            Helper.SetInstanceField(typeof(Communicator<QueueMessagesIterator>), instance, "_dispatcher", _dispatcher.Object);
        }

        #endregion
    }
}
