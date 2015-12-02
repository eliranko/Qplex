using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Qplex.Messages.Handlers
{
    /// <summary>
    /// Messages iterator implemented using queue
    /// </summary>
    public class QueueMessagesIterator : IMessagesIterator
    {
        /// <summary>
        /// Messages queue
        /// </summary>
        private readonly Queue<Message> _queue;

        /// <summary>
        /// Object used to notify the clients when there is a new message
        /// </summary>
        public object Inventory { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        public QueueMessagesIterator()
        {
            _queue = new Queue<Message>();
            Inventory = new object();
        }

        /// <summary>
        /// Add message to handle
        /// </summary>
        /// <param name="message">Message</param>
        public void Add(Message message)
        {
            lock (Inventory)
            {
                _queue.Enqueue(message);

                //If the queue is empty, a thread may be sleeping on Inventory.
                if (!HasNext())
                {
                    //Notify threads that a new message has arrived
                    Monitor.PulseAll(Inventory);
                }
            }
        }

        /// <summary>
        /// Get message to handle
        /// </summary>
        /// <returns>Message</returns>
        public Message Next()
        {
            return _queue.Dequeue();
        }

        /// <summary>
        /// Indicates whether the queue has messages
        /// </summary>
        /// <returns>True if the queue has messages, false otherwise.</returns>
        public bool HasNext()
        {
            return _queue.Any();
        }
    }
}
