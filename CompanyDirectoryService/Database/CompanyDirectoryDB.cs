using Messages.Database;
using Messages.DataTypes.Database.CompanyDirectory;
using Messages.Events;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;

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
                string query =
                    @"INSERT INTO company(companyname, phonenumber, email) " +
                    @"VALUES('" + accountInfo.username + @"', '" + accountInfo.phonenumber +
                    @"', '" + accountInfo.email + @"');" +
                    @"INSERT INTO location(address, companyname) " +
                    @"VALUES('" + accountInfo.address + @"', '" + accountInfo.username + @"');";

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch(MySqlException e)
                {
                    Messages.Debug.consoleMsg("Unable to complete insert new company into database." +
                        " Error :" + e.Number + e.Message);
                    closeConnection();
                    return false;
                }
                catch(Exception e)
                {
                    closeConnection();
                    Messages.Debug.consoleMsg("Unable to Unable to complete insert new company into database." +
                        " Error:" + e.Message);
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

        /// <summary>
        /// Attempts to insert a new company location into the database
        /// </summary>
        /// <param name="address">The location of the company</param>
        /// <param name="companyname">The name of the existuing company</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool insertNewLocation(string address, string companyname)
        {
            if(openConnection() == true)
            {
                string query =
                    @"INSERT INTO location(address, companyname) " +
                    @"VALUES('" + address + @"', '" + companyname + @"');";

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.ExecuteNonQuery();
                }
                catch(MySqlException e)
                {
                    Messages.Debug.consoleMsg("Unable to insert new location into database." +
                        " Error:" + e.Number + e.Message);
                    closeConnection();
                    return false;
                }
                catch (Exception e)
                {
                    closeConnection();
                    Messages.Debug.consoleMsg("Unable to Unable to complete insert new location into database." +
                        " Error:" + e.Message);
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

        /// <summary>
        /// Searches the database for companies that match the given criteria
        /// </summary>
        /// <param name="name">The name of the company to search for</param>
        /// <returns>A list of companies matching the search criteria</returns>
        public CompanyList searchByName(string name)
        {
            if(openConnection() == true)
            {
                string query = @"SELECT * FROM " + databaseName + @".company " +
                  @"WHERE companyname='" + name + @"';";

                List<string> result = new List<string>();

                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    result.Add(dataReader["companyname"] + "");
                }

                dataReader.Close();

                closeConnection();

                return new CompanyList
                {
                    companyNames = result.ToArray()
                };
            }
            return new CompanyList();
        }

        /// <summary>
        /// Searches the database for a company matching the given name
        /// </summary>
        /// <param name="name">The name of the company to search for</param>
        /// <returns>Info about the company being returned</returns>
        public CompanyInstance getCompanyInfo(string name)
        {
            if(openConnection() == true)
            {
                string query = @"SELECT * FROM " + databaseName + @".company " +
                    @"WHERE companyname='" + name + @"';";

                MySqlCommand command = new MySqlCommand(query, connection);

                MySqlDataReader dataReader = command.ExecuteReader();

                if (!dataReader.Read())
                {
                    return new CompanyInstance();
                }
                string companyName = dataReader["companyname"] + "";
                string phoneNumber = dataReader["phonenumber"] + "";
                string email = dataReader["email"] + "";
                List<string> locations = new List<string>();

                dataReader.Close();

                query = @"SELECT * FROM " + databaseName + @".location " +
                    @"WHERE companyname='" + name + @"';";

                command = new MySqlCommand(query, connection);

                dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    locations.Add(dataReader["address"] + "");
                }

                dataReader.Close();

                closeConnection();

                return new CompanyInstance(companyName, phoneNumber, email, locations.ToArray());

            }
            return new CompanyInstance();
        }
    }
    
    public partial class CompanyDirectoryDB : AbstractDatabase
    {

        private const String dbname = "companydirectoryservicedb";
        public override String databaseName { get; } = dbname;

        protected static CompanyDirectoryDB instance;

        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "company",
                    new Column[]
                    {
                        new Column
                        (
                            "companyname", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        ),
                        new Column
                        (
                            "phonenumber", "VARCHAR(10)",
                            null, false
                        ),
                        new Column
                        (
                            "email", "VARCHAR(100)",
                            new string[]
                            {
                                "NOT NULL", 
                                "UNIQUE"
                            }, false
                        )
                    }
                ),
            new Table
                (
                    dbname,
                    "location",
                    new Column[]
                    {
                        new Column
                        (
                            "address", "VARCHAR(100)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        ),
                         new Column
                        (
                            "companyname", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE"
                            }, true
                        )

                    }
                )
        };

    }
}
