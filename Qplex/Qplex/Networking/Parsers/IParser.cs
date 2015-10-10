using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace Qplex.Networking.Parsers
{
    /// <summary>
    /// Parser sends and receives Message objects.
    /// </summary>
    public interface IParser : ICommunicator
    {
        /// <summary>
        /// Connect
        /// </summary>
        void Connect();

        /// <summary>
        /// Close conneciton
        /// </summary>
        void Close();

        /// <summary>
        /// Send serialized message
        /// </summary>
        /// <param name="message">Message</param>
        void Send(Message message);
    }
}