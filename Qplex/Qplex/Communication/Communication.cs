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
    }
}
