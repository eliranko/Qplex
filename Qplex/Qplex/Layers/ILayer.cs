namespace Qplex.Layers
{
    /// <summary>
    /// Layer interface
    /// </summary>
    public interface ILayer
    {
        /// <summary>
        /// Layer's init method
        /// </summary>
        /// <returns>Operation status. True on success, false otherwise.</returns>
        bool Init();

        /// <summary>
        /// Layer's start method
        /// </summary>
        /// <returns>Operation status. True on success, false otherwise.</returns>
        bool Start();
    }
}
