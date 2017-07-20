using System;

namespace Messages.DataTypes.Database.Chat
{
    public partial class ChatMessage
    {
        public ChatMessage() { }

        /// <summary>
        /// creates a chat message object from a string 
        /// The string is assumed to be in the format generated
        /// by the toString function defined beflow
        /// </summary>
        /// <param name="info">The string representation of the object</param>
        public ChatMessage(string info)
        {
            string[] parts = info.Split('&');
            foreach(string part in parts)
            {
                string[] pieces = part.Split('=');
                switch (pieces[0])
                {
                    case ("sender"):
                        sender = pieces[1];
                        break;
                    case ("receiver"):
                        receiver = pieces[1];
                        break;
                    case ("timestamp"):
                        unix_timestamp = Int32.Parse(pieces[1]);
                        break;
                    case ("content"):
                        messageContents = pieces[1];
                        break;
                    default:
                        throw new ArgumentException("Error: Could not parse chat message, invalid format");
                }
            }
        }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        /// <returns>The string representation</returns>
        public string toString()
        {
            string returned = "";

            if (!"".Equals(sender))
            {
                returned += "sender=" + sender;
            }
            if (!"".Equals(receiver))
            {
                returned += (!"".Equals(returned)) ? "&" : "";
                returned += "receiver=" + receiver;
            }
            if(unix_timestamp != -1)
            {
                returned += (!"".Equals(returned)) ? "&" : "";
                returned += "timestamp=" + unix_timestamp.ToString();
            }
            if (!"".Equals(messageContents))
            {
                returned += (!"".Equals(returned)) ? "&" : "";
                returned += "content=" + messageContents;
            }
            return returned;
        }
    }

    public partial class ChatMessage
    {
        public string sender { get; set; }
        public string receiver { get; set; }
        public int unix_timestamp { get; set; } = -1;
        public string messageContents { get; set; }
    }
}
