using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.DataTypes.Database.Chat
{
    public partial class ChatHistory
    {
        public ChatHistory() { }

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

        public string toString()
        {
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

    public partial class ChatHistory
    {
        public List<ChatMessage> messages { get; set; }
        public string usersname { get; set; }
        public string companyname { get; set; }
    }
}
