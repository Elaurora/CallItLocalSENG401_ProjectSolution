using Messages.Database;
using Messages.DataTypes.Database.CompanyDirectory;
using Messages.NServiceBus.Events;
using Messages.ServiceBusRequest.CompanyDirectory.Requests;
using Messages.ServiceBusRequest.CompanyDirectory.Responses;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;

namespace CompanyDirectoryService.Database
{
    /// <summary>
    /// This class is used to manipulate and read the Company Directory Service's database in a safe and consistent manner.
    /// It follows the singleton design pattern, as only one instance of this class should ever be in existance at any given time.
    /// </summary>
    public partial class CompanyDirectoryDB : AbstractDatabase
    {
        /// <summary>
        /// Private default constructor to enforce the use of the singleton design pattern
        /// </summary>
        private CompanyDirectoryDB()
            : base()
        {
        }

        /// <summary>
        /// Gets the singleton instance of the database
        /// </summary>
        /// <returns>The singleton instance of the database</returns>
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
        public CompanySearchResponse searchByName(CompanySearchRequest request)
        {
            bool result = false;
            string response = "";

            if(openConnection() == true)
            {
                string query = @"SELECT * FROM " + databaseName + @".company " +
                  @"WHERE companyname='" + request.searchDeliminator + @"';";
                MySqlDataReader dataReader = null;
                List<string> list = new List<string>();

                try
                {
                    MySqlCommand command = new MySqlCommand(query, connection);

                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        list.Add(dataReader.GetString("companyname"));
                    }

                    result = true;
                }
                catch(Exception e)
                {
                    response = e.Message;
                }
                finally
                {
                    if(dataReader != null && dataReader.IsClosed == false)
                    {
                        dataReader.Close();
                    }
                    closeConnection();
                }

                CompanyList CList = new CompanyList
                {
                    companyNames = list.ToArray()
                };

                return new CompanySearchResponse(result, response, CList);
            }
            return new CompanySearchResponse(false, "Could not connect to database", null);
        }

        /// <summary>
        /// Searches the database for a company matching the given name
        /// </summary>
        /// <param name="name">The name of the company to search for</param>
        /// <returns>Info about the company being returned</returns>
        public GetCompanyInfoResponse getCompanyInfo(GetCompanyInfoRequest request)
        {
            bool result = false;
            string response = "";
            CompanyInstance requestData = request.companyInfo;

            if (openConnection() == true)
            {
                string query = @"SELECT * FROM " + databaseName + @".company " +
                    @"WHERE companyname='" + requestData.companyName + @"';";
                MySqlDataReader dataReader = null;
                CompanyInstance returnData = null;

                try
                {

                    MySqlCommand command = new MySqlCommand(query, connection);

                    dataReader = command.ExecuteReader();

                    if (!dataReader.Read())
                    {
                        return new GetCompanyInfoResponse(false, "No company with the given name exists in our database.", requestData);
                    }
                    string companyName = dataReader.GetString("companyname");
                    string phoneNumber = dataReader.GetString("phonenumber");
                    string email = dataReader.GetString("email");
                    List<string> locations = new List<string>();

                    dataReader.Close();

                    query = @"SELECT * FROM " + databaseName + @".location " +
                        @"WHERE companyname='" + requestData.companyName + @"';";

                    command = new MySqlCommand(query, connection);

                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        locations.Add(dataReader.GetString("address"));
                    }

                    returnData = new CompanyInstance(
                        companyName, phoneNumber, email, locations.ToArray()
                    );

                    result = true;

                }
                catch (Exception e)
                {
                    response = e.Message;
                }
                finally
                {
                    if (dataReader != null && dataReader.IsClosed == false)
                    {
                        dataReader.Close();
                    }
                    closeConnection();
                }

                return new GetCompanyInfoResponse(result, response, returnData);

            }
            return new GetCompanyInfoResponse(false, "Unable to connect to database", null);
        }
    }

    /// <summary>
    /// This portion of the class contains the member variables as well as the schema definition in the form of Table/Column objects
    /// </summary>
    public partial class CompanyDirectoryDB : AbstractDatabase
    {
        /// <summary>
        /// The name of the database.
        /// Both of these properties are required in order for both the base class and the
        /// table definitions below to have access to the variable.
        /// </summary>
        private const String dbname = "companydirectoryservicedb";
        public override String databaseName { get; } = dbname;

        /// <summary>
        /// The singleton instance of the database
        /// </summary>
        protected static CompanyDirectoryDB instance;

        /// <summary>
        /// This property represents the database schema, and will be used by the base class
        /// to create and delete the database.
        /// </summary>
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
                            }, true
                        ),
                         new Column
                        (
                            "companyname", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                            }, true
                        )

                    }
                )
        };

    }
}
