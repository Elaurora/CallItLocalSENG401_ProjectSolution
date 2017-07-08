using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages.Database
{
    public partial class Column
    {
        public Column(string name, string type, string[] mods, bool primaryKey)
        {
            this.name = name.ToLower();
            this.type = type;
            this.primaryKey = primaryKey;
            this.mods = mods;
        }

        /// <summary>
        /// Returns the structure of the column in a way that can be used in a
        /// CREATE table statement
        /// </summary>
        /// <returns>The structure of the column as a string</returns>
        public string getCreateStructure()
        {
            string structure = name + " " + type;
            if (mods != null)
            {
                foreach (string mod in mods)
                {
                    structure += " " + mod;
                }
            }
            return structure;
        }
    }

    public partial class Column
    {
        /// <summary>
        /// The name of the column
        /// </summary>
        private string name;
        public string getName()
        {
            return name;
        }

        /// <summary>
        /// The data type of the column
        /// </summary>
        private string type;
        public string getType()
        {
            return type;
        }

        /// <summary>
        /// The modfiers on the column, such as UNIQUE or NOT NULL
        /// May be null
        /// </summary>
        private string[] mods;
        public string[] getMods()
        {
            return mods;
        }

        /// <summary>
        /// Represents whether or not this column is a primary key.
        /// </summary>
        private bool primaryKey;
        public bool isPrimary()
        {
            return primaryKey;
        }
    }
}
