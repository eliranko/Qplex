using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Qplex.FramingAlgorithms;
using Qplex.Messages;

namespace Qplex.MessageFactories
{
    /// <summary>
    /// Message factory which serializes and deserialize Message object as is
    /// </summary>
    public class MessageObjectMessageFactory : IMessageFactory
    {
        /// <summary>
        /// Serialize message to frame
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>Frame</returns>
        public Frame Serialize(Message message)
        {
            var memoryStream = new MemoryStream();
            var formatter = new BinaryFormatter
            {
                //the assembly used during deserialization need not match exactly the assembly used during serialization
                AssemblyFormat = FormatterAssemblyStyle.Simple
            };
            formatter.Serialize(memoryStream, message);
            return new Frame(memoryStream.ToArray());
            //TODO: Dispose memory stream
        }

        /// <summary>
        /// Deserialize frame to message
        /// </summary>
        /// <param name="frame">Frame</param>
        /// <returns>Message</returns>
        public Message Deserialize(Frame frame)
        {
            var memoryStream = new MemoryStream(frame.Buffer);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var formatter = new BinaryFormatter
            {
                //the assembly used during deserialization need not match exactly the assembly used during serialization
                AssemblyFormat = FormatterAssemblyStyle.Simple,
                Binder = new MessageBinder()
            };
            return (Message) formatter.Deserialize(memoryStream);
            //TODO: Dispose memory stream
        }
    }

    /// <summary>
    /// Message object binder
    /// </summary>
    internal sealed class MessageBinder : SerializationBinder
    {
        /// <summary>
        /// Bind message name to message object in current AppDomain
        /// </summary>
        /// <param name="assemblyName">Assembly name of sender</param>
        /// <param name="typeName">Message type name</param>
        /// <returns></returns>
        public override Type BindToType(string assemblyName, string typeName)
        {
            //Extract message type, disregarding namespaces
            var messageTypeName = typeName.Split('.').Last();

            return (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where type.Name == messageTypeName
                select type).FirstOrDefault();
        }
    }
}
