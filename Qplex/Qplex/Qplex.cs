using System;

namespace Qplex
{
    /// <summary>
    /// Global application helper
    /// </summary>
    public class Qplex
    {
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
            //TODO: Log
            Environment.Exit(-1);
        }
    }
}
