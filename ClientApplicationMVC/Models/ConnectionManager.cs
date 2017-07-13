using Messages.Commands;
using Messages.Message;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Web;

namespace ClientApplicationMVC.Models
{
    public static partial class ConnectionManager
    {
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

    public static partial class ConnectionManager
    {
        /// <summary>
        /// Contains bus connection for all users who are logged in
        /// </summary>
        private static Dictionary<string, ServiceBusConnection> connections =
            new Dictionary<string, ServiceBusConnection>();
    }
}