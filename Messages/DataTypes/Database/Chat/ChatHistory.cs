using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.DataTypes.Database.Chat
{
    /// <summary>
    /// This class represents all messages sent between two users since the accounts creation
    /// </summary>
    public partial class ChatHistory
    {
        public ChatHistory() { }

        /// <summary>
        /// This constructor parses given string representation of the object
        /// The string is assumed to be in the same format that is created by the
        /// user defined toString() function. Note the difference between the 
        /// ToString() method inherited from the object class
        /// </summary>
        /// <param name="info">The string representation of a ChatHistory object</param>
        public ChatHistory(string info)
        {
            string[] properties = info.Split(new char[] { '&' }, 3);
            foreach(string property in properties)
            {
                string[] pieces = property.Split(new char[] { '=' }, 2);
                switch (pieces[0])
                {
                    case ("usersname"):
                        usersname = pieces[1];
                        break;
                    case ("companyname"):
                        companyname = pieces[1];
                        break;
                    case ("messages"):
                        string[] chatMessages = pieces[1].Split('$');
                        messages = new List<ChatMessage>(chatMessages.Length);
                        foreach(string chatMessage in chatMessages)
                        {
                            messages.Add(new ChatMessage(chatMessage));
                        }
                        break;
                    default:
                        throw new ArgumentException("Chat history string is in an invalid format.");
                }
            }
        }

        /// <summary>
        /// Creates a string representation of the current state of the object using a "rest-esque" type formatting
        /// </summary>
        /// <returns>The string representation of the object</returns>
        public string toString()
        {
            //TODO low importance: change all classes defined in Messages using the user defined toString()
            //methods to use built in serialization instead. Better performance (probably?) and increased evolvability
            string returned = "";

            returned += "usersname=" + usersname;
            returned += "&companyname=" + companyname;
            
            if(messages != null && messages.Count > 0)
            {
                returned += "&messages=";
                for(int i = 0; i != messages.Count; i++)
                {
                    if(i != 0)
                    {
                        returned += "$";
                    }
                    returned += messages[i].toString();
                }
            }

            return returned;
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables of the class
    /// </summary>
    public partial class ChatHistory
    {
        /// <summary>
        /// A list of all the messages sent between the two users.
        /// </summary>
        public List<ChatMessage> messages { get; set; } = new List<ChatMessage>(0);

        /// <summary>
        /// The username of the client 
        /// </summary>
        public string usersname { get; set; }

        /// <summary>
        /// The username of the company
        /// </summary>
        public string companyname { get; set; }

        //TODO very low importance: Change the name of usersname and companyname to be more general
        //so that company to company communication (if ever implemented) does not cause these names 
        //to be confusing
    }
}
