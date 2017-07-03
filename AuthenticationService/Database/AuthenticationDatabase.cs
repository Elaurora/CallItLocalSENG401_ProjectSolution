using MySql.Data.MySqlClient;

using System;

namespace AuthenticationService.Database
{
    static partial class AuthenticationDatabase
    {

        public static void startupDB()
        {
            createDB();
            createTables();
        }

        private static void createTables()
        {
            openConnection();
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

        private static void createDB()
        {
            String commandString;
            MySqlCommand command;

            commandString = "CREATE DATABASE CompanyDirectoryServiceDB;";

            try
            {
                openConnection();
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

        public static void deleteDatabase()
        {
            openConnection();
            string commandString;
            MySqlCommand command;
            foreach (string name in tableNames)
            {
                try
                {
                    commandString = "DROP TABLE " + name + ";";
                    command = new MySqlCommand(commandString, connection);
                    command.ExecuteNonQuery();
                }
                catch (MySqlException e)
                {
                    Console.Write(e.Message + e.Number);
                }
            }

            commandString = "DROP DATABASE CompanyDirectoryServiceDB;";
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

    static partial class AuthenticationDatabase
    {
        private static readonly MySqlConnection connection = new MySqlConnection
                ("SERVER=localhost;DATABASE=mysql;UID=" + UID + ";PASSWORD=" + Password);
        private const string UID = "root";
        private const string Password = "abc123";


        private const string userTableCreateCommand = "CREATE TABLE User" +
            "(username CHAR(50) NOT NULL," +
            "password CHAR(50), NOT NULL," +
            "PRIMARY KEY(name, password)" +
            ");";

        private const string businessUserTableCreateCommand = "CREATE TABLE BusinessUser" +
            "(username CHAR(50) NOT NULL," +
            "password CHAR(50), NOT NULL," +
            "phonenumber char(10)" +
            "PRIMARY KEY(name, password)" +
            ");";

        private static readonly string[] tableCreateCommands =
        {
            userTableCreateCommand,
            businessUserTableCreateCommand
        };

        private static readonly string[] tableNames =
        {
            "User",
            "BusinessUser"
        };
    }
}
