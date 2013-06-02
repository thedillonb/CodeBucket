using System.Collections.Generic;
using BitbucketBrowser.Data;
using MonoTouch;
using System.Collections;
using System.Linq;

namespace CodeBucket.Data
{
	/// <summary>
	/// A collection of accounts within the system
	/// </summary>
	public class Accounts : IEnumerable<Account>
	{
		/// <summary>
		/// Gets the count of accounts in the database
		/// </summary>
		public int Count 
		{
			get { return Database.Main.Table<Account>().Count(); }
		}

		/// <summary>
		/// Gets the default account
		/// </summary>
		public Account GetDefault()
		{
			var id = Utilities.Defaults.IntForKey("DEFAULT_ACCOUNT");
			return Database.Main.Table<Account>().SingleOrDefault(x => x.Id == id);
		}

		/// <summary>
		/// Sets the default account
		/// </summary>
		public void SetDefault(Account account)
		{
			if (account == null)
				Utilities.Defaults.RemoveObject("DEFAULT_ACCOUNT");
			else
				Utilities.Defaults.SetInt(account.Id, "DEFAULT_ACCOUNT");
			Utilities.Defaults.Synchronize();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		public IEnumerator<Account> GetEnumerator ()
		{
			return Database.Main.Table<Account>().GetEnumerator();
		}

		/// <summary>
		/// Insert the specified account.
		/// </summary>
		public void Insert(Account account)
		{
			Database.Main.Insert(account);
		}

		/// <summary>
		/// Remove the specified account.
		/// </summary>
		public void Remove(Account account)
		{
			account.Delete();
		}

		/// <summary>
		/// Remove the specified username.
		/// </summary>
		public void Remove(string username, Account.Type type)
		{
			var q = from f in Database.Main.Table<Account>()
				where f.Username == username && f.AccountType == type
					select f;
			var account = q.FirstOrDefault();
			if (account != null)
				Remove(account);
		}

		/// <summary>
		/// Checks to see whether a specific account exists (Username comparison)
		/// </summary>
		public bool Exists(Account account)
		{
			return Find(account.Username, account.AccountType) != null;
		}

		/// <summary>
		/// Find the specified account via it's username
		/// </summary>
		public Account Find(string username, Account.Type type)
		{
			var query = Database.Main.Query<Account>("select * from Account where LOWER(Username) = LOWER(?) and AccountType = ?", username, type);
			if (query.Count > 0)
				return query[0];
			return null;
		}
	}

}

