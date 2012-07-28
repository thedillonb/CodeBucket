using System;
using SQLite;

namespace BitbucketBrowser
{
    public class Database : SQLiteConnection
    {
        private Database(string file) : base(file)
        {
            CreateTable<Account>();
        }

        public readonly static Database Main = new Database(Utils.Util.BaseDir + "/Documents/data.db");
    }
}

