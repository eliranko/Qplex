using System.Collections.Generic;
using Qplex.Communication.Channels;
using Qplex.Messages.Handlers;
using Qplex.Networking.NetService;

namespace Qplex.Layers
{
    /// <summary>
    /// Communication layer is used for in process messaging,
    /// and processes communication using Message object.
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public abstract class CommunicationLayer<TIterator>: Layer<TIterator> where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Net services list
        /// </summary>
        protected readonly IList<NetService<TIterator>> NetServicesList;

        /// <summary>
        /// Layer to services channel
        /// </summary>
        private readonly InternalChannel _layerToServicesChannel;

        /// <summary>
        /// Ctor
        /// </summary>
        protected CommunicationLayer()
        {
            _layerToServicesChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToServicesChannel");
            SubscribeToChannel(_layerToServicesChannel);

            NetServicesList = new List<NetService<TIterator>>();
        }

        /// <summary>
        /// Start net service
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            var status = base.Start();
            foreach (var netService in NetServicesList)
            {
                status &= netService.Start();
            }

            return status;
        }

        /// <summary>
        /// Stop net service
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            foreach (var netService in NetServicesList)
            {
                netService.Stop();
            }
        }

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="netService">Network service</param>
        public void AddService(NetService<TIterator> netService)
        {
            netService.SubscribeToChannel(_layerToServicesChannel);
            NetServicesList.Add(netService);
        }
    }

    /// <summary>
    /// Communication layer implemented using queue messages iterator
    /// </summary>
    public abstract class CommunicationLayer : CommunicationLayer<QueueMessagesIterator>
    {
        
    }
}
