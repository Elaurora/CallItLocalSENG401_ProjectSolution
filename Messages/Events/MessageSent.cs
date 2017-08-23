using Messages.DataTypes.Database.Chat;

using NServiceBus;

namespace Messages.Events
{
    /// <summary>
    /// Wrapper class for chat message to be published as an event. 
    /// Yes, i know i could have just had ChatMessage implement IEvent, however 
    /// this keeps all events more organized and increases changeability
    /// </summary>
    public class MessageSent : IEvent
    {
        public ChatMessage msg { get; set; }
    }
}
