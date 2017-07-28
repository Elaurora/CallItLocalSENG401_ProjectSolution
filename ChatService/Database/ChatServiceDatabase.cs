using Messages.Database;
using Messages.DataTypes.Database.Chat;
using Messages.DataTypes;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;

namespace ChatService.Database
{
    public partial class ChatServiceDatabase : AbstractDatabase
    {
        private ChatServiceDatabase() { }

        public static ChatServiceDatabase getInstance()
        {
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
            if(openConnection() == true)
            {
                string query = @"SELECT id FROM " + databaseName + @".chats " +
                    @"WHERE (usersname='" + msg.sender + @"' AND companyname='" + msg.receiver + @"') " +
                    @"OR (usersname='" + msg.receiver + @"' AND companyname='" + msg.sender + @"');";

                MySqlCommand command = new MySqlCommand(query, connection);
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
        public List<string> getAllChatContactsForUser(string usersname)
        {
            //TODO low importance: Turn this from max 3 queries to 2
            if(openConnection() == true)
            {
                string query = "SELECT * FROM " + databaseName + ".chats " +
                    "WHERE usersname='" + usersname + "' OR companyname='" + usersname + "';";

                List<string> result = new List<string>();

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read() == true)
                {
                    if (usersname.Equals(reader.GetString("usersname")))
                    {
                        result.Add(reader.GetString("companyname"));
                    }
                    else
                    {
                        result.Add(reader.GetString("usersname"));
                    }
                }

                reader.Close();

                closeConnection();

                return result;
            }
            else
            {
                throw new Exception("Could not connect to database.");
            }
        }

        /// <summary>
        /// This function will search the chat database for all of the chat messages passed between the 2 specified users.
        /// </summary>
        /// <param name="usersname">The name of the user</param>
        /// <param name="companyname">The name of the company</param>
        /// <returns>The chat history of the two users</returns>
        public ChatHistory getChatHistory(string usersname, string companyname)
        {
            if(openConnection() == true)
            {
                string query = "SELECT m.message, m.timestamp, m.sender " +
                    "FROM " + databaseName + ".messages AS m LEFT JOIN " +
                    databaseName + ".chats AS c ON m.id = c.id " +
                    "WHERE (c.usersname='" + usersname + "' AND c.companyname='" + companyname + "') " +
                    "OR (c.usersname='" + companyname + "' AND c.companyname='" + usersname + "');";

                ChatHistory result = new ChatHistory()
                {
                    usersname = usersname,
                    companyname = companyname,
                };

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                while(reader.Read() == true)
                {
                    ChatMessage msg = new ChatMessage
                    {
                        messageContents = reader.GetString("message"),
                        unix_timestamp = reader.GetInt32("timestamp")
                    };
                    string sender = reader.GetString("sender");
                    if (usersname.Equals(sender))
                    {
                        msg.sender = usersname;
                    }
                    else
                    {
                        msg.receiver = companyname;
                    }
                    result.messages.Add(msg);
                }

                reader.Close();
                closeConnection();

                return result;
            }
            else
            {
                throw new Exception("Could not connect to database.");
            }
        }
    }

    public partial class ChatServiceDatabase : AbstractDatabase
    {
        private const String dbname = "chatservicedb";
        public override String databaseName { get; } = dbname;

        protected static ChatServiceDatabase instance;

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
