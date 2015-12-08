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
    public class Communicator<TIterator> : Broadcaster, ICommunicator
        where TIterator : IMessagesIterator, new()
    {
        private const int InitMessageTimeout = 15000; //TODO: Extract to configuration

        private readonly IDispatcher _dispatcher;
        private readonly List<Message> _waitingList;
        private readonly List<Type> _expectedList;
        private IList<Type> _initMessages;

        /// <summary>
        /// Ctor
        /// </summary>
        public Communicator()
        {
            _dispatcher = new Dispatcher<TIterator>($"{Name}Dispatcher");
            _initMessages = new List<Type>();
            _waitingList = new List<Message>();
            _expectedList = new List<Type>();
        }

        /// <summary>
        /// Start dispatcher threads
        /// </summary>
        /// <returns></returns>
        public virtual bool Start()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(InitMessageTimeout);
                if (_initMessages.Any())
                    throw new TimeoutException("Timeout for initialization message has expired");
            });
            LoadMessageHandlers();

            Log(LogLevel.Trace, $"Starting communicator: {Name}");
            return _dispatcher.Start();
        }

        /// <summary>
        /// Stop dispatcher threads
        /// </summary>
        public virtual void Stop()
        {
            Log(LogLevel.Debug, $"Stopping communicator: {Name}");
            _dispatcher.Stop();
        }

        /// <summary>
        /// New incoming message
        /// </summary>
        /// <param name="message">New received message</param>
        public void NewMessage(Message message)
        {
            var messageType = message.GetType();

            //Remove message from expected list, if expected
            if (_expectedList.Contains(messageType))
                _expectedList.Remove(messageType);

            //Dispatch message if init queue is empty
            if (!_initMessages.Any())
                _dispatcher.Dispatch(message);

            //Add message to wait list, until the init messages are handled
            else if (_initMessages.Any() && !_initMessages.Contains(message.GetType()))
                _waitingList.Add(message);

            //Dispatch init message
            else
            {
                Log(LogLevel.Trace, $"Received initial message of type: {messageType.Name}");
                _initMessages.Remove(messageType);
                _dispatcher.Dispatch(message);

                if (_initMessages.Any()) return;
                //If last init message, dispatch the waiting list
                _waitingList.ForEach(_dispatcher.Dispatch);
                _waitingList.Clear();
            }
        }

        /// <summary>
        /// Notify self the message
        /// </summary>
        /// <param name="message">Message</param>
        public void Notify(Message message)
        {
            Log(LogLevel.Trace, $"Notifing message: {message.Name}");
            NewMessage(message);
        }

        /// <summary>
        /// Delayed notify self the message
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="milliseconds">Delay time</param>
        public void DelayedNotify(Message message, int milliseconds)
        {
            Log(LogLevel.Debug, $"Delayed Notify of {milliseconds} of message:{message.Name}");
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(milliseconds);
                NewMessage(message);
            });
        }

        /// <summary>
        /// Set initial number of message to receiving before starting layer
        /// </summary>
        /// <param name="messages">Messages requeired to initiate the communicator</param>
        public void SetInitMessages(params Type[] messages)
        {
            Log(LogLevel.Debug, $"Setting initial message: {string.Join(",", messages.Select(type => type.Name))}");
            _initMessages = messages.ToList();
        }

        /// <summary>
        /// Handle message if an handler exists, otherwise invoke action with message
        /// </summary>
        public void TunnelMessage(Message message, Action<Message> action)
        {
            Log(LogLevel.Trace, $"Tunneling message: {message.Name}...");
            if(_dispatcher.IsHandled(message.GetType()))
            {
                Log(LogLevel.Trace, $"Handling tunneled message: {message.Name}");
                NewMessage(message);
                return;
            }
            Log(LogLevel.Trace, $"Invoking given action on message: {message.Name}");
            Task.Factory.StartNew(() => action.Invoke(message));
        }

        /// <summary>
        /// Expect a message
        /// </summary>
        /// <typeparam name="TExpectedMessage">Type of message expected</typeparam>
        /// <param name="timeout">Milliseconds until the action is invoked</param>
        /// <param name="errorMessage">The error message to action is invoked with</param>
        /// <param name="timeoutAction">The action to invoke when timeout expires</param>
        public void Expect<TExpectedMessage>(int timeout, Message errorMessage, Action<Message> timeoutAction)
            where TExpectedMessage : Message
        {
            var expectedMessageType = typeof(TExpectedMessage);
            _expectedList.Add(expectedMessageType);

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(timeout);
                if(!_expectedList.Contains(expectedMessageType)) return;

                Log(LogLevel.Trace,
                    $"Timeout for the expected message {expectedMessageType.Name} has expired. Invoking it's handler.");
                timeoutAction.Invoke(errorMessage);
            });
        }

        /// <summary>
        /// Expect a message
        /// </summary>
        /// <typeparam name="TExpectedMessage">Type of message expected</typeparam>
        /// <param name="broadcastMessage"></param>
        /// <param name="timeout">The timeout until the action is invoked</param>
        /// <param name="errorMessage">The error message to action is invoked with</param>
        /// <param name="timeoutAction">The action to invoke incase of failure</param>
        public void BroadcastAndExpect<TExpectedMessage>(Message broadcastMessage, int timeout, Message errorMessage,
            Action<Message> timeoutAction) where TExpectedMessage : Message
        {
            Broadcast(broadcastMessage);
            Expect<TExpectedMessage>(timeout, errorMessage, timeoutAction);
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
        private string GetAttributeThreadName(MemberInfo methodInfo)
        {
            var messageHandler = methodInfo.GetCustomAttributes(typeof (MessageHandler)).First() as MessageHandler;
            if (messageHandler == null)
                StaticQplex.CloseApplication($"Fatal error getting thread handler name of method: {methodInfo.Name}");

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
                StaticQplex.CloseApplication($"MessageHandler {methodInfo.Name} should have 1 parameter!");
            }

            if (parameters.First().ParameterType != typeof (Message)  &&
                !(parameters.First().ParameterType.IsSubclassOf(typeof(Message))))
            {
                StaticQplex.CloseApplication($"MessageHandler {methodInfo.Name} parameter should inherit Message!");
            }

            if (methodInfo.ReturnType != typeof (void))
            {
                StaticQplex.CloseApplication($"MessageHandler {methodInfo.Name} return type should be void!");
            }
        }

        #endregion
    }
}
