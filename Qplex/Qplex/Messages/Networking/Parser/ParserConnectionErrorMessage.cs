namespace Qplex.Messages.Networking.Parser
{
    /// <summary>
    /// Message notifying of a connection error
    /// </summary>
    public class ParserConnectionErrorMessage : Message
    {
        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ParserConnectionErrorMessage";
        }
    }
}
