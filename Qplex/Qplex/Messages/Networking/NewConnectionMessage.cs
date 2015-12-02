using Qplex.Networking.Parsers;

namespace Qplex.Messages.Networking
{
    /// <summary>
    /// New connection message
    /// </summary>
    public class NewConnectionMessage : Message
    {
        /// <summary>
        /// Network parser
        /// </summary>
        public IParser Parser { get; }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="parser">Parser</param>
        public NewConnectionMessage(IParser parser)
        {
            Parser = parser;
        }
    }
}
