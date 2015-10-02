using System;
using System.Collections.Generic;
using Qplex.Communication.Channels;
using Qplex.MessageFactories;
using Qplex.Networking;

namespace Qplex.Layers
{
    /// <summary>
    /// Communication layer is used for in process messaging,
    /// and processes communication using Message object.
    /// </summary>
    public abstract class CommunicationLayer: Layer
    {
        /// <summary>
        /// Net services list
        /// </summary>
        private IList<NetService<Protocol, Listener<IConnection, IMessageFactory>>> _netServicesList;

        /// <summary>
        /// Layer to services channel
        /// </summary>
        private InternalChannel _layerToServicesChannel;

        #region Constructors
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="netServicesList">Net services list</param>
        protected CommunicationLayer(IEnumerable<NetService<Protocol, Listener<IConnection, IMessageFactory>>> netServicesList)
        {
            CreateLayer(netServicesList);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="netService">Net service</param>
        protected CommunicationLayer(NetService<Protocol, Listener<IConnection, IMessageFactory>> netService)
        {
            CreateLayer(netService);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagesIteratorType">Messages iterator type</param>
        /// <param name="netServicesList">Net services list</param>
        protected CommunicationLayer(Type messagesIteratorType, IEnumerable<NetService<Protocol, Listener<IConnection, IMessageFactory>>> netServicesList)
            : base(messagesIteratorType)
        {
            CreateLayer(netServicesList);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagesIteratorType">Messages iterator type</param>
        /// <param name="netService">Net services</param>
        protected CommunicationLayer(Type messagesIteratorType, NetService<Protocol, Listener<IConnection, IMessageFactory>> netService)
            : base(messagesIteratorType)
        {
            CreateLayer(netService);
        }
        #endregion

        #region Constructors helpers
        /// <summary>
        /// Create layer
        /// </summary>
        /// <param name="netService"></param>
        private void CreateLayer(NetService<Protocol, Listener<IConnection, IMessageFactory>> netService)
        {
            CreateChannel();
            AddServiceToList(netService);
        }

        /// <summary>
        /// Create layer
        /// </summary>
        /// <param name="netServicesList">Services list</param>
        private void CreateLayer(IEnumerable<NetService<Protocol, Listener<IConnection, IMessageFactory>>> netServicesList)
        {
            CreateChannel();
            foreach (var netService in netServicesList)
            {
                AddServiceToList(netService);
            }
        }

        /// <summary>
        /// Create channel and subscribe to it
        /// </summary>
        private void CreateChannel()
        {
            _layerToServicesChannel = new InternalChannel(
                $"{GetType().FullName}{GetType().GUID.ToString().Substring(0, 4)}ToServicesChannel");
            SubscribeToChannel(_layerToServicesChannel);
        }

        /// <summary>
        /// Add service to list
        /// </summary>
        /// <param name="netService">Service</param>
        private void AddServiceToList(NetService<Protocol, Listener<IConnection, IMessageFactory>> netService)
        {
            netService.SubscribeToChannel(_layerToServicesChannel);
            if (_netServicesList == null)
            {
                _netServicesList = new List<NetService<Protocol, Listener<IConnection, IMessageFactory>>> {netService};
            }
            else
            {
                _netServicesList.Add(netService);
            }
        }
        #endregion
    }
}
