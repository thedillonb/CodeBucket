using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using SQLite;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public class AccountsService : IAccountsService
    {
        private readonly SQLiteConnection _userDatabase;
        private readonly IDefaultValueService _defaults;
        private readonly string _accountsPath;

        public BitbucketAccount ActiveAccount { get; private set; }

        public AccountsService(IDefaultValueService defaults, IAccountPreferencesService accountPreferences)
        {
            _defaults = defaults;
            _accountsPath = accountPreferences.AccountsDir;

            // Assure creation of the accounts path
            if (!Directory.Exists(_accountsPath))
                Directory.CreateDirectory(_accountsPath);

            _userDatabase = new SQLiteConnection(Path.Combine(_accountsPath, "accounts.db"));
            _userDatabase.CreateTable<BitbucketAccount>();
        }

        public BitbucketAccount GetDefault()
        {
            int id;
			return !_defaults.TryGet("DEFAULT_ACCOUNT", out id) ? null : Find(id);
        }

        public void SetDefault(BitbucketAccount account)
        {
            if (account == null)
                _defaults.Set("DEFAULT_ACCOUNT", null);
            else
                _defaults.Set("DEFAULT_ACCOUNT", account.Id);
        }

        public void SetActiveAccount(BitbucketAccount account)
        {
			if (account != null)
			{
				var accountDir = CreateAccountDirectory(account);
				if (!Directory.Exists(accountDir))
					Directory.CreateDirectory(accountDir);
			}

            SetDefault(account);
            ActiveAccount = account;
        }

        protected string CreateAccountDirectory(BitbucketAccount account)
        {
            return Path.Combine(_accountsPath, account.Id.ToString(CultureInfo.InvariantCulture));
        }

        public void Insert(BitbucketAccount account)
        {
			lock (_userDatabase)
			{
				_userDatabase.Insert(account);
			}
        }

        public void Remove(BitbucketAccount account)
        {
			lock (_userDatabase)
			{
				_userDatabase.Delete(account);
			}
            var accountDir = CreateAccountDirectory(account);

            if (!Directory.Exists(accountDir))
                return;
            Directory.Delete(accountDir, true);
        }

        public void Update(BitbucketAccount account)
        {
			lock (_userDatabase)
			{
				_userDatabase.Update(account);
			}
        }

        public bool Exists(BitbucketAccount account)
        {
			return Find(account.Id) != null;
        }

        public BitbucketAccount Find(int id)
        {
			lock (_userDatabase)
			{
                var query = _userDatabase.Find<BitbucketAccount>(x => x.Id == id);
				return query;
			}
        }

        public BitbucketAccount Find(string username)
        {
            lock (_userDatabase)
            {
                var query = _userDatabase.Find<BitbucketAccount>(x => x.Username == username);
                return query;
            }
        }

        public IEnumerator<BitbucketAccount> GetEnumerator()
        {
            return _userDatabase.Table<BitbucketAccount>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
