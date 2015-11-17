using Qplex.Attributes;
using Qplex.Communication.Handlers;
using Qplex.Messages;
using Qplex.Messages.Networking.Connection;

namespace Qplex.Networking.Parsers
{
    /// <summary>
    /// Parser sends and receives Message objects.
    /// </summary>
    public interface IParser : ICommunicator
    {
        /// <summary>
        /// Send serialized message
        /// </summary>
        void Send(Message message);

        /// <summary>
        /// Retrieve connection
        /// </summary>
        void RetrieveConnection();

        /// <summary>
        /// Handle received buffer from connection
        /// </summary>
        [MessageHandler]
        void HandleConnectionBufferReceivedMessage(ConnectionBufferReceivedMessage message);

        /// <summary>
        /// Handle send status received from the connection
        /// </summary>
        [MessageHandler]
        void HandleConnectionSendStatusMessage(ConnectionSendStatusMessage message);
    }
}