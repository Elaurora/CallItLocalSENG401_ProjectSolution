using NServiceBus;

using Messages.DataTypes.Database.Chat;

namespace Messages.Commands
{
    public class GetChatHistory : ICommand
    {
        public string userone { get; set; }
        public string usertwo { get; set; }
        public ChatHistory history { get; set; }
    }
}
