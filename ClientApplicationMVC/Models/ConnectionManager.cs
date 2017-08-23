using CompanyDirectoryService.Database;

using Messages.Commands;
using Messages.DataTypes.Database.Chat;
using Messages.DataTypes.Database.CompanyDirectory;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace ClientApplicationMVC.Models
{
    /// <summary>
    /// This class is responsible for maintaining and controlling the ServiceBusConnections for each client with an open session
    /// </summary>
    public static partial class ServiceBusCommunicationManager
    {
        #region AuthenticationServiceMessages
        
        /// <summary>
        /// Sends the login information to the bus
        /// </summary>
        /// <param name="username">The username entered</param>
        /// <param name="password">The password entered</param>
        /// <returns>The response from the bus</returns>
        public static string sendLogIn(string username, string password)
        {
            ServiceBusConnection newConnection = new ServiceBusConnection();
            string response = newConnection.sendLogIn(username, password);

            if ("Success".Equals(response))
            {
                addConnection(username, newConnection);
                Globals.setUser(username);
            }
            else
            {
                newConnection.close();
            }

            return response;
        }

        /// <summary>
        /// Indicates to the service bus that this client wishes to create a new account.
        /// Sends the new account info to the service bus and awaits a response indicating success or failure.
        /// </summary>
        /// <param name="msg">The CreateAccount object containing the new accounts information</param>
        /// <returns>The response from the bus</returns>
        public static string sendNewAccountInfo(CreateAccount msg)
        {
            ServiceBusConnection newConnection = new ServiceBusConnection();
            string response = newConnection.sendNewAccountInfo(msg);

            if ("Success".Equals(response))
            {
                addConnection(msg.username, newConnection);
                Globals.setUser(msg.username);
            }
            else
            {
                newConnection.close();
            }

            return response;
        }
        #endregion AuthenticationServiceMessages

        #region CompanyDirectoryServiceMessages

        /// <summary>
        /// Asks the service bus to search for companies of the given name
        /// </summary>
        /// <param name="name">The name of the company to search for</param>
        /// <returns>A list of company names matching the criteria given</returns>
        public static CompanyList searchCompanyByName(string name)
        {
            ServiceBusConnection connection;
            if (!connections.TryGetValue(Globals.getUser(), out connection)){
                return null;
            }
            return connection.searchCompanyByName(name);
        }

        /// <summary>
        /// Returns the database information regarding the given company name
        /// </summary>
        /// <param name="name">The name of the company</param>
        public static CompanyInstance getCompanyInfo(string name)
        {
            ServiceBusConnection connection;
            if (!connections.TryGetValue(Globals.getUser(), out connection))
            {
                return null;
            }
            return connection.getCompanyInfo(name);
        }

        #endregion CompanyDirectoryServiceMessages

        #region ChatServiceMessages
        /// <summary>
        /// Notifies the service bus that a user has sent a message.
        /// </summary>
        /// <param name="msg">The message to send</param>
        /// <returns>True if successfulm false otherwise</returns>
        public static bool sendChatMessage(ChatMessage msg)
        {
            ServiceBusConnection connection;
            if (!connections.TryGetValue(Globals.getUser(), out connection))
            {
                return false;
            }
            return connection.sendChatMessage(msg);
        }

        /// <summary>
        /// Makes a request to the service bus for a list of usernames the current user has contacted via chat in the past
        /// </summary>
        /// <returns>An array of usernames</returns>
        public static string[] getAllChatContacts()
        {
            ServiceBusConnection connection;
            if (!connections.TryGetValue(Globals.getUser(), out connection))
            {
                return null;
            }
            return connection.getAllChatContacts();
        }

        /// <summary>
        /// Makes a request to the Chat Service to get the chat history between the requesting user and the given user
        /// </summary>
        /// <param name="otherUser">The username of the other user</param>
        /// <returns>The response from the service bus</returns>
        public static ChatHistory getChatHistory(string otherUser)
        {
            ServiceBusConnection connection;
            if (!connections.TryGetValue(Globals.getUser(), out connection))
            {
                return null;
            }
            return connection.getChatHistory(otherUser);
        }

        #endregion ChatServiceMessages

        /// <summary>
        /// Removes the connection of the given user from the list of connections
        /// </summary>
        /// <param name="user">The connection key to terminate</param>
        public static void removeConnection(string user)
        {
            connections.Remove(user);
        }

        /// <summary>
        /// Adds the given connection to the list of connection with the given string as a key
        /// </summary>
        /// <param name="user">The identifier for the connection</param>
        /// <param name="connection">The ServiceBusConnection to add to the list</param>
        private static void addConnection(string user, ServiceBusConnection connection)
        {
            connections[user] = connection;
        }
    }

    /// <summary>
    /// This portion of the class contains member variables
    /// </summary>
    public static partial class ServiceBusCommunicationManager
    {
        /// <summary>
        /// Contains bus connection for all users who are logged in
        /// </summary>
        private static Dictionary<string, ServiceBusConnection> connections =
            new Dictionary<string, ServiceBusConnection>();
    }
}