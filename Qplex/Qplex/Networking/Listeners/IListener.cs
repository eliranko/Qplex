using Qplex.Communication.Handlers;

namespace Qplex.Networking.Listeners
{
    /// <summary>
    /// Network listener
    /// </summary>
    public interface IListener : IBroadcaster
    {
        /// <summary>
        /// Start listener thread
        /// </summary>
        /// <returns>Operation status</returns>
        bool Start();

        /// <summary>
        /// Stop listener's thread
        /// </summary>
        void Stop();
    }
}