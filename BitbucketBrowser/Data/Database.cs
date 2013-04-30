using System;
using System.IO;
using SQLite;
using MonoTouch;

namespace BitbucketBrowser.Data
{
    public class Database : SQLiteConnection
    {
        private Database(string file) 
			: base(file)
        {
            CreateTable<Account>();
        }

		private static void Upgrade() 
		{
			if (!File.Exists(Utilities.BaseDir + "/Documents/data.db"))
				return;
		}

        public readonly static Database Main = new Database(Utilities.BaseDir + "/Documents/database.db");
    }
}

