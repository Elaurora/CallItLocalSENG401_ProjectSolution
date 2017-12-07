using System;

namespace Messages.DataTypes.Database.Chat
{
    /// <summary>
    /// This class represents a single chat message sent from one user to another.
    /// </summary>
    [Serializable]
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

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    public partial class ChatMessage
    {
        /// <summary>
        /// The username of the person who sent the message
        /// </summary>
        public string sender { get; set; }

        /// <summary>
        /// The username of the person who the message was sent to
        /// </summary>
        public string receiver { get; set; }

        /// <summary>
        /// A unix timestamp representing when the message was sent, recorded on the clients machine at the time of sending
        /// </summary>
        public int unix_timestamp { get; set; } = -1;

        /// <summary>
        /// The content of the message
        /// </summary>
        public string messageContents { get; set; }
    }
}
