using Qplex.Communication.Handlers;
using Qplex.Messages.Handlers;

namespace Qplex.Layers
{
    /// <summary>
    /// Layer object is used for internal(in process only) message passing and handling.
    /// </summary>
    /// /// <typeparam name="TIterator">Messages iterator</typeparam>
    public abstract class Layer<TIterator> : Communicator<TIterator>, ILayer
        where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Layer's init method. Initiate everything the layer needs before starting.
        /// </summary>
        /// <returns>Operation status. True on success, false otherwise.</returns>
        public abstract bool Init();
    }

    /// <summary>
    /// Layer implemented using queue message iterator
    /// </summary>
    public abstract class Layer : Layer<QueueMessagesIterator>
    {
        
    }
}
