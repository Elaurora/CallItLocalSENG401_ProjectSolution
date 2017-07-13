using NServiceBus;

namespace Messages.Commands
{
    public class SearchForCompany : ICommand
    {
        public string delim { get; set; }
    }
}
