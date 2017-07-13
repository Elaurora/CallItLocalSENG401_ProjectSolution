using Messages.Commands;
using Messages.DataTypes;
using Messages.Message;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientApplicationMVC.Models
{
    partial class ServiceBusConnection
    {
        /// <summary>
        /// Sends the login information to the bus
        /// </summary>
        /// <param name="username">The username entered</param>
        /// <param name="password">The password entered</param>
        /// <returns>The response from the bus</returns>
        public string sendLogIn(string username, string password)
        {
            //_lock.WaitOne();
            send("authentication");
            send("login");
            send(username);
            send(password);
            string response = readUntilEOF();

            //TODO: Get the semaphore working properly

            //_lock.Release();
            return response;
        }

        /// <summary>
        /// Indicates to the service bus that this client wishes to create a new account.
        /// Sends the new account info to the service bus and awaits a response indicating success or failure.
        /// </summary>
        /// <param name="msg">The CreateAccount object containing the new accounts information</param>
        /// <returns>The response from the bus</returns>
        public string sendNewAccountInfo(CreateAccount msg)
        {
            send("authentication");
            send("createaccount");

            send(msg.toString());

            return readUntilEOF();
        }

        public CompanyList searchCompanyByName(string name)
        {
            send("companydirectory");
            send("companysearch");

            send(name);

            return new CompanyList(readUntilEOF());
        }

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