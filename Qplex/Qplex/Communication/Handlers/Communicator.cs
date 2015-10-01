using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Qplex.Attributes;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Communicator broadcasts and receives messages.
    /// </summary>
    public class Communicator : Broadcaster, ICommunicator
    {
        /// <summary>
        /// Dispatcher
        /// </summary>
        private readonly Dispatcher _dispatcher;

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="messagesIteratorType">Messages iterator type</param>
        public Communicator(Type messagesIteratorType)
        {
            if (!messagesIteratorType.GetInterfaces().Contains(typeof(IMessagesIterator)))
            {
                Qplex.Instance.CloseApplication(
                    $"Message iterator type received does not impelements IMessagesIterator interface: {messagesIteratorType.FullName}");
            }

            _dispatcher = new Dispatcher(messagesIteratorType);
        }

        /// <summary>
        /// Load message handlers using reflection, and start dispatcher threads
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            //TODO: Log
            LoadMessageHandlers();
            return _dispatcher.Start();
        }

        /// <summary>
        /// Stop dispatcher threads
        /// </summary>
        public void Stop()
        {
            _dispatcher.Stop();
        }

        /// <summary>
        /// New incoming message
        /// </summary>
        /// <param name="message">New received message</param>
        public void NewMessage(Message message)
        {
            //TODO: Log
            _dispatcher.Dispatch(message);
        }

        #region Reflection

        /// <summary>
        /// Load message handlers using reflectoin
        /// </summary>
        private void LoadMessageHandlers()
        {
            //TODO: Log
            var methods = GetMethodsDecoratedByAttribute(GetTopDerivedClass(), typeof (MessageHandler));
            ValidateMessageHandlers(methods);

            //Load handlers in dispatcher
            foreach (var methodInfo in methods)
            {
                _dispatcher.AddHandler(GetParameterType(methodInfo), 
                    GetDelegate(methodInfo), GetAttributeThreadName(methodInfo));
            }
        }

        /// <summary>
        /// Get the top derived class
        /// </summary>
        /// <returns>Top derived class</returns>
        private Type GetTopDerivedClass()
        {
            Type topClass = null;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var derivedClass = from type in assembly.GetTypes()
                    where type.GetInterfaces().Contains(typeof (ICommunicator)) && type.GUID == GetType().GUID
                    select type;

                topClass = derivedClass.FirstOrDefault();

                if(topClass != null) break;
            }

            return topClass;
        }

        /// <summary>
        /// Get methods of the given type, decorated by the given attribute.
        /// </summary>
        /// <param name="objectType">Object type</param>
        /// <param name="attributeType">Decorating attribute</param>
        /// <returns></returns>
        private IEnumerable<MethodInfo> GetMethodsDecoratedByAttribute(Type objectType, Type attributeType)
        {
            return objectType.GetMethods().Where(method => method.GetCustomAttributes(attributeType).Any());
        }

        /// <summary>
        /// Get method delegate
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>Method delegate</returns>
        private Delegate GetDelegate(MethodInfo methodInfo)
        {
            return methodInfo.CreateDelegate(Expression.GetDelegateType(
                    (from parameter in methodInfo.GetParameters() select parameter.ParameterType)
                    .Concat(new[] { methodInfo.ReturnType })
                    .ToArray()), this);
        }

        /// <summary>
        /// Get method's parameter type
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>Parameter type</returns>
        private Type GetParameterType(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().First().ParameterType;
        }

        /// <summary>
        /// Get the thread's name that handles the method's parameter's type
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>Thread name</returns>
        private string GetAttributeThreadName(MethodInfo methodInfo)
        {
// ReSharper disable once PossibleNullReferenceException
            return (methodInfo.GetCustomAttributes(typeof(MessageHandler)).First() as MessageHandler).Name;
        }

        /// <summary>
        /// Validate the message handlers
        /// </summary>
        /// <param name="methodInfos">Methods to vaidate</param>
        private void ValidateMessageHandlers(IEnumerable<MethodInfo> methodInfos)
        {
            foreach (var methodInfo in methodInfos)
            {
                var parameters = methodInfo.GetParameters();

                if (parameters.Count() > 1 || !parameters.Any())
                {
                    Qplex.Instance.CloseApplication($"MessageHandler {methodInfo.Name} should have 1 parameter!");
                }

                if (parameters.First().ParameterType != typeof (Message)  &&
                    !(parameters.First().ParameterType.IsSubclassOf(typeof(Message))))
                {
                    Qplex.Instance.CloseApplication($"MessageHandler {methodInfo.Name} parameter should inherit Message!");
                }
            }
        }

        #endregion
    }
}
