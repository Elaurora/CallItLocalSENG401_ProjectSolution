using Messages.DataTypes.Database.Chat;

using NServiceBus;

namespace Messages.Events
{
    public class MessageSent : IEvent
    {
        public ChatMessage msg { get; set; }
    }
}
