using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace QplexTests.CommunicationTests.ChannelsTests
{
    [TestClass]
    public class InternalChannelTests
    {
        private IInternalChannel _internalChannel;
        private ICommunicator _communicator;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _internalChannel = new InternalChannel();
            _communicator = new Communicator<QueueMessagesIterator>();
            _message = new MockMessage();
        }

        #region Subscribe

        [TestMethod]
        public void SubscribinNullReturnsFalse()
        {
            Assert.IsFalse(_internalChannel.Subscribe(null), "Subscribed null and returned true");
        }

        [TestMethod]
        public void SubscribinNewBroadcasterReturnsTrue()
        {
            Assert.IsTrue(_internalChannel.Subscribe(_communicator), "Subscribed broadcaster and returned false");
        }

        [TestMethod]
        public void SubscribingSubscribedBroadcasterReturnsFalse()
        {
            Assert.IsTrue(_internalChannel.Subscribe(_communicator), "Subscribed broadcaster and returned false");
            Assert.IsFalse(_internalChannel.Subscribe(_communicator), "Subscribed same broadcaster twice, and returned true");
        }

        [TestMethod]
        public void SubscribingDifferentBroadcastersReturnsTrue()
        {
            Assert.IsTrue(_internalChannel.Subscribe(_communicator), "Subscribed broadcaster and returned false");
            Assert.IsTrue(_internalChannel.Subscribe(new Broadcaster()), "Subscribed broadcaster and returned false");
        }

        #endregion

        #region Unsubscribe

        [TestMethod]
        public void UnsubscribinNullReturnsFalse()
        {
            Assert.IsFalse(_internalChannel.Unsubscribe(null), "Unsubscribed null and returned true");
        }

        #endregion

        #region Integration

        [TestMethod]
        public void SubscribingThenUnsbscribingSameBroadcasterReturnsTrue()
        {
            Assert.IsTrue(_internalChannel.Subscribe(_communicator), "Subscribed broadcaster and returned false");
            Assert.IsTrue(_internalChannel.Unsubscribe(_communicator), "Unsubscribed broadcaster and returned false");
        }

        [TestMethod]
        public void UnsubscribingSameBroadcasterTwiceReturnsFalse()
        {
            Assert.IsTrue(_internalChannel.Subscribe(_communicator), "Subscribed broadcaster and returned false");
            Assert.IsTrue(_internalChannel.Unsubscribe(_communicator), "Unsubscribed broadcaster and returned false");
            Assert.IsFalse(_internalChannel.Unsubscribe(_communicator), "Unsubscribed same broadcaster twice and returned true");
        }

        [TestMethod]
        public void SubscribingUnsubscribingAndSubscribingSameBroadcasterReturnsTrue()
        {
            Assert.IsTrue(_internalChannel.Subscribe(_communicator), "Subscribed broadcaster and returned false");
            Assert.IsTrue(_internalChannel.Unsubscribe(_communicator), "Unsubscribed broadcaster and returned false");
            Assert.IsTrue(_internalChannel.Subscribe(_communicator),
                "Tried to subscribe broadcaster after unsubscribing it, and returned false");
        }

        [TestMethod]
        public void BroadcastDoesntBroadcastToBroadcaster()
        {
            var broadcaster = new Mock<ICommunicator>();
            broadcaster.Setup(communicator => communicator.NewMessage(It.IsAny<Message>())).Throws(new Exception("Broadcasted to self"));
            _internalChannel.Subscribe(broadcaster.Object);
            _internalChannel.Broadcast(_message, broadcaster.Object.TypeGuid);
        }

        [TestMethod]
        public void BroadcastBroadcastsToOtherCommunicators()
        {
            var communicator = new Mock<ICommunicator>();
            communicator.Setup(
                communicator1 =>
                    communicator1.NewMessage(It.IsAny<MockMessage>())).Verifiable();

            _internalChannel.Subscribe(_communicator);
            _internalChannel.Subscribe(communicator.Object);
            _internalChannel.Broadcast(_message, _communicator.TypeGuid);

            communicator.Verify();
        }

        #endregion
    }
}
