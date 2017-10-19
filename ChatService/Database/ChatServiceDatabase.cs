using Messages.Database;
using Messages.DataTypes.Database.Chat;
using Messages.DataTypes;
using Messages.NServiceBus.Commands;
using Messages.ServiceBusRequest.Chat.Responses;
using Messages.ServiceBusRequest.Chat.Requests;

using MySql.Data.MySqlClient;

using System;
using System.Data.Sql;
using System.Collections.Generic;

namespace ChatService.Database
{
    /// <summary>
    /// This class is used to manipulate and read the Chat Service's database in a safe and consistent manner.
    /// It follows the singleton design pattern, as only one instance of this class should ever be in existance at any given time.
    /// </summary>
    public partial class ChatServiceDatabase : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private ChatServiceDatabase() { }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
        public static ChatServiceDatabase getInstance()
        {
            //TODO low importance: Add a semaphore to this and the other databases.
            if(instance == null)
            {
                instance = new ChatServiceDatabase();
            }
            return instance;
        }

        /// <summary>
        /// Saves the chat message to the database and makes all necessary inserts
        /// </summary>
        /// <param name="msg">The message to save</param>
        public void saveMessage(ChatMessage msg)
        {
            
            if (openConnection() == true)
            {
                string query = @"SELECT id FROM " + databaseName + @".chats " +
                    @"WHERE (usersname='" + msg.sender + @"' AND companyname='" + msg.receiver + @"') " +
                    @"OR (usersname='" + msg.receiver + @"' AND companyname='" + msg.sender + @"');";

                //TODO Low importance, figure out what MySqlTransaction is and how it do. Also, something along the lines of "TransactionScope"
                MySqlCommand command = new MySqlCommand(query, connection);
                
                //TODO medium importance: Look into the classes and interfaces in the namespace below.
                //System.Data.SqlClient.
                MySqlDataReader dataReader = command.ExecuteReader();
                long id = -1;

                if(dataReader.Read() == true)
                {
                    id = dataReader.GetInt64("id");
                    dataReader.Close();
                }
                else // The chat instance does not exist and needs to be created
                {
                    dataReader.Close();
                    createNewChatInstance(msg.sender, msg.receiver);

                    query = @"SELECT id FROM " + databaseName + @".chats " +
                        @"WHERE usersname='" + msg.sender + @"' AND companyname='" + msg.receiver + @"';";

                    command = new MySqlCommand(query, connection);

                    dataReader = command.ExecuteReader();
                    dataReader.Read();
                    id = dataReader.GetInt64("id");
                    dataReader.Close();
                }

                query = @"INSERT INTO messages(id, message, timestamp, sender) " +
                    @"VALUES('" + id.ToString() + @"', '" + msg.messageContents +
                    @"', '" + msg.unix_timestamp.ToString() + @"', '" + msg.sender + @"');";

                command = new MySqlCommand(query, connection);
                command.ExecuteNonQuery();
                closeConnection();
            }
            else
            {
                throw new Exception("Could not connect to database.");
            }
        }

        /// <summary>
        /// Creates a new chat row in the chats table using the given parameters
        /// </summary>
        /// <param name="usersname">the usersname</param>
        /// <param name="companyname">the companies name</param>
        private void createNewChatInstance(string usersname, string companyname)
        {
            bool wasClosed = false;
            if(connection.State == System.Data.ConnectionState.Closed)
            {
                if(openConnection() == false)
                {
                    throw new Exception("Could not confirm connection to database in helper function.");
                }
                wasClosed = true;
            }

            string query = @"INSERT INTO chats(usersname, companyname) " +
                @"VALUES('" + usersname + @"', '" + companyname + @"');";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            if(wasClosed == true)
            {
                closeConnection();
            }
        }

        /// <summary>
        /// Selects the names of all other users that the given user has made chat contact with in the past
        /// </summary>
        /// <param name="usersname">The name of the user</param>
        /// <returns>A list of usernames the user has sent at least one chat message to</returns>
        public GetChatContactsResponse getAllChatContactsForUser(GetChatContactsRequest request)
        {
            //TODO low importance: Turn this from max 3 queries to 2 for added efficiency
            bool result = false;
            string response = "";

            GetChatContacts requestData = request.getCommand;

            if(openConnection() == true)
            {
                List<string> contacts = new List<string>();
                MySqlDataReader reader = null;
                try
                {
                    string query = "SELECT * FROM " + databaseName + ".chats " +
                       "WHERE usersname='" + requestData.usersname + "' OR companyname='" + requestData.usersname + "';";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read() == true)
                    {
                        if (requestData.usersname.Equals(reader.GetString("usersname")))
                        {
                            contacts.Add(reader.GetString("companyname"));
                        }
                        else
                        {
                            contacts.Add(reader.GetString("usersname"));
                        }
                    }

                    requestData.contactNames = contacts;
                    result = true;
                }
                catch (Exception e)
                {
                    response = e.Message;
                }
                finally
                {
                    if(reader != null && reader.IsClosed == false)
                    {
                        reader.Close();
                    }
                    closeConnection();
                }

                return new GetChatContactsResponse(result, response, requestData);
            }

            return new GetChatContactsResponse(false, "Could not connect to database", requestData);
            
        }

        /// <summary>
        /// This function will search the chat database for all of the chat messages passed between the 2 specified users.
        /// </summary>
        /// <param name="usersname">The name of the user</param>
        /// <param name="companyname">The name of the company</param>
        /// <returns>The chat history of the two users</returns>
        public GetChatHistoryResponse getChatHistory(GetChatHistoryRequest request)
        {
            GetChatHistory requestData = request.getCommand;
            string user1 = requestData.history.user1;
            string user2 = requestData.history.user2;
            List<ChatMessage> messages = new List<ChatMessage>();
            bool result = false;
            string response = "";

            if(openConnection() == true)
            {
                MySqlDataReader reader = null;

                try
                {
                    string query = "SELECT m.message, m.timestamp, m.sender " +
                        "FROM " + databaseName + ".messages AS m LEFT JOIN " +
                        databaseName + ".chats AS c ON m.id = c.id " +
                        "WHERE (c.usersname='" + user1 + "' AND c.companyname='" + user2 + "') " +
                        "OR (c.usersname='" + user2 + "' AND c.companyname='" + user1 + "');";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    reader = command.ExecuteReader();

                    while (reader.Read() == true)
                    {
                        ChatMessage msg = new ChatMessage
                        {
                            messageContents = reader.GetString("message"),
                            unix_timestamp = reader.GetInt32("timestamp")
                        };
                        string sender = reader.GetString("sender");
                        msg.sender = sender;
                        if (user1.Equals(sender))
                        {
                            msg.receiver = user2;
                        }
                        else
                        {
                            msg.receiver = user1;
                        }
                        messages.Add(msg);
                    }

                    requestData.history.messages = messages;
                    result = true;
                }
                catch(Exception e)
                {
                    response = e.Message;
                }
                finally
                {
                    if (reader != null && reader.IsClosed == false)
                    {
                        reader.Close();
                    }
                    closeConnection();
                }
                return new GetChatHistoryResponse(result, response, requestData);
            }
            else
            {
                return new GetChatHistoryResponse(false, "Could not connect to Database.", requestData);
            }
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables as well as the schema definition in the form of Table/Column objects
    /// </summary>
    public partial class ChatServiceDatabase : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "chatservicedb";
        public override String databaseName { get; } = dbname;

        /// <summary>
        /// The singleton instance of the database
        /// </summary>
        protected static ChatServiceDatabase instance = null;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "chats",
                    new Column[]
                    {
                        new Column
                        (
                            "id", "INT(64)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE",
                                "AUTO_INCREMENT"
                            }, true
                        ),
                        new Column
                        (
                            "usersname", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "companyname", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        )
                    }
                ),
            new Table
                (
                    dbname,
                    "messages",
                    new Column[]
                    {
                        new Column
                        (
                            "id", "INT(64)",
                            new string[]
                            {
                                "NOT NULL",
                            }, true
                        ),
                        new Column
                        (
                            "message", "VARCHAR(" + SharedData.MAX_MESSAGE_LENGTH.ToString() + ")",
                            new string[]
                            {
                                "NOT NULL",
                            }, false
                        ),
                        new Column
                        (
                            "timestamp", "INT(32)",
                            new string[]
                            {
                                "NOT NULL",
                            }, true
                        ),
                        new Column
                        (
                            "sender", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                            }, false
                        )
                    }
                )
        };
    }
}
