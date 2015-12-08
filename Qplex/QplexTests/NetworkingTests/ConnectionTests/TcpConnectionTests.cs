using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.Language.Flow;
using Qplex.Communication.Channels;
using Qplex.Messages;
using Qplex.Messages.Networking.Connection;
using Qplex.Networking.Connection;
using Qplex.Networking.Connection.Adapters.NetworkStream;
using Qplex.Networking.Connection.Adapters.Tcp;

namespace QplexTests.NetworkingTests.ConnectionTests
{
    [TestClass]
    public class TcpConnectionTests
    {
        private readonly IPAddress _ip = IPAddress.Parse("127.0.0.1");
        private const int Port = 589;

        private IConnection _connection;
        private Mock<IInternalChannel> _internalChannel;
        private Mock<INetworkStream> _streamMock;
        private Mock<IAsyncResult> _asyncResultMock;
        private Mock<ITcpClient> _tcpMock;
        private int _headerSize;
        private byte[] _buffer;

        [TestInitialize]
        public void TestInit()
        {
            //Async result mock
            _asyncResultMock = new Mock<IAsyncResult>();

            //Stream mock
            _streamMock = new Mock<INetworkStream>();
            _streamMock.Setup(
                stream =>
                    stream.BeginRead(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<AsyncCallback>(),
                        It.IsAny<object>())).Returns(_asyncResultMock.Object);

            //Tcp mock
            _tcpMock = new Mock<ITcpClient>();
            _tcpMock.Setup(client => client.Ip).Returns(_ip);
            _tcpMock.Setup(client => client.Port).Returns(Port);
            _tcpMock.Setup(client => client.GetStream()).Returns(_streamMock.Object);
            _headerSize = 4;

            _connection = new TcpConnection(_tcpMock.Object) {HeaderSize = _headerSize };
            _buffer = new[] {(byte) 1, (byte) 2};

            _internalChannel = new Mock<IInternalChannel>();
            _connection.SubscribeToChannel(_internalChannel.Object);
        }

        #region ConnectAndReceive

        [TestMethod]
        public void ConnectAndReceiveReturnsSocketAlreadyConnected()
        {
            _tcpMock.Setup(client => client.Connected).Returns(true).Verifiable();

            Assert.AreEqual(ConnectionConnectStatus.SocketAlreadyConnected, _connection.ConnectAndReceive());
            _tcpMock.Verify();
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsSuccess()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false).Verifiable();
            _tcpMock.Setup(
                client =>
                    client.Connect(It.Is<IPAddress>(address => address.Equals(_ip)),
                        It.Is<int>(i => i == Port))).Verifiable();

            Assert.AreEqual(ConnectionConnectStatus.Success, _connection.ConnectAndReceive());
            _tcpMock.Verify();
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsNullAddress()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false);
            _tcpMock.Setup(client => client.Connect(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Throws(new ArgumentNullException());

            Assert.AreEqual(ConnectionConnectStatus.NullAddress, _connection.ConnectAndReceive());
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsInvalidPort()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false);
            _tcpMock.Setup(client => client.Connect(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Throws(new ArgumentOutOfRangeException());

            Assert.AreEqual(ConnectionConnectStatus.InvalidPort, _connection.ConnectAndReceive());
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsSocketError()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false);
            _tcpMock.Setup(client => client.Connect(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Throws(new SocketException());

            Assert.AreEqual(ConnectionConnectStatus.SocketError, _connection.ConnectAndReceive());
        }

        [TestMethod]
        public void ConnectAndReceiveReturnsClientDisposed()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false);
            _tcpMock.Setup(client => client.Connect(It.IsAny<IPAddress>(), It.IsAny<int>()))
                .Throws(new ObjectDisposedException("SomeObject"));

            Assert.AreEqual(ConnectionConnectStatus.ClientDisposed, _connection.ConnectAndReceive());
        }

        #endregion

        #region Close

        [TestMethod]
        public void CloseCallsClientsClose()
        {
            _tcpMock.Setup(client => client.Close()).Verifiable();
            _connection.Close();

            _tcpMock.Verify();
        }

        #endregion

        #region Send

        [TestMethod]
        public void SendDoesntGetStream()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false);
            _tcpMock.Setup(client => client.GetStream()).Throws(new Exception("Send shouldn't get stream!"));
            _connection.Send(_buffer);
        }

        [TestMethod]
        public void SendCallsBeginWrite()
        {
            SetupAsyncWrite().Verifiable();
            _connection.Send(_buffer);

            _streamMock.Verify();
        }

        [TestMethod]
        public void SendReturnsSuccessOnCompletion()
        {
            SetupChannel()
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.Success,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();

            _streamMock.Setup(stream => stream.EndWrite(It.IsAny<IAsyncResult>())).Verifiable();
            SetupAsyncWrite()
                .Callback<byte[], int, int, AsyncCallback, object>(
                    (bytes, i, arg3, callback, arg5) => callback.Invoke(_asyncResultMock.Object));
            _connection.Send(_buffer);

            _streamMock.Verify();
            _internalChannel.Verify();
        }

        [TestMethod]
        public void SendReturnsSocketClosedOnCompletion()
        {
            SetupChannel()
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.SocketClosed,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();

            _streamMock.Setup(stream => stream.EndWrite(It.IsAny<IAsyncResult>())).Throws(new IOException());
            SetupAsyncWrite()
                .Callback<byte[], int, int, AsyncCallback, object>(
                    (bytes, i, arg3, callback, arg5) => callback.Invoke(_asyncResultMock.Object));
            _connection.Send(_buffer);

            _streamMock.Verify();
            _internalChannel.Verify();
        }

        [TestMethod]
        public void SendReturnsClientDisposedOnCompletion()
        {
            SetupChannel()
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.ClientDisposed,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();

            _streamMock.Setup(stream => stream.EndWrite(It.IsAny<IAsyncResult>())).Throws(new ObjectDisposedException(""));
            SetupAsyncWrite()
                .Callback<byte[], int, int, AsyncCallback, object>(
                    (bytes, i, arg3, callback, arg5) => callback.Invoke(_asyncResultMock.Object));
            _connection.Send(_buffer);

            _streamMock.Verify();
            _internalChannel.Verify();
        }

        [TestMethod]
        public void SendReturnsSocketClosedOnCompletionWithInvalidOperationException()
        {
            SetupChannel()
                .Callback<Message, Guid>(
                    (message, guid) =>
                        Assert.AreEqual(ConnectionSocketStatus.SocketClosed,
                            (message as ConnectionSendStatusMessage)?.ConnectionSocketStatus))
                .Verifiable();

            _streamMock.Setup(stream => stream.EndWrite(It.IsAny<IAsyncResult>())).Throws(new InvalidOperationException());
            SetupAsyncWrite()
                .Callback<byte[], int, int, AsyncCallback, object>(
                    (bytes, i, arg3, callback, arg5) => callback.Invoke(_asyncResultMock.Object));
            _connection.Send(_buffer);

            _streamMock.Verify();
            _internalChannel.Verify();
        }

        #endregion

        #region Read

        [TestMethod]
        public void ReceiveOnSuccessfulConnection()
        {
            SetupAsyncRead().Verifiable();
            Assert.AreEqual(ConnectionConnectStatus.Success, _connection.ConnectAndReceive());
            _tcpMock.Verify();
        }

        #endregion

        #region Helpers

        private ISetup<INetworkStream, IAsyncResult> SetupAsyncWrite()
        {
            _tcpMock.Setup(client => client.Connected).Returns(true);
            return _streamMock.Setup(
                stream =>
                    stream.BeginWrite(It.Is<byte[]>(bytes => bytes.Equals(_buffer)), It.Is<int>(i => i == 0),
                        It.Is<int>(i => i == _buffer.Length), It.IsAny<AsyncCallback>(), It.IsAny<object>()));
        }

        private ISetup<INetworkStream, IAsyncResult> SetupAsyncRead()
        {
            _tcpMock.Setup(client => client.Connected).Returns(false);
            return _streamMock.Setup(
                stream =>
                    stream.BeginRead(It.Is<byte[]>(bytes => bytes.Length == _headerSize), It.Is<int>(i => i == 0),
                        It.Is<int>(i => i == _headerSize), It.IsAny<AsyncCallback>(), It.IsAny<object>()));
        }

        private ISetup<IInternalChannel> SetupChannel()
        {
            return
                _internalChannel.Setup(
                    channel => channel.Broadcast(It.IsAny<ConnectionSendStatusMessage>(), It.IsAny<Guid>()));
        }

        #endregion
    }
}
