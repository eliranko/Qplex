using System;
using NLog;

namespace Qplex
{
    /// <summary>
    /// Global application helper
    /// </summary>
    public class Qplex
    {
        /// <summary>
        /// Logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Ctor
        /// </summary>
        private Qplex()
        {
        }

        /// <summary>
        /// Instance
        /// </summary>
        public static Qplex Instance = new Qplex();

        /// <summary>
        /// Close applicatoin due to an error
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        public void CloseApplication(string errorMessage)
        {
            Logger.Log(LogLevel.Fatal, errorMessage);
            Environment.Exit(-1);
        }
    }
}
