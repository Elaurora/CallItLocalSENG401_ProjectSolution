using Messages.Database;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Database
{
    public partial class ChatServiceDatabase : AbstractDatabase
    {
        private ChatServiceDatabase() { }

        public static ChatServiceDatabase getInstance()
        {
            if(instance == null)
            {
                instance = new ChatServiceDatabase();
            }
            return instance;
        }
    }

    public partial class ChatServiceDatabase : AbstractDatabase
    {
        private const String dbname = "chatservicedb";
        public override String databaseName { get; } = dbname;

        protected static ChatServiceDatabase instance;

        protected override Table[] tables { get; } =
        {
            new Table
                (
                    dbname,
                    "chats",
                    new Column[]
                    {
                        new Column
                        (
                            "id", "INT(64)",
                            new string[]
                            {
                                "NOT NULL",
                                "UNIQUE",
                                "AUTO INCREMENT"
                            }, true
                        ),
                        new Column
                        (
                            "initiator", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, true
                        ),
                        new Column
                        (
                            "receiver", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL"
                            }, true
                        )
                    }
                ),
            new Table
                (
                    dbname,
                    "messages",
                    new Column[]
                    {
                        new Column
                        (
                            "id", "INT(64)",
                            new string[]
                            {
                                "NOT NULL",
                            }, true
                        ),
                        new Column
                        (
                            "message", "VARCHAR(250)",
                            new string[]
                            {
                                "NOT NULL",
                            }, false
                        ),
                        new Column
                        (
                            "timestamp", "INT(32)",
                            new string[]
                            {
                                "NOT NULL",
                            }, true
                        ),
                        new Column
                        (
                            "sender", "VARCHAR(50)",
                            new string[]
                            {
                                "NOT NULL",
                            }, false
                        )
                    }
                )
        };

    }
}
