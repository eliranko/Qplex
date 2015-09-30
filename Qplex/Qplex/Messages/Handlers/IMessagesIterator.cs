namespace Qplex.Messages.Handlers
{
    /// <summary>
    /// Handles the order messages are handled
    /// </summary>
    public interface IMessagesIterator
    {
        /// <summary>
        /// Object used to notify the clients when there is a new message
        /// </summary>
        object Inventory { get; }

        /// <summary>
        /// Add message to handle
        /// </summary>
        /// <param name="message">Message</param>
        void Add(Message message);

        /// <summary>
        /// Get message to handle
        /// </summary>
        /// <returns>Message</returns>
        Message Next();

        /// <summary>
        /// Indicates whether the queue has messages
        /// </summary>
        /// <returns>True if the queue has messages, false otherwise.</returns>
        bool HasNext();
    }
}
