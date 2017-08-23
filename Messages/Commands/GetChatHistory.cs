using NServiceBus;

using Messages.DataTypes.Database.Chat;

namespace Messages.Commands
{
    /// <summary>
    /// This class represents a request for the chat history between two users
    /// </summary>
    public class GetChatHistory : IMessage
    {
        /// <summary>
        /// The name of the first user
        /// </summary>
        public string userone { get; set; }

        /// <summary>
        /// the name of the second user
        /// </summary>
        public string usertwo { get; set; }

        /// <summary>
        /// The chat history between these two users.
        /// </summary>
        public ChatHistory history { get; set; }
    }
}
