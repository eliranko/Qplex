﻿using Qplex.Communication.Handlers;
using Qplex.Messages;

namespace Qplex.Networking.Agents
{
    /// <summary>
    /// Network agent. Agent sends and receives messsages over network.
    /// </summary>
    public interface IAgent : ICommunicator
    {
        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        void Send(Message message);
    }
}