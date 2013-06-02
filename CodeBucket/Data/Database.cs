using System.IO;
using CodeBucket.Data;
using SQLite;
using MonoTouch;

namespace CodeBucket.Data
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

