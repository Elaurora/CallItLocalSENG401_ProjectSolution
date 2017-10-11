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
            ServiceBusConnection newConnection = new ServiceBusConnection(username);
            string response = newConnection.sendLogIn(password);

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
            ServiceBusConnection newConnection = new ServiceBusConnection(msg.username);
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

        /// <summary>
        /// Returns the ServiceBusConnection object associates with the given user.
        /// </summary>
        /// <param name="user">The name of the user to get the connection object for</param>
        /// <returns>The connection object if the user has been properly authenticated recently. null otherwise</returns>
        public static ServiceBusConnection getConnectionObject(string user)
        {
            ServiceBusConnection connection;
            if (connections.TryGetValue(user, out connection) == false)
            {
                return null;
            }

            if(connection.isConnected() == false)
            {
                connection.close();
                connections.Remove(user);
                return null;
            }

            return connection;
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