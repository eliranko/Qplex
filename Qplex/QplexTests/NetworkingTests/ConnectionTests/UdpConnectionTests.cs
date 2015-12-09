using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Qplex.Communication.Channels;
using Qplex.Messages;
using Qplex.Messages.Networking.Connection;
using Qplex.Networking.Connection;
using Qplex.Networking.Connection.Adapters.Udp;

namespace QplexTests.NetworkingTests.ConnectionTests
{
    [TestClass]
    public class UdpConnectionTests
    {
        private UdpConnection _udpConnection;
        private Mock<IUdpClient> _udpMock;
        private Mock<IInternalChannel> _channelMock;
        private Mock<IAsyncResult> _resultMock;
        private IPAddress _ip;
        private int _port;
        private byte[] _buffer;

        [TestInitialize]
        public void TestInit()
        {
            _ip = IPAddress.Parse("127.0.0.1");
            _port = 589;
            _udpMock = new Mock<IUdpClient>();
            _udpMock.Setup(client => client.Ip).Returns(_ip);
            _udpMock.Setup(client => client.Port).Returns(_port);
            _udpConnection = new UdpConnection(_udpMock.Object);

            _channelMock = new Mock<IInternalChannel>();
            _udpConnection.SubscribeToChannel(_channelMock.Object);
            _buffer = new byte[0];

            _resultMock = new Mock<IAsyncResult>();
        }

        #region ConnectAndReceive

        [TestMethod]
        public void ConnectAndReceiveReturnsSuccess()
        {
            _udpMock.Setup(client => client.Connect(It.IsAny<IPEndPoint>())).Verifiable();

            Assert.AreEqual(ConnectionConnectStatus.Success, _udpConnection.ConnectAndReceive());
            _udpMock.Verify();
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsSocketError()
        {
            _udpMock.Setup(client => client.Connect(It.IsAny<IPEndPoint>())).Throws(new SocketException());

            Assert.AreEqual(ConnectionConnectStatus.SocketError, _udpConnection.ConnectAndReceive());
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsNullAddress()
        {
            _udpMock.Setup(client => client.Connect(It.IsAny<IPEndPoint>())).Throws(new ArgumentNullException());

            Assert.AreEqual(ConnectionConnectStatus.NullAddress, _udpConnection.ConnectAndReceive());
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsClientDisposed()
        {
            _udpMock.Setup(client => client.Connect(It.IsAny<IPEndPoint>())).Throws(new ObjectDisposedException(""));

            Assert.AreEqual(ConnectionConnectStatus.ClientDisposed, _udpConnection.ConnectAndReceive());
        }

        #endregion

        #region Close

        [TestMethod]
        public void CloseClosesClient()
        {
            _udpMock.Setup(client => client.Close()).Verifiable();
            _udpConnection.Close();

            _udpMock.Verify();
        }

        #endregion

        #region Send

        [TestMethod]
        public void SendBroadcastsSuccess()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ConnectionSendStatusMessage>(), It.IsAny<Guid>()))
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.Success,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();

            _udpConnection.Send(_buffer);
            _channelMock.Verify();
        }

        [TestMethod]
        public void SendBroadcastsErrorOnArgumentNullException()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ConnectionSendStatusMessage>(), It.IsAny<Guid>()))
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.Error,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();
            _udpMock.Setup(client => client.Send(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new ArgumentNullException());

            _udpConnection.Send(_buffer);
            _channelMock.Verify();
        }

        [TestMethod]
        public void SendBroadcastsClientDisposedOnObjectDisposedException()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ConnectionSendStatusMessage>(), It.IsAny<Guid>()))
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.ClientDisposed,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();
            _udpMock.Setup(client => client.Send(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new ObjectDisposedException(""));

            _udpConnection.Send(_buffer);
            _channelMock.Verify();
        }

        [TestMethod]
        public void SendBroadcastsErrorOnInvalidOperationException()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ConnectionSendStatusMessage>(), It.IsAny<Guid>()))
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.Error,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();
            _udpMock.Setup(client => client.Send(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new InvalidOperationException(""));

            _udpConnection.Send(_buffer);
            _channelMock.Verify();
        }

        [TestMethod]
        public void SendBroadcastsSocketClosedOnSocketException()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ConnectionSendStatusMessage>(), It.IsAny<Guid>()))
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.SocketClosed,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();
            _udpMock.Setup(client => client.Send(It.IsAny<byte[]>(), It.IsAny<int>()))
                .Throws(new SocketException());

            _udpConnection.Send(_buffer);
            _channelMock.Verify();
        }

        #endregion

        #region Read

        private bool _callbackCalled;

        [TestMethod]
        public void ReadBroadcastsSuccess()
        {
            _channelMock.Setup(channel => channel.Broadcast(It.IsAny<ConnectionBufferReceivedMessage>(), It.IsAny<Guid>()))
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.Success,
                            (message as ConnectionBufferReceivedMessage)?.ConnectionSocketStatus))
                .Verifiable();
            SetupBeginReceive();
            _udpConnection.ConnectAndReceive();

            Assert.IsTrue(_callbackCalled);
            _channelMock.Verify();
        }

        private void SetupBeginReceive()
        {
            _udpMock.Setup(client => client.BeginReceive(It.IsAny<AsyncCallback>(), It.IsAny<object>()))
                .Callback<AsyncCallback, object>((callback, o) =>
                {
                    callback.Invoke(_resultMock.Object);
                    _callbackCalled = true;
                });
        }

        #endregion
    }
    
}
