using System.Threading;
using NLog;
using Qplex.Communication.Handlers;

namespace Qplex.Networking.Listeners
{
    /// <summary>
    /// Network listener
    /// </summary>
    public abstract class Listener : Broadcaster, IListener
    {
        /// <summary>
        /// Listening thread
        /// </summary>
        protected readonly Thread ListeningThread;

        /// <summary>
        /// Ctor
        /// </summary>
        protected Listener()
        {
            ListeningThread = new Thread(Listen);
        }

        /// <summary>
        /// Start listener thread
        /// </summary>
        /// <returns>Operation status</returns>
        public bool Start()
        {
            Log(LogLevel.Debug, "Starting...");
            ListeningThread.Start();

            return true;
        }

        /// <summary>
        /// Stop listener's thread
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// Listener thread
        /// </summary>
        protected abstract void Listen();
    }
}
