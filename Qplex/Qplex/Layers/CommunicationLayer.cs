using System.Collections.Generic;
using System.Linq;
using NLog;
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
    public abstract class CommunicationLayer<TIterator> : Layer<TIterator> where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Net services list
        /// </summary>
        protected readonly IList<INetService> NetServicesList;

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

            NetServicesList = new List<INetService>();
        }

        /// <summary>
        /// Add service
        /// </summary>
        /// <param name="netService">Network service</param>
        protected void AddService(INetService netService)
        {
            Log(LogLevel.Debug, $"Added service {netService.GetType().Name}");
            netService.SubscribeToChannel(_layerToServicesChannel);
            NetServicesList.Add(netService);
        }

        /// <summary>
        /// Start net service
        /// </summary>
        /// <returns>Operation status</returns>
        public override bool Start()
        {
            Log(LogLevel.Debug, "Starting...");
            return NetServicesList.Aggregate(true, (current, netService) => current & netService.Start()) && base.Start();
        }

        /// <summary>
        /// Stop net service
        /// </summary>
        public override void Stop()
        {
            Log(LogLevel.Debug, "Stopping...");
            base.Stop();
            foreach (var netService in NetServicesList)
            {
                netService.Stop();
            }
        }
    }

    /// <summary>
    /// Communication layer implemented using queue messages iterator
    /// </summary>
    public abstract class CommunicationLayer : CommunicationLayer<QueueMessagesIterator>
    {
        
    }
}
