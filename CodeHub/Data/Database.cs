using System;
using SQLite;
using MonoTouch;

namespace CodeHub
{
    public class Database : SQLiteConnection
    {
        private Database(string file) : base(file)
        {
            CreateTable<Account>();
        }

        public readonly static Database Main = new Database(Utilities.BaseDir + "/Documents/data.db");
    }
}

