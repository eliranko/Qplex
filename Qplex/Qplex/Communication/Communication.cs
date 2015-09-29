using Qplex.Messages;

namespace Qplex.Communication
{
    /// <summary>
    /// Communication singelton
    /// </summary>
    public class Communication
    {
        /// <summary>
        /// Ctor
        /// </summary>
        private Communication()
        {
            
        }

        /// <summary>
        /// Singelton instance
        /// </summary>
        public static Communication Instance = new Communication();

        /// <summary>
        /// Message handler template
        /// </summary>
        /// <param name="message">Message to handle</param>
        public delegate void MessageHandler(Message message);
    }
}
