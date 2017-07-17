using MySql.Data.MySqlClient;

using System;

namespace Messages.Database
{
    /// <summary>
    /// This class is used as a base class for the creation and deletion of a database.
    /// To use this class you will need to implement the databaseName and tables properties.
    /// It is recommended that the inhereting class be a singleton.
    /// </summary>
    public abstract partial class AbstractDatabase
    {
        /// <summary>
        /// Creates the connection object and attempts to create the database if it does not exist already
        /// </summary>
        protected AbstractDatabase()
        {
            createDB();
        }

        /// <summary>
        /// Creates the database, if it does not already exist
        /// </summary>
        public void createDB()
        {
            string commandString;
            MySqlCommand command;
            connection = new MySqlConnection("SERVER=localhost;DATABASE=mysql;UID=" + UID + ";AUTO ENLIST=false;PASSWORD=" + Password);
            commandString = "CREATE DATABASE " + databaseName + ";";
            if (openConnection() == true)
            {
                //First try to create the actual database
                try
                {
                    command = new MySqlCommand(commandString, connection); 
                    command.ExecuteNonQuery();
                    Messages.Debug.consoleMsg("Successfully created database " + databaseName);
                }
                catch (MySqlException e)
                {
                    if (e.Number == 1007)//Database already exists, no need to continure further
                    {
                        Messages.Debug.consoleMsg("Database already exists.");
                        closeConnection();
                        connection = new MySqlConnection("SERVER=localhost;DATABASE=" + databaseName + ";UID=" + UID + ";AUTO ENLIST=false;PASSWORD=" + Password);
                        return;
                    }
                    Messages.Debug.consoleMsg("Unable to create database"
                        + databaseName + " Error: " +  e.Number + e.Message);
                    closeConnection();
                    return;
                }

                //Then try to create each of the tables in the database
                foreach (Table table in tables)
                {
                    try
                    {
                        commandString = table.getCreateCommand();
                        command = new MySqlCommand(commandString, connection);
                        command.ExecuteNonQuery();
                        Messages.Debug.consoleMsg("Successfully created the table "
                            + table.getDBName() + "." + table.getTableName());

                    }
                    catch (MySqlException e)
                    {
                        Messages.Debug.consoleMsg("Unable to create table "
                            + table.getDBName() + "." + table.getTableName()
                            + " Error: " + e.Number + e.Message);
                    }
                }

                //TODO: Add functionality to populate database with some default shit

                closeConnection();
                connection = new MySqlConnection("SERVER=localhost;DATABASE=" + databaseName + ";UID=" + UID + ";AUTO ENLIST=false;PASSWORD=" + Password);
            }
        }

        /// <summary>
        /// Deletes the database if it exists
        /// </summary>
        public void deleteDatabase()
        {
            if (openConnection() == true)
            {
                string commandString;
                MySqlCommand command;
                foreach (Table table in tables)
                {
                    try
                    {
                        commandString = table.getDropCommand();
                        command = new MySqlCommand(commandString, connection);
                        command.ExecuteNonQuery();
                        Messages.Debug.consoleMsg("Successfully deleted table "
                            + table.getDBName() + "." + table.getTableName());
                    }
                    catch (MySqlException e)
                    {
                        Messages.Debug.consoleMsg("Unable to delete table "
                            + table.getDBName() + "." + table.getTableName()
                            + " Error: " + e.Number + e.Message);
                    }
                }

                commandString = "DROP DATABASE " + databaseName + ";";
                command = new MySqlCommand(commandString, connection);
                try
                {
                    command.ExecuteNonQuery();
                    Messages.Debug.consoleMsg("Successfully deleted database " + databaseName);
                }
                catch (MySqlException e)
                {
                    Messages.Debug.consoleMsg("Unable to delete database " + databaseName
                        + " Error: " + e.Number + e.Message);
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
        protected bool openConnection()
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
                        Messages.Debug.consoleMsg("Cannot connect to database.");
                        break;
                    case 1045:
                        Messages.Debug.consoleMsg("Invalid username or password for database.");
                        break;
                    default:
                        Messages.Debug.consoleMsg("Cannot connect to database. Error code <" + e.Number + ">");
                        break;
                }
                return false;
            }
            catch (InvalidOperationException e)
            {
                if(e.Message.Equals("The connection is already open."))
                {
                    return true;
                }
                throw e;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Attempts to close the connection with the database
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        protected bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException e)
            {
                Messages.Debug.consoleMsg("Could not close connection to database. Error message: " + e.Number + e.Message);
                return false;
            }
        }
    }

    public abstract partial class AbstractDatabase
    {
        protected MySqlConnection connection;
        private string UID = "root";
        private string Password = "abc123";
        public abstract String databaseName { get; }

        protected abstract Table[] tables { get; }
    }
}
