using Qplex.Communication.Handlers;

namespace Qplex.Layers
{
    /// <summary>
    /// Layer object is used for internal(in process only) message passing and handling.
    /// </summary>
    public abstract class Layer : Communicator ,ILayer
    {
        public abstract bool Init();

        public override bool Start()
        {
            //TODO: Log
            var status = base.Start();
            return status;
        }
    }
}
