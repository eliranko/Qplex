using System.Threading;
using Qplex.Communication.Handlers;
using Qplex.MessageFactories;

namespace Qplex.Networking
{
    /// <summary>
    /// Network listener
    /// </summary>
    public abstract class Listener<T, TU> : Broadcaster where T : IConnection where TU : IMessageFactory
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
