using EchoService.Database;

using Messages.Events;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoService.Handlers
{
    public class EchoEventHandler : IHandleMessages<EchoEvent>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        static ILog log = LogManager.GetLogger<EchoEvent>();

        /// <summary>
        /// Saves the echo to the database
        /// </summary>
        /// <param name="message">Information about the echo</param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Handle(EchoEvent message, IMessageHandlerContext context)
        {
            EchoServiceDatabase.getInstance().saveForewardEcho(message);
            return Task.CompletedTask;
        }
    }
}
