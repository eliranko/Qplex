﻿using System.Net;
using Qplex.Networking.Parsers;

namespace Qplex.Messages.Networking
{
    /// <summary>
    /// New connection message
    /// </summary>
    public class NewConnectionMessage : Message
    {
        /// <summary>
        /// Network parser
        /// </summary>
        public IParser Parser { get; }

        /// <summary>
        /// Local ip end point of the connection
        /// </summary>
        public IPEndPoint LocalIpEndPoint { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parser">Parser</param>
        /// <param name="localIpEndPoint">Local end point</param>
        public NewConnectionMessage(IParser parser, IPEndPoint localIpEndPoint)
        {
            Parser = parser;
            LocalIpEndPoint = localIpEndPoint;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"NewConnectionMessage with local end point {LocalIpEndPoint.Address}:{LocalIpEndPoint.Port}";
        }
    }
}
