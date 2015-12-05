using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Channels;
using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace QplexTests.CommunicationTests.HandlersTests
{
    [TestClass]
    public class BroadcasterTests
    {
        private Mock<IInternalChannel> _internalChannel;
        private IBroadcaster _broadcaster;
        private Message _message;

        [TestInitialize]
        public void TestInit()
        {
            _broadcaster = new Broadcaster();
            _internalChannel = new Mock<IInternalChannel>();
            _message = new MockMessage();
            _internalChannel.Setup(channel => channel.Subscribe(It.IsAny<IBroadcaster>())).Returns(true);
            _internalChannel.Setup(channel => channel.Unsubscribe(It.IsAny<IBroadcaster>())).Returns(true);
        }

        #region GetInternalChannels

        [TestMethod]
        public void GetInternalChannelsReturnsEmptyIEnumerable()
        {
            Assert.IsFalse(_broadcaster.GetInternalChannels().Any());
        }

        #endregion

        #region SubscribeToChannel

        [TestMethod]
        public void SubscribeToChannelReturnsFalseWhenChannelIsNull()
        {
            Assert.IsFalse(_broadcaster.SubscribeToChannel(null));
        }

        [TestMethod]
        public void SubscribeToChannelReturnsFalseWhenChannelFailsToSubscribe()
        {
            _internalChannel.Setup(channel => channel.Subscribe(It.IsAny<IBroadcaster>())).Returns(false);
            Assert.IsFalse(_broadcaster.SubscribeToChannel(_internalChannel.Object));
        }

        [TestMethod]
        public void SubscribeToChannelReturnsTrue()
        {
            Assert.IsTrue(_broadcaster.SubscribeToChannel(_internalChannel.Object));
        }

        [TestMethod]
        public void SubscribeToChannelReturnsFalseWhenSubscribingTwiceToSameChannel()
        {
            Assert.IsTrue(_broadcaster.SubscribeToChannel(_internalChannel.Object));
            Assert.IsFalse(_broadcaster.SubscribeToChannel(_internalChannel.Object));
        }

        #endregion

        #region UnsubscribeFromChannel

        [TestMethod]
        public void UnsubscribeFromChannelReturnsFalseWhenChannelIsNull()
        {
            Assert.IsFalse(_broadcaster.UnsubscribeFromChannel(null));
        }

        [TestMethod]
        public void UnsubscribeFromChannelReturnsFalseWhenChannelFailsToSubscribe()
        {
            _internalChannel.Setup(channel => channel.Unsubscribe(It.IsAny<IBroadcaster>())).Returns(false);
            Assert.IsFalse(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
        }

        [TestMethod]
        public void UnsubscribeFromChannelReturnsFalseWhenUnsubscribingFromUnfamiliarChannel()
        {
            Assert.IsFalse(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
        }

        #endregion

        #region Broadcast

        [TestMethod]
        public void BroadcastBroadcastsToChannel()
        {
            _internalChannel.Setup(
                channel =>
                    channel.Broadcast(It.Is<MockMessage>(message1 => message1 == _message),
                        It.Is<Guid>(guid => guid == _broadcaster.TypeGuid))).Verifiable();

            _broadcaster.SubscribeToChannel(_internalChannel.Object);
            _broadcaster.Broadcast(_message);
            _internalChannel.Verify();
        }

        [TestMethod]
        public void BroadcastDoesntBroadcastToUnsubscribedChannel()
        {
            _internalChannel.Setup(channel => channel.Broadcast(It.IsAny<Message>(), It.IsAny<Guid>()))
                .Throws(new Exception("Broadcast to unsubscribed channel"));

            _broadcaster.SubscribeToChannel(_internalChannel.Object);
            _broadcaster.UnsubscribeFromChannel(_internalChannel.Object);
            _broadcaster.Broadcast(new MockMessage());
        }

        #endregion

        #region Integration

        [TestMethod]
        public void GetInternalChannelsReturnsSubscribedChannel()
        {
            _broadcaster.SubscribeToChannel(_internalChannel.Object);
            var channelsList = _broadcaster.GetInternalChannels().ToList();
            Assert.AreEqual(1, channelsList.Count());
            Assert.AreEqual(_internalChannel.Object, channelsList.First());
        }

        [TestMethod]
        public void GetInternalChannelsReturnsEmptyIEnumerableWhenUnsubscribingFromChannels()
        {
            _broadcaster.SubscribeToChannel(_internalChannel.Object);
            _broadcaster.UnsubscribeFromChannel(_internalChannel.Object);
            Assert.IsFalse(_broadcaster.GetInternalChannels().Any());
        }

        [TestMethod]
        public void GetInternalChannelsReturnsChannelsWhenSubscribingThenUnsubscribingThenSubscribing()
        {
            _broadcaster.SubscribeToChannel(_internalChannel.Object);
            _broadcaster.UnsubscribeFromChannel(_internalChannel.Object);
            _broadcaster.SubscribeToChannel(_internalChannel.Object);

            var channelsList = _broadcaster.GetInternalChannels().ToList();
            Assert.AreEqual(1, channelsList.Count());
            Assert.AreEqual(_internalChannel.Object, channelsList.First());
        }

        [TestMethod]
        public void UnsubscribeFromChannelReturnsTrue()
        {
            Assert.IsTrue(_broadcaster.SubscribeToChannel(_internalChannel.Object));
            Assert.IsTrue(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
        }

        [TestMethod]
        public void UnsubscribeFromChannelReturnsFalseWhenUnsubscribingTwiceToSameChannel()
        {
            Assert.IsTrue(_broadcaster.SubscribeToChannel(_internalChannel.Object));
            Assert.IsTrue(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
            Assert.IsFalse(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
        }

        [TestMethod]
        public void TwiceSubscribingAndUnsubscribingReturnsTrue()
        {
            Assert.IsTrue(_broadcaster.SubscribeToChannel(_internalChannel.Object));
            Assert.IsTrue(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
            Assert.IsTrue(_broadcaster.SubscribeToChannel(_internalChannel.Object));
            Assert.IsTrue(_broadcaster.UnsubscribeFromChannel(_internalChannel.Object));
        }

        #endregion
    }
}
