using Messages.Commands;
using Messages.DataTypes;
using Messages.DataTypes.Database.CompanyDirectory;
using Messages.DataTypes.Database.Chat;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientApplicationMVC.Models
{
    partial class ServiceBusConnection
    {

        #region AuthenticationServiceMessages
        /// <summary>
        /// Sends the login information to the bus
        /// </summary>
        /// <param name="username">The username entered</param>
        /// <param name="password">The password entered</param>
        /// <returns>The response from the bus</returns>
        public string sendLogIn(string username, string password)
        {
            string message = "authentication/login/" +
                "u=" + username + "&" +
                "p=" + password;
            send(message);
            string response = readUntilEOF();

            return response;
        }

        /// <summary>
        /// Indicates to the service bus that this client wishes to create a new account.
        /// Sends the new account info to the service bus and awaits a response indicating success or failure.
        /// </summary>
        /// <param name="msg">The CreateAccount object containing the new accounts information</param>
        /// <returns>The response from the bus</returns>
        public string sendNewAccountInfo(CreateAccount accountInfo)
        {
            string message = "authentication/createaccount/" + accountInfo.toString();

            send(message);

            return readUntilEOF();
        }
        #endregion AuthenticationServiceMessages

        #region CompanyDirectoryServiceMessages
        /// <summary>
        /// Makes a request to te bus to search for companies matching the given criteria
        /// </summary>
        /// <param name="delim">The criteria to search for</param>
        /// <returns>A list of companies matching the given criteria</returns>
        public CompanyList searchCompanyByName(string delim)
        {
            string message = "companydirectory/companysearch/" + delim;

            send(message);

            return new CompanyList(readUntilEOF());
        }

        /// <summary>
        /// Makes a request to the servicebus to get the information about a specific company
        /// </summary>
        /// <param name="name">The name of the company being searched for</param>
        /// <returns>The information about the company</returns>
        public CompanyInstance getCompanyInfo(string name)
        {
            string message = "companydirectory/getcompany/" + name;

            send(message);

            return new CompanyInstance(readUntilEOF());
        }
        #endregion CompanyDirectoryServiceMessages

        #region ChatServiceMessages
        /// <summary>
        /// Notifies the service bus that a user has sent a message. This function will also
        /// attempt to send the message directly to the receiver,if they have an open session
        /// </summary>
        /// <param name="msg">The message to send</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool sendChatMessage(ChatMessage msg)
        {
            string busmsg = "chat/sendmessage/" + msg.toString();
            send(busmsg);
            return true;
        }

        /// <summary>
        /// Makes a request to the service bus for a list of usernames the current user has contacted via chat in the past
        /// </summary>
        /// <returns>An array of usernames</returns>
        public string[] getAllChatContacts()
        {
            string msg = "chat/getchatcontacts/" + Globals.getUser();

            send(msg);

            string response = readUntilEOF();

            if(" ".Equals(response))
            {
                return new string[0];
            }

            return response.Split('&');
        }

        /// <summary>
        /// Sends a message to the bus requesting the chat history between this user
        /// and the other specified user.
        /// </summary>
        /// <param name="otherUser">The other user whos chat history with the current user is requested</param>
        /// <returns>The chat history between two users</returns>
        public ChatHistory getChatHistory(string otherUser)
        {
            string msg = "chat/getchathistory/" +
                "userone=" + Globals.getUser() +
                "&usertwo=" + otherUser;

            send(msg);

            string response = readUntilEOF();
            return new ChatHistory(response);
        }

        #endregion ChatServiceMessages

        /// <summary>
        /// Closes the connection with the service bus.
        /// </summary>
        public void close()
        {
            connection.Close();
        }

        /// <summary>
        /// Sends the sppecified message through the socket
        /// Attaches the msgEndDelim to the end of the message to indicate the end of the string
        /// </summary>
        /// <param name="message">The message to be sent</param>
        private void send(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message + SharedData.msgEndDelim);

            while (!connection.Connected)
            {
                //TODO: Low Importance - Add a timeout to this
                connect();
            }

            connection.Send(msg);
        }

        /// <summary>
        /// Attempts to connect to the service Bus through the socket
        /// </summary>
        private void connect()
        {
            connection.Connect(ServiceBusInfo.serverHostName, ServiceBusInfo.serverPort);
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the "<EOF>" string of characters is found
        /// </summary>
        /// <returns>The string representation of bytes read from the server socket</returns>
        private string readUntilEOF()
        {
            byte[] readByte = new byte[1];
            string returned = String.Empty;

            while (returned.Contains(SharedData.msgEndDelim) == false)
            {
                connection.Receive(readByte, 1, 0);
                returned += (char)readByte[0];
            }

            return returned.Substring(0, returned.IndexOf(SharedData.msgEndDelim));
        }

        /// <summary>
        /// Disconnects the socket from the server. The socket will be able to reconnect later.
        /// </summary>
        private void terminateConnection()
        {
            connection.Disconnect(true);
        }
    }

    partial class ServiceBusConnection
    {
        /// <summary>
        /// This is the socket that connects the application to the database
        /// </summary>
        private Socket connection = new Socket(ServiceBusInfo.ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Semaphore in charge of making sure only one thread accesses the socket at a time
        /// </summary>
        private Semaphore _lock = new Semaphore(0, 1);
    }
}