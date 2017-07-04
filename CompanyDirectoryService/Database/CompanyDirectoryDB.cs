using MySql.Data.MySqlClient;

using System;

namespace CompanyDirectoryService.Database
{


    static partial class CompanyDirectoryDB
    {
        /// <summary>
        /// Creates the database, if it was not already created
        /// </summary>
        public static void startupDB()
        {
            createDB();
            createTables();
        }

        /// <summary>
        /// Creates the Tables of the database, if they do not already exist
        /// </summary>
        private static void createTables()
        {
            if (openConnection() == true)
            {
                foreach (string commandString in tableCreateCommands)
                {
                    try
                    {
                        MySqlCommand command = new MySqlCommand(commandString, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        if (e.Number == 1050)//1050 means table already exists
                        {
                            Console.WriteLine("Table already exixts");
                        }
                        else
                        {
                            Console.Write(e.Message + e.Number);
                        }
                    }
                }
                closeConnection();
            }
        }

        /// <summary>
        /// Creates the database, if it does not already exist
        /// </summary>
        private static void createDB()
        {
            String commandString;
            MySqlCommand command;

            commandString = "CREATE DATABASE " + databaseName + ";";
            if (openConnection() == true)
            {
                try
                {
                    command = new MySqlCommand(commandString, connection);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    if (e.Number == 1007)//Database already exists, no need to continure further
                    {
                        Console.WriteLine("Database already exists.");
                        return;
                    }
                    Console.Write(e.Message + e.Number);
                    throw e;
                }
                finally
                {
                    closeConnection();
                }
            }
        }

        /// <summary>
        /// Deletes the database if it exists
        /// </summary>
        public static void deleteDatabase()
        {
            if (openConnection() == true)
            {
                string commandString;
                MySqlCommand command;
                foreach (string name in tableNames)
                {
                    try
                    {
                        commandString = "DROP TABLE " + databaseName + "." + name + ";";
                        command = new MySqlCommand(commandString, connection);
                        command.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        Console.Write(e.Message + e.Number);
                    }
                }

                commandString = "DROP DATABASE " + databaseName + ";";
                command = new MySqlCommand(commandString, connection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    Console.Write(e.Message + e.Number);
                }
                finally
                {
                    closeConnection();
                }
            }
        }

        /// <summary>
        /// Attempts to open a connection to the database
        /// </summary>
        /// <returns>true if the connection was successful, false otherwise</returns>
        private static bool openConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        Console.Write("Cannot connect to database.");
                        break;
                    case 1045:
                        Console.Write("Invalid username or password for database.");
                        break;
                    default:
                        Console.Write("Cannot connect to database. Error code <" + e.Number + ">");
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Attempts to close the connection with the database
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        private static bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Console.Write("Could not close connection to database. Error message: " + e.Message);
                return false;
            }
        }
    }




    static partial class CompanyDirectoryDB
    {
        private static readonly MySqlConnection connection = new MySqlConnection
                ("SERVER=localhost;DATABASE=mysql;UID=" + UID + ";PASSWORD=" + Password);
        private const string UID = "root";
        private const string Password = "abc123";
        private const string databaseName = "CompanyDirectoryServiceDB";


        private const string companyTableCreateCommand = @"CREATE TABLE " + databaseName + "." + "company" +
            @"(name VARCHAR(50) NOT NULL," +
            @"phonenumber VARCHAR(10)," +
            @"PRIMARY KEY(name)" +
            @");";

        private const string locationTableCreateCommand = @"CREATE TABLE " + databaseName + "." + "location" +
            @"(address VARCHAR(100) NOT NULL," +
            @"companyname VARCHAR(50) NOT NULL," +
            @"PRIMARY KEY(address, companyname)" +
            @");";

        private static readonly string[] tableCreateCommands =
        {
            companyTableCreateCommand,
            locationTableCreateCommand
        };

        private static readonly string[] tableNames =
        {
            "company",
            "location"
        };
        
    }
}
