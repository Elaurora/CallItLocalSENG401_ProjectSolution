using Messages.Commands;
using Messages.DataTypes;
using Messages.DataTypes.Database.CompanyDirectory;
using Messages.DataTypes.Database.Chat;

using System;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace ClientApplicationMVC.Models
{
    /// <summary>
    /// This class is responsible for sending and receiving messages between the service bus and the web server in a secure manner.
    /// </summary>
    partial class ServiceBusConnection
    {
        public ServiceBusConnection()
        {
            //connection.ReceiveTimeout = readTimeout_ms;
        }

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
            //TODO medium importance USE SignalR TO SEND TO RECEIVING CLIENT HERE
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

        #region EchoServiceMessages

        /// <summary>
        /// Sends the data to be echo'd to the service bus
        /// </summary>
        /// <param name="data">The data to be echo'd</param>
        /// <returns>The response from the servicebus</returns>
        public string echoForeward(string data)
        {
            string msg = "echo/echo/" + data;

            send(msg);

            string response = readUntilEOF();
            return response;
        }

        /// <summary>
        /// Sends the data to be echo'd to the service bus
        /// </summary>
        /// <param name="data">The data to be echo'd</param>
        /// <returns>The response from the servicebus</returns>
        public string echoReverse(string data)
        {
            string msg = "echo/reverse/" + data;

            send(msg);

            string response = readUntilEOF();
            return response;
        }

        #endregion EchoServiceMessages


        /// <summary>
        /// Closes the connection with the service bus.
        /// </summary>
        public void close()
        {
            connectionStream.Close();//TODO low importance: Make sure this is a valid way of closing the stream
            connection.Close();
        }

        /// <summary>
        /// Sends the sppecified message through the socket
        /// Attaches the msgEndDelim to the end of the message to indicate the end of the string
        /// </summary>
        /// <param name="message">The message to be sent</param>
        private void send(string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message + SharedData.msgEndDelim);

            while (!connection.Connected)
            {
                //TODO: Low Importance - Add a timeout to this
                connect();
            }
            connectionStream.Write(msg);
            connectionStream.Flush();
        }

        /// <summary>
        /// Attempts to connect to the service Bus through the socket, 
        /// then attempts to open an SslStream using the socket,
        /// then attempts to validate the connection with the server
        /// </summary>
        private void connect()
        {
            connection.Connect(ServiceBusInfo.serverHostName, ServiceBusInfo.serverPort);

            connectionStream = new SslStream(
                new NetworkStream(connection),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null);

            connectionStream.AuthenticateAsClient("localhost");
        }

        /// <summary>
        /// Continuously reads one byte at a time from the client until the end of file string of characters defined in the Messages library is found
        /// </summary>
        /// <returns>The string representation of bytes read from the server socket</returns>
        private string readUntilEOF()
        {
            byte[] encodedBytes = new byte[2048];
            string returned = String.Empty;

            while (returned.Contains(SharedData.msgEndDelim) == false)
            {
                try
                {
                    //connection.Receive(encodedBytes, 1, 0);

                    int bytesRead = connectionStream.ReadAsync(encodedBytes, 0, encodedBytes.Length).ConfigureAwait(false).GetAwaiter().GetResult();
                    //int bytesRead = connectionStream.Read(encodedBytes, 0, encodedBytes.Length);

                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] decodedBytes = new char[decoder.GetCharCount(encodedBytes, 0, bytesRead)];

                    decoder.GetChars(encodedBytes, 0, bytesRead, decodedBytes, 0);

                    returned += new string(decodedBytes);
                }
                catch (SocketException)// This is thrown when the timeout occurs. The timeout is set in the constructor
                {
                    Thread.Yield();// Yield this threads remaining timeslice to another process, this process does not appear to need it
                }
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

        /// <summary>
        /// TODO AMIR Medium Importance: Please write an accurate description of this method, its parameters and response
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool ValidateServerCertificate(
            object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            //TODO: low importance - Handle a failed validation

            return false;
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables
    /// </summary>
    partial class ServiceBusConnection
    {
        /// <summary>
        /// This is the socket that connects the application to the database
        /// </summary>
        private Socket connection = new Socket(ServiceBusInfo.ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        /// <summary>
        /// The stream used to communicate through securely
        /// </summary>
        private SslStream connectionStream;

        /// <summary>
        /// Semaphore in charge of making sure only one thread accesses the socket at a time
        /// </summary>
        private Semaphore _lock = new Semaphore(0, 1);


        /// <summary>
        /// The number of milliseconds the ServiceBusConnection should wait for a response from the server before yielding its remaining timeslice
        /// </summary>
        private const int readTimeout_ms = 50;
    }
}