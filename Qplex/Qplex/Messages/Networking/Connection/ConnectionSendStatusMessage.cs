using Qplex.Networking.Connection;

namespace Qplex.Messages.Networking.Connection
{
    /// <summary>
    /// Connection send status
    /// </summary>
    public class ConnectionSendStatusMessage : Message
    {
        /// <summary>
        /// Connection socket status
        /// </summary>
        public ConnectionSocketStatus ConnectionSocketStatus { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="connectionSocketStatus">Connection socket status</param>
        public ConnectionSendStatusMessage(ConnectionSocketStatus connectionSocketStatus)
        {
            ConnectionSocketStatus = connectionSocketStatus;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"ConnectionSendStatusMessage with socket status: {ConnectionSocketStatus}";
        }
    }
}
