using System;
using NLog;

namespace Qplex
{
    /// <summary>
    /// Global application helper
    /// </summary>
    public static class StaticQplex
    {
        /// <summary>
        /// Logger
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Close applicatoin due to an error
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public static void CloseApplication(string errorMessage)
        {
            Logger.Log(LogLevel.Fatal, errorMessage);
            Environment.Exit(-1);
        }
    }
}
