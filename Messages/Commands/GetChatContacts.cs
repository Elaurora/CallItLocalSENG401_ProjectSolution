using NServiceBus;

using System.Collections.Generic;

namespace Messages.Commands
{
    public partial class GetChatContacts : IMessage
    {
        public string usersname { get; set; }
        public List<string> contactNames { get; set; } = null;
    }
}
