using NLog;

namespace Qplex
{
    /// <summary>
    /// Log class wrapper
    /// </summary>
    public class LogWrapper
    {
        /// <summary>
        /// NLog logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        /// <summary>
        /// Log
        /// </summary>
        /// <param name="logLevel">Log level</param>
        /// <param name="message">Message to log</param>
        public void Log(LogLevel logLevel, string message)
        {
            Logger.Log(logLevel, message);
        }
    }
}
