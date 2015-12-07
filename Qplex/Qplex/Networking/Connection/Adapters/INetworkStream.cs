using System;

namespace Qplex.Networking.Connection.Adapters
{
    /// <summary>
    /// Provides the underlying stream of data for network access.
    /// </summary>
    public interface INetworkStream
    {
        ///
        /// Summary:
        ///     Begins an asynchronous read from the System.Net.Sockets.NetworkStream.
        ///
        /// Parameters:
        ///   buffer:
        ///     An array of type System.Byte that is the location in memory to store data read
        ///     from the System.Net.Sockets.NetworkStream.
        ///
        ///   offset:
        ///     The location in buffer to begin storing the data.
        ///
        ///   size:
        ///     The number of bytes to read from the System.Net.Sockets.NetworkStream.
        ///
        ///   callback:
        ///     The System.AsyncCallback delegate that is executed when System.Net.Sockets.NetworkStream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        ///     completes.
        ///
        ///   state:
        ///     An object that contains any additional user-defined data.
        ///
        /// Returns:
        ///     An System.IAsyncResult that represents the asynchronous call.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     The buffer parameter is null.
        ///
        ///   T:System.ArgumentOutOfRangeException:
        ///     The offset parameter is less than 0.-or- The offset parameter is greater than
        ///     the length of the buffer paramater.-or- The size is less than 0.-or- The size
        ///     is greater than the length of buffer minus the value of the offset parameter.
        ///
        ///   T:System.IO.IOException:
        ///     The underlying System.Net.Sockets.Socket is closed.-or- There was a failure while
        ///     reading from the network. -or-An error occurred when accessing the socket. See
        ///     the Remarks section for more information.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The System.Net.Sockets.NetworkStream is closed.
        IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state);

        /// Summary:
        ///     Begins an asynchronous write to a stream.
        ///
        /// Parameters:
        ///   buffer:
        ///     An array of type System.Byte that contains the data to write to the System.Net.Sockets.NetworkStream.
        ///
        ///   offset:
        ///     The location in buffer to begin sending the data.
        ///
        ///   size:
        ///     The number of bytes to write to the System.Net.Sockets.NetworkStream.
        ///
        ///   callback:
        ///     The System.AsyncCallback delegate that is executed when System.Net.Sockets.NetworkStream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)
        ///     completes.
        ///
        ///   state:
        ///     An object that contains any additional user-defined data.
        ///
        /// Returns:
        ///     An System.IAsyncResult that represents the asynchronous call.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     The buffer parameter is null.
        ///
        ///   T:System.ArgumentOutOfRangeException:
        ///     The offset parameter is less than 0.-or- The offset parameter is greater than
        ///     the length of buffer.-or- The size parameter is less than 0.-or- The size parameter
        ///     is greater than the length of buffer minus the value of the offset parameter.
        ///
        ///   T:System.IO.IOException:
        ///     The underlying System.Net.Sockets.Socket is closed.-or- There was a failure while
        ///     writing to the network. -or-An error occurred when accessing the socket. See
        ///     the Remarks section for more information.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The System.Net.Sockets.NetworkStream is closed.
        IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state);

        ///
        /// Summary:
        ///     Handles the end of an asynchronous read.
        ///
        /// Parameters:
        ///   asyncResult:
        ///     An System.IAsyncResult that represents an asynchronous call.
        ///
        /// Returns:
        ///     The number of bytes read from the System.Net.Sockets.NetworkStream.
        ///
        /// Exceptions:
        ///   T:System.ArgumentException:
        ///     The asyncResult parameter is null.
        ///
        ///   T:System.IO.IOException:
        ///     The underlying System.Net.Sockets.Socket is closed.-or- An error occurred when
        ///     accessing the socket. See the Remarks section for more information.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The System.Net.Sockets.NetworkStream is closed.
        int EndRead(IAsyncResult asyncResult);

        ///
        /// Summary:
        ///     Handles the end of an asynchronous write.
        ///
        /// Parameters:
        ///   asyncResult:
        ///     The System.IAsyncResult that represents the asynchronous call.
        ///
        /// Exceptions:
        ///   T:System.ArgumentNullException:
        ///     The asyncResult parameter is null.
        ///
        ///   T:System.IO.IOException:
        ///     The underlying System.Net.Sockets.Socket is closed.-or- An error occurred while
        ///     writing to the network. -or-An error occurred when accessing the socket. See
        ///     the Remarks section for more information.
        ///
        ///   T:System.ObjectDisposedException:
        ///     The System.Net.Sockets.NetworkStream is closed.
        void EndWrite(IAsyncResult asyncResult);
    }
}