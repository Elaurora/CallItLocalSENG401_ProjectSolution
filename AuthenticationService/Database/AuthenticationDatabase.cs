using Messages.Database;
using Messages.Events;

using MySql.Data.MySqlClient;

using System;

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
        public bool insertNewUserAccount(AccountCreated accountInfo)
        {
            if(openConnection() == true)
            {
                string query = @"INSERT INTO user(username, password, address) " +
                    @"VALUES('" + accountInfo.username + @"', '" + accountInfo.password + 
                    @"', '" + accountInfo.address + @"');";

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch(MySqlException e)
                {
                    Console.WriteLine("Unable to complete insert new user into database." +
                        " Error :" + e.Number + e.Message);
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
    }

    public partial class AuthenticationDatabase : AbstractDatabase
    {
        private static AuthenticationDatabase instance = new AuthenticationDatabase();

        private const String dbname = "authenticationservicedb";
        public override String databaseName { get; } = dbname;

        private const string userTableStructure =
            "(username VARCHAR(50) NOT NULL UNIQUE," +
            "password VARCHAR(50) NOT NULL," +
            "address VARCHAR(50) NOT NULL," +
            "PRIMARY KEY(username)" +
            ")";

        private const string businessUserTableStructure =
            "(username VARCHAR(50) NOT NULL UNIQUE," +
            "password VARCHAR(50) NOT NULL," +
            "phonenumber VARCHAR(10)," +
            "PRIMARY KEY(username)" +
            ")";

        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "user",
                    userTableStructure
                ),
            new Table
                (
                    dbname,
                    "businessuser",
                    businessUserTableStructure
                )
        };
    }
}
