using System;
using System.Collections.Generic;

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
        public Table(string databaseName, string tableName, Column[] columns)
        {
            this.databaseName = databaseName;
            this.tableName = tableName;
            this.columns = columns;
        }

        /// <summary>
        /// Generates a MySQL command that when executed will create this table.
        /// </summary>
        /// <returns>The MySQL statement to create the table</returns>
        public string getCreateCommand()
        {
            string query = "CREATE TABLE " + databaseName + "." + tableName + "(";
            List<string> primaryKeys = new List<string>();

            foreach(Column column in columns)
            {
                query += column.getCreateStructure() + ",";
                if(column.isPrimary() == true)
                {
                    primaryKeys.Add(column.getName());
                }
            }
            if (primaryKeys.Count == 0)
            {
                query = query.Substring(0, query.Length - 1);//Removes the extra comma, since no primary keys are specified
            }
            else
            {
                query += "PRIMARY KEY(";
                int i;
                for(i = 0; i != primaryKeys.Count - 1; i++)
                {
                    query += primaryKeys[i] + ", ";
                }
                query += primaryKeys[i] + ")";
            }
            query += ");";
            return query;
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
        /// Represents the structure of the database
        /// </summary>
        private Column[] columns;
        public Column[] getColumns()
        {
            return columns;
        }
        public Column getColumn(string name)
        {
            foreach(Column column in columns)
            {
                if (column.getName().Equals(name.ToLower()))
                {
                    return column;
                }
            }
            return null;
        }
    }

}
