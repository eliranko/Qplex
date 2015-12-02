using System;

namespace Qplex.Messages
{
    /// <summary>
    /// Base object passing between layers
    /// </summary>
    [Serializable]
    public abstract class Message
    {
        /// <summary>
        /// Message's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        protected Message()
        {
            Name = GetType().Name;
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="name">Message's name</param>
        protected Message(string name)
        {
            Name = name;
        }
    }
}
