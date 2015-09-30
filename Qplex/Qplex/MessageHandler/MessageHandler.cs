using System;

namespace Qplex.MessageHandler
{
    [AttributeUsage()]
    public class MessageHandler : Attribute
    {
        /// <summary>
        /// Handling thread's name
        /// </summary>
        public string Name { get; }
    }
}
