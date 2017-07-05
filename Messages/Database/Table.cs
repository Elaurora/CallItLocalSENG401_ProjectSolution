using System;

namespace Messages.Database
{

    /// <summary>
    /// This class represents a table in a MySQL Database.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// Creates a data structure to hold information about your table
        /// </summary>
        /// <param name="name">The name of the table</param>
        /// <param name="structure">The MySQL information to create the table. Should be able to append this string
        /// to a CREATE TABLE command.</param>
        public Table(string databaseName, string tableName, string structure)
        {
            this.databaseName = databaseName;
            this.tableName = tableName;
            this.structure = structure;
        }

        /// <summary>
        /// Generates a MySQL command that when executed will create this table.
        /// </summary>
        /// <returns>The MySQL statement to create the table</returns>
        public string getCreateCommand()
        {
            return "CREATE TABLE " + databaseName + "." + tableName + structure + ";";
        }

        /// <summary>
        /// Generates a MySQL command that when executed will delete this table.
        /// </summary>
        /// <returns>The MySQL statement to create the table</returns>
        public string getDropCommand()
        {
            return "DROP TABLE " + databaseName + "." + tableName + ";";
        }
    }

    /// <summary>
    /// This class represents a table in a MySQL Database.
    /// </summary>
    public partial class Table
    {
        /// <summary>
        /// The name of the database this table is used in
        /// </summary>
        private String databaseName;

        public string getDBName()
        {
            return databaseName;
        }
        public void setDBName(string newName)
        {
            this.databaseName = newName;
        }

        /// <summary>
        /// The name of the table
        /// </summary>
        private string tableName;

        public string getTableName()
        {
            return tableName;
        }
        public void setTableName(string newName)
        {
            this.tableName = newName;
        }

        /// <summary>
        /// The MySQL information to create the table. Should be able to append this string
        /// to a CREATE TABLE command.
        /// </summary>
        private string structure;

        public string getStructure()
        {
            return structure;
        }
        public void setStructure(string newStructure)
        {
            this.structure = newStructure;
        }
    }

}
