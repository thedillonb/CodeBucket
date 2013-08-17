using System.IO;
using CodeBucket.Data;
using SQLite;
using MonoTouch;
using System.Linq;
using System.Collections.Generic;
using System;

namespace CodeBucket.Data
{
    public class Database : SQLiteConnection
    {
        //The current database version
        private static readonly int CurrentDBVersion = 1;
        private static readonly string DatabaseVersionKey = "DATABASE_VERSION";
        private static readonly string DatabaseFilePath = System.IO.Path.Combine(Utilities.BaseDir, "Documents/data.db");
        private static Database _database;

        public static Database Main
        {
            get 
            {
                if (_database == null)
                    _database = new Database(DatabaseFilePath, System.IO.File.Exists(DatabaseFilePath));
                return _database;
            }
        }


        private Database(string file, bool dbExists) 
			: base(file)
        {
            //We only need to run upgrades if the database exists...
            if (dbExists == true)
            {
                //Run any upgrades needed
                Upgrade();
            }

            //Execute the typical stuff
            CreateTable<CodeBucket.Data.Account>();
            CreateTable<CodeBucket.Data.PinnedRepository>();
            CreateTable<CodeBucket.Data.Filter>();
        }

		private void Upgrade() 
		{
            var version = Utilities.Defaults.IntForKey(DatabaseVersionKey);
            if (version == CurrentDBVersion)
                return;

            //Keep going until we upgrade to the current!
            for (; version < CurrentDBVersion; version++)
            {
                try
                {
                    if (version == 0)
                    {
                        var oldAccounts = this.Table<Account>();
                        var newAccounts = new List<CodeBucket.Data.Account>(oldAccounts.Select(x => 
                            new CodeBucket.Data.Account() {
                                AvatarUrl = x.AvatarUrl,
                                DontRemember = x.DontRemember,
                                Password = x.Password,
                                Username = x.Username,
                            }
                        ));

                        this.DropTable<Account>();
                        this.CreateTable<CodeBucket.Data.Account>();
                        this.InsertAll(newAccounts);
                    }
                }
                catch (Exception e)
                {
                    Utilities.LogException("Unable to migrate database from version 0", e);
                }
            }

            Utilities.Defaults.SetInt(version, DatabaseVersionKey);
            Utilities.Defaults.Synchronize();
		}


        #region Migration Objects (Old database objects)

        private class Account
        {
            [PrimaryKey]
            public string Username { get; set; }
            public string Password { get; set; }
            public string AvatarUrl { get; set; }
            public bool DontRemember { get; set; }
        }

        #endregion
    }
}

