﻿using System.Net;
using System.Net.Sockets;

namespace Qplex.Networking.Connection.Adapters
{
    /// <summary>
    /// Tcp client adapter
    /// </summary>
    public interface ITcpClient
    {
        /// <summary>
        /// Gets a value indicating whether the underlying System.Net.Sockets.Socket for
        /// a System.Net.Sockets.TcpClient is connected to a remote host.
        /// </summary>
        /// <returns>
        /// true if the System.Net.Sockets.TcpClient.Client socket was connected to a remote
        /// resource as of the most recent operation; otherwise, false.
        /// </returns>
        bool Connected { get; }

        /// <summary>
        /// Gets or sets the underlying System.Net.Sockets.Socket.
        /// </summary>
        /// <returns>The underlying network System.Net.Sockets.Socket.</returns>
        Socket Client { get; }

        /// <summary>
        /// Gets the remote ip
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when attempting to access the socket.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.Net.Sockets.Socket has been closed.
        /// </exception>
        /// <returns>The ip with which the System.Net.Sockets.Socket is communicating.</returns>
        IPAddress Ip { get; }

        /// <summary>
        /// Gets the remote pot
        /// </summary>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when attempting to access the socket.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.Net.Sockets.Socket has been closed.
        /// </exception>
        /// <returns>The port with which the System.Net.Sockets.Socket is communicating.</returns>
        int Port { get; }

        /// <summary>
        /// Connects the client to a remote TCP host using the specified IP address and port
        ///     number.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// The address parameter is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// The port is not between System.Net.IPEndPoint.MinPort and System.Net.IPEndPoint.MaxPort.
        /// </exception>
        /// <exception cref="System.Net.Sockets.SocketException">
        /// An error occurred when accessing the socket. See the Remarks section for more
        ///     information.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// System.Net.Sockets.TcpClient is closed.
        /// </exception>
        /// <param name="address">The System.Net.IPAddress of the host to which you intend to connect.</param>
        /// <param name="port">The port number to which you intend to connect.</param>
        void Connect(IPAddress address, int port);

        /// <summary>
        /// Disposes this System.Net.Sockets.TcpClient instance and requests that the underlying
        ///     TCP connection be closed.
        /// </summary>
        void Close();

        /// <summary>
        /// Returns the System.Net.Sockets.NetworkStream used to send and receive data.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The System.Net.Sockets.TcpClient is not connected to a remote host.
        /// </exception>
        /// <exception cref="System.ObjectDisposedException">
        /// The System.Net.Sockets.TcpClient has been closed.
        /// </exception>
        /// <returns>The underlying System.Net.Sockets.NetworkStream.</returns>
        INetworkStream GetStream();
    }
}