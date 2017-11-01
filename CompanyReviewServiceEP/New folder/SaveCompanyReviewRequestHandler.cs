using Messages.ServiceBusRequest.CompanyReview.Requests;

using NServiceBus;
using NServiceBus.Logging;

using System;
using System.Threading.Tasks;

namespace CompanyReviewServiceEP.Handlers
{
    public class SaveCompanyReviewRequestHandler : IHandleMessages<SaveCompanyReviewRequest>
    {
        /// <summary>
        /// This is a class provided by NServiceBus. Its main purpose is to be use log.Info() instead of Messages.Debug.consoleMsg().
        /// When log.Info() is called, it will write to the console as well as to a log file managed by NServiceBus
        /// </summary>
        /// It is important that all logger member variables be static, because NServiceBus tutorials warn that GetLogger<>()
        /// is an expensive call, and there is no need to instantiate a new logger every time a handler is created.
        private static ILog log = LogManager.GetLogger<SaveCompanyReviewRequestHandler>();

        public Task Handle(SaveCompanyReviewRequest request, IMessageHandlerContext context)
        {
            throw new NotImplementedException();
        }
    }
}
