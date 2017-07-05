using Messages.Database;
using Messages.Events;

using MySql.Data.MySqlClient;

using System;

namespace CompanyDirectoryService.Database
{

    
    public partial class CompanyDirectoryDB : AbstractDatabase
    {
        private CompanyDirectoryDB()
            : base()
        {
        }

        /// <summary>
        /// Returns the singleton instance of the CompanyDirectoryDB
        /// If the instance had not yet been created it will be upon calling
        /// this function
        /// </summary>
        /// <returns></returns>
        public static CompanyDirectoryDB getInstance()
        {
            if(instance == null)
            {
                instance = new CompanyDirectoryDB();
            }
            return instance;
        }

        /// <summary>
        /// Attempts to insert a new company into the database
        /// </summary>
        /// <param name="accountInfo">Contains the info about the new company to be added to the database</param>
        /// <returns>true if successful, false otherwise</returns>
        public bool insertNewCompany(AccountCreated accountInfo)
        {
            if (openConnection() == true)
            {
                string query = @"INSERT INTO company(companyname, phonenumber)" +
                @"VALUES('" + accountInfo.username + @"', '" + accountInfo.phonenumber + @"');";

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    //TODO: Also insert the address into the location table, if there is one
                }
                catch(MySqlException e)
                {
                    Console.WriteLine("Unable to complete insert new company into database." +
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
            closeConnection();
            return true;
        }
    }
    
    public partial class CompanyDirectoryDB : AbstractDatabase
    {
        private static CompanyDirectoryDB instance = new CompanyDirectoryDB();

        private const String dbname = "companydirectoryservicedb";
        public override String databaseName { get; } = dbname;

        private const string companyTableStructure =
            @"(companyname VARCHAR(50) NOT NULL," +
            @"phonenumber VARCHAR(10)," +
            @"PRIMARY KEY(companyname)" +
            @")";

        private const string locationTableInfo = 
            @"(address VARCHAR(100) NOT NULL," +
            @"companyname VARCHAR(50) NOT NULL," +
            @"PRIMARY KEY(address, companyname)" +
            @")";

        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "company",
                    companyTableStructure
                ),
            new Table
                (
                    dbname,
                    "location",
                    locationTableInfo
                )
        };

    }
}
