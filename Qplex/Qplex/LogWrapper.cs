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
        public Logger Logger { get; set; }

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
