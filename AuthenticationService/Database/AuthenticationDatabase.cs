using Messages.Database;
using Messages.Commands;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;

namespace AuthenticationService.Database
{
    public partial class AuthenticationDatabase : AbstractDatabase
    {
        private AuthenticationDatabase()
            : base()
        {
        }

        /// <summary>
        /// Returns the singleton instance of the CompanyDirectoryDB
        /// If the instance had not yet been created it will be upon calling
        /// this function
        /// </summary>
        /// <returns></returns>
        public static AuthenticationDatabase getInstance()
        {
            if (instance == null)
            {
                instance = new AuthenticationDatabase();
            }
            return instance;
        }

        /// <summary>
        /// Attempts to insert a new user account into the database
        /// </summary>
        /// <param name="accountInfo">Contains information about the </param>
        /// <returns>true if successful, false otherwise</returns>
        public bool insertNewUserAccount(CreateAccount accountInfo)
        {
            if(openConnection() == true)
            {
                string query = @"INSERT INTO user(username, password, address, phonenumber, type) " +
                    @"VALUES('" + accountInfo.username + @"', '" + accountInfo.password + 
                    @"', '" + accountInfo.address + @"', '" + accountInfo.phonenumber + @"', '" +
                    accountInfo.type.ToString() + @"');";

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch(MySqlException e)
                {
                    Console.WriteLine("Unable to complete insert new user into database." +
                        " Error :" + e.Number + e.Message);
                    Console.WriteLine("The query was:" + query);
                    closeConnection();
                    return false;
                }
                finally
                {
                    closeConnection();
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// This function is used to check and see if the given username and password correspond
        /// to an existing user account.
        /// </summary>
        /// <param name="username">The username to check the database for</param>
        /// <param name="password">The password to check the database for</param>
        /// <returns>True if the info corresponds to an entry in the database, false otherwise</returns>
        public bool isValidUserInfo(string username, string password)
        {
            string query = "SELECT * FROM user" +
                "WHERE username = " + username + " " +
                "AND password = " + password + ";";

            bool returned = false;

            if(openConnection() == true)
            {
                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = command.ExecuteReader();

                returned = dataReader.Read();

                dataReader.Close();

                closeConnection();
            }

            return returned;
        }
    }

    public partial class AuthenticationDatabase : AbstractDatabase
    {
        private static AuthenticationDatabase instance = new AuthenticationDatabase();

        private const String dbname = "authenticationservicedb";
        public override String databaseName { get; } = dbname;

        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "user",
                    new Column[]
                    {
                        new Column
                        (
                            "username", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        ),
                        new Column
                        (
                            "password", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "address", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "phonenumber", "VARCHAR(10)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "email", "VARCHAR(30)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        ),
                        new Column
                        (
                            "type", "VARCHAR(10)",
                            new string[]
                            {
                                "NOT NULL"
                            }, false
                        )
                    }
                )
        };
    }
}
