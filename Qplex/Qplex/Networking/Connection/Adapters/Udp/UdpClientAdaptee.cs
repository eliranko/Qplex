using System;
using System.Net;
using System.Net.Sockets;

namespace Qplex.Networking.Connection.Adapters.Udp
{
    /// <summary>
    /// Udp client adaptee
    /// </summary>
    public class UdpClientAdaptee : IUdpClient
    {
        /// <summary>
        /// Udp client
        /// </summary>
        private readonly UdpClient _udpClient;

        private IPEndPoint _endPoint;

        /// Summary:
        ///     Initializes a new instance of the System.Net.Sockets.UdpClient class and binds
        ///     it to the specified local endpoint.
        ///
        /// Parameters:
        ///   localEP:
        ///     An System.Net.IPEndPoint that respresents the local endpoint to which you bind
        ///     the UDP connection.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     localEP is null.
        ///
        ///   T:System.Net.Sockets.SocketException:
        ///     An error occurred when accessing the socket. See the Remarks section for more
        ///     information.
        public UdpClientAdaptee(IPEndPoint localEp)
        {
            _endPoint = localEp;
            _udpClient = new UdpClient(localEp);
        }

        /// <summary>
        /// Remote host ip address
        /// </summary>
        public IPAddress Ip => _endPoint.Address;

        /// <summary>
        /// Remote host port
        /// </summary>
        public int Port => _endPoint.Port;

        /// Summary:
        ///     Establishes a default remote host using the specified network endpoint.
        ///
        /// Parameters:
        ///   endPoint:
        ///     An System.Net.IPEndPoint that specifies the network endpoint to which you intend
        ///     to send data.
        ///
        /// Exceptions:
        ///   T:System.Net.Sockets.SocketException:
        ///     An error occurred when accessing the socket. See the Remarks section for more
        ///     information.
        ///
        ///   T:System.ArgumentNullException:
        ///     endPoint is null.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The System.Net.Sockets.UdpClient is closed.
        public void Connect(IPEndPoint endPoint)
        {
            _endPoint = endPoint;
            _udpClient.Connect(endPoint);
        }

        /// Summary:
        ///     Closes the UDP connection.
        ///
        /// Exceptions:
        ///   T:System.Net.Sockets.SocketException:
        ///     An error occurred when accessing the socket.
        public void Close()
        {
            _udpClient.Close();
        }

        /// Summary:
        ///     Sends a UDP datagram to a remote host.
        ///
        /// Parameters:
        ///   dgram:
        ///     An array of type System.Byte that specifies the UDP datagram that you intend
        ///     to send represented as an array of bytes.
        ///
        ///   bytes:
        ///     The number of bytes in the datagram.
        ///
        /// Returns:
        ///     The number of bytes sent.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     dgram is null.
        ///
        ///   T:System.InvalidOperationException:
        ///     The System.Net.Sockets.UdpClient has already established a default remote host.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The System.Net.Sockets.UdpClient is closed.
        ///
        ///   T:System.Net.Sockets.SocketException:
        ///     An error occurred when accessing the socket. See the Remarks section for more
        ///     information.
        public int Send(byte[] dgram, int bytes)
        {
            return _udpClient.Send(dgram, bytes);
        }

        /// Summary:
        ///     Receives a datagram from a remote host asynchronously.
        ///
        /// Parameters:
        ///   requestCallback:
        ///     An System.AsyncCallback delegate that references the method to invoke when the
        ///     operation is complete.
        ///
        ///   state:
        ///     A user-defined object that contains information about the receive operation.
        ///     This object is passed to the requestCallback delegate when the operation is complete.
        ///
        /// Returns:
        ///     An System.IAsyncResult object that references the asynchronous receive.
        public IAsyncResult BeginReceive(AsyncCallback requestCallback, object state)
        {
            return _udpClient.BeginReceive(requestCallback, state);
        }

        /// Summary:
        ///     Ends a pending asynchronous receive.
        ///
        /// Parameters:
        ///   asyncResult:
        ///     An System.IAsyncResult object returned by a call to System.Net.Sockets.UdpClient.BeginReceive(System.AsyncCallback,System.Object).
        ///
        ///   remoteEP:
        ///     The specified remote endpoint.
        ///
        /// Returns:
        ///     If successful, the number of bytes received. If unsuccessful, this method returns
        ///     0.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     asyncResult is null.
        ///
        ///   T:System.ArgumentException:
        ///     asyncResult was not returned by a call to the System.Net.Sockets.UdpClient.BeginReceive(System.AsyncCallback,System.Object)
        ///     method.
        ///
        ///   T:System.InvalidOperationException:
        ///     System.Net.Sockets.UdpClient.EndReceive(System.IAsyncResult,System.Net.IPEndPoint@)
        ///     was previously called for the asynchronous read.
        ///
        ///   T:System.Net.Sockets.SocketException:
        ///     An error occurred when attempting to access the underlying System.Net.Sockets.Socket.
        ///     See the Remarks section for more information.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The underlying System.Net.Sockets.Socket has been closed.
        public byte[] EndReceive(IAsyncResult asyncResult, ref IPEndPoint remoteEp)
        {
            return _udpClient.EndReceive(asyncResult, ref remoteEp);
        }
    }
}
