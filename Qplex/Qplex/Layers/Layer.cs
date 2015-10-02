using System;
using Qplex.Communication.Handlers;

namespace Qplex.Layers
{
    /// <summary>
    /// Layer object is used for internal(in process only) message passing and handling.
    /// </summary>
    public abstract class Layer : Communicator, ILayer
    {
        #region Constructors
        /// <summary>
        /// Ctor
        /// </summary>
        protected Layer()
        {
            
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagesIteratorType">Messages iterator type</param>
        protected Layer(Type messagesIteratorType)
            : base (messagesIteratorType)
        {
            
        }
        #endregion

        /// <summary>
        /// Layer's init method. Initiate everything the layer needs before starting.
        /// </summary>
        /// <returns>Operation status. True on success, false otherwise.</returns>
        public abstract bool Init();
    }
}
