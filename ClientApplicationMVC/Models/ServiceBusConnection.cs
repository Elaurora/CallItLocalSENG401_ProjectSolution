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
        public static string sendCredentials(string username, string password)
        {
            //_lock.WaitOne();
            send(username);
            send(password);
            string response = readUntilEOF();

            //TODO: Check the response of the server. If it is negative, close the connection
            //TODO: Get the semaphore working properly

            //_lock.Release();
            return response;
        }


        /// <summary>
        /// Sends the sppecified message through the socket
        /// Attaches "<EOF>" to the end of the message to indicate the end of the string
        /// </summary>
        /// <param name="message">The message to be sent</param>
        private static void send(string message)
        {
            byte[] msg = Encoding.ASCII.GetBytes(message + "<EOF>");

            if (!connection.Connected)
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
        /// Attempts to close the connection to the ServiceBus
        /// </summary>
        private static void close()
        {
            connection.Close();
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the "<EOF>" string of characters is found
        /// </summary>
        /// <returns>The string representation of bytes read from the server socket</returns>
        private static string readUntilEOF()
        {
            byte[] readByte = new byte[1];
            string returned = String.Empty;

            while (returned.Contains("<EOF>") == false)
            {
                connection.Receive(readByte, 1, 0);
                returned += (char)readByte[0];
            }

            return returned.Substring(0, returned.IndexOf("<EOF>"));
        }
    }
}