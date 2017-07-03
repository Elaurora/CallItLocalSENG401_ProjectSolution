using MySql.Data.MySqlClient;

using System;

namespace CompanyDirectoryService.Database
{
    class CompanyDirectoryDBConnection
    {
        private MySqlConnection connection;
        private string server;
        private string dbname;
        private string uid;
        private string password;

        public CompanyDirectoryDBConnection()
        {
            server = "localhost";
            dbname = "CompanyDirectoryServiceDB";
            uid = "username";
            password = "password";
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                dbname + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool openConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch(MySqlException e)
            {
                switch (e.Number)
                {
                    case 0:
                        Console.Write("Cannot connect to database named <" + dbname + ">");
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

        private bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch(MySqlException e)
            {
                Console.Write("Could not close connection to database. Error message: " + e.Message + e.Number);
                return false;
            }
        }

        public void addNewCompany(string name, string phoneNumber)
        {
            if(openConnection() == true)
            {
                string query = "INSERT INTO Company (name, phonenumber) VALUES('" + name + "', '" + phoneNumber + "')";

                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.ExecuteNonQuery();

                closeConnection();
            }
        }
    }
}
