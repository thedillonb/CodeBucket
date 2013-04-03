using System;
using System.Collections.Generic;
using MonoTouch;
using System.Collections;

namespace BitbucketBrowser.Data
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
			var name = Utilities.Defaults.StringForKey("DEFAULT_ACCOUNT");
			if (name == null)
				return null;
			
			foreach (Account a in this)
				if (a.Username.ToLower().Equals(name.ToLower()))
					return a;
			return null;
		}

		/// <summary>
		/// Sets the default account
		/// </summary>
		public void SetDefault(Account account)
		{
			if (account == null)
				Utilities.Defaults.RemoveObject("DEFAULT_ACCOUNT");
			else
				Utilities.Defaults.SetString(account.Username, "DEFAULT_ACCOUNT");
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
		public void Remove(string username)
		{
			var q = from f in Database.Main.Table<Account>()
				where f.Username == username
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
			var query = Database.Main.Query<Account>("select * from Account where LOWER(Username) = LOWER(?)", account.Username);
			return query.Count > 0;
		}

		/// <summary>
		/// Find the specified account via it's username
		/// </summary>
		public Account Find(string username)
		{
			var query = Database.Main.Query<Account>("select * from Account where LOWER(Username) = LOWER(?)", username);
			if (query.Count > 0)
				return query[0];
			return null;
		}
	}

}

