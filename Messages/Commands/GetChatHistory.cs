using NServiceBus;

using Messages.DataTypes.Database.Chat;

namespace Messages.Commands
{
    public class GetChatHistory : IMessage
    {
        public string userone { get; set; }
        public string usertwo { get; set; }
        public ChatHistory history { get; set; }
    }
}
