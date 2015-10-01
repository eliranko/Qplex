using Qplex.Communication.Handlers;

namespace Qplex.Layers
{
    /// <summary>
    /// Layer interface
    /// </summary>
    public interface ILayer : ICommunicator
    {
        /// <summary>
        /// Layer's init method. Initiate everything the layer needs before starting.
        /// </summary>
        /// <returns>Operation status. True on success, false otherwise.</returns>
        bool Init();
    }
}
