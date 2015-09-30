using System;

namespace Qplex.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MessageHandler : Attribute
    {
        /// <summary>
        /// Handling thread's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Handling thread's name</param>
        public MessageHandler(string name = "")
        {
            Name = name;
        }
    }
}
