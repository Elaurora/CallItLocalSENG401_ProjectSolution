using Messages.Commands;
using Messages.DataTypes;

using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ClientApplicationMVC.Models
{
    public static class ServiceBusConnection
    {
        /// <summary>
        /// This is the socket that connects the application to the database
        /// </summary>
        private static Socket connection = new Socket(ServiceBusInfo.ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// Semaphore in charge of making sure only one thread accesses the socket at a time
        /// </summary>
        private static Semaphore _lock = new Semaphore(0, 1);

        /// <summary>
        /// Sends the 
        /// </summary>
        /// <param name="username">The username entered</param>
        /// <param name="password">The password entered</param>
        /// <returns></returns>
        public static string sendLogIn(string username, string password)
        {
            //_lock.WaitOne();
            send("login");
            send(username);
            send(password);
            string response = readUntilEOF();

            if ("Failure".Equals(response))
            {
                terminateConnection();
            }

            //TODO: Get the semaphore working properly

            //_lock.Release();
            return response;
        }

        public static string sendNewAccountInfo(CreateAccount msg)
        {
            send("createaccount");

            send(msg.toString());

            return readUntilEOF();
        }

        /// <summary>
        /// Sends the sppecified message through the socket
        /// Attaches the msgEndDelim to the end of the message to indicate the end of the string
        /// </summary>
        /// <param name="message">The message to be sent</param>
        private static void send(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message + SharedData.msgEndDelim);

            while (!connection.Connected)
            {
                connect();
            }

            connection.Send(msg);
        }

        /// <summary>
        /// Attempts to connect to the service Bus through the socket
        /// </summary>
        private static void connect()
        {
            connection.Connect(ServiceBusInfo.serverHostName, ServiceBusInfo.serverPort);
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the "<EOF>" string of characters is found
        /// </summary>
        /// <returns>The string representation of bytes read from the server socket</returns>
        private static string readUntilEOF()
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
        private static void terminateConnection()
        {
            connection.Disconnect(true);
        }
    }
}