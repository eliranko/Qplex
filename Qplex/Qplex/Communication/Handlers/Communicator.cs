using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Qplex.Attributes;
using Qplex.Messages;
using Qplex.Messages.Handlers;

namespace Qplex.Communication.Handlers
{
    /// <summary>
    /// Communicator broadcasts and receives messages.
    /// </summary>
    /// <typeparam name="TIterator">Message iterator</typeparam>
    public class Communicator<TIterator> : Broadcaster, ICommunicator where TIterator : IMessagesIterator, new()
    {
        /// <summary>
        /// Dispatcher
        /// </summary>
        private readonly Dispatcher<TIterator> _dispatcher;

        /// <summary>
        /// Ctor
        /// </summary>
        public Communicator()
        {
            _dispatcher = new Dispatcher<TIterator>($"{GetType().Name}Dispatcher");
            LoadMessageHandlers();
        }

        /// <summary>
        /// Start dispatcher threads
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            Log(LogLevel.Debug, "Starting...");
            return _dispatcher.Start();
        }

        /// <summary>
        /// Stop dispatcher threads
        /// </summary>
        public virtual void Stop()
        {
            Log(LogLevel.Debug, "Stopping...");
            _dispatcher.Stop();
        }

        /// <summary>
        /// New incoming message
        /// </summary>
        /// <param name="message">New received message</param>
        public void NewMessage(Message message)
        {
            _dispatcher.Dispatch(message);
        }

        /// <summary>
        /// Notify self the message
        /// </summary>
        /// <param name="message">Message</param>
        public void Notify(Message message)
        {
            Log(LogLevel.Debug, $"Notifing message:{message.GetType().Name}");
            Task.Factory.StartNew(() => NewMessage(message));
        }

        /// <summary>
        /// Delayed notify self the message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="milliseconds">Delay time</param>
        public void DelayedNotify(Message message, int milliseconds)
        {
            Log(LogLevel.Debug, $"Delayed Notify of {milliseconds} of message:{message.GetType().Name}");
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(milliseconds);
                NewMessage(message);
            });
        }

        #region Reflection

        /// <summary>
        /// Load message handlers using reflectoin
        /// </summary>
        private void LoadMessageHandlers()
        {
            var methods = GetMethodsDecoratedByAttribute(typeof (MessageHandler));

            //Load handlers in dispatcher
            foreach (var methodInfo in methods)
            {
                ValidateMessageHandlers(methodInfo);
                _dispatcher.AddHandler(
                    GetParameterType(methodInfo), 
                    GetAction(methodInfo), 
                    GetAttributeThreadName(methodInfo));
            }
        }

        /// <summary>
        /// Get methods of the given type, decorated by the given attribute.
        /// </summary>
        /// <param name="attributeType">Decorating attribute</param>
        /// <returns>IEnumerable methods</returns>
        private IEnumerable<MethodInfo> GetMethodsDecoratedByAttribute(Type attributeType)
        {
            return GetType().GetMethods().Where(method => method.GetCustomAttributes(attributeType).Any());
        }

        /// <summary>
        /// Get method delegate
        /// </summary>
        /// <param name="methodInfo">Method</param>
        /// <returns>Method delegate</returns>
        private Action<Message> GetAction(MethodInfo methodInfo)
        {
            return message => methodInfo.Invoke(this, new object[] {message} );
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
        /// Validate a message handler
        /// </summary>
        /// <param name="methodInfo">Method to vaidate</param>
        private void ValidateMessageHandlers(MethodInfo methodInfo)
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

            if (methodInfo.ReturnType != typeof (void))
            {
                Qplex.Instance.CloseApplication($"MessageHandler {methodInfo.Name} return type should be void!");
            }
        }

        #endregion
    }
}
