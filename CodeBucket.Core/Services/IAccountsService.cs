using System.Collections.Generic;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public interface IAccountsService : IEnumerable<BitbucketAccount>
    {
        /// <summary>
        /// Gets the active account
        /// </summary>
        BitbucketAccount ActiveAccount { get; }

        /// <summary>
        /// Sets the active account
        /// </summary>
        /// <param name="account"></param>
        void SetActiveAccount(BitbucketAccount account);

        /// <summary>
        /// Gets the default account
        /// </summary>
        BitbucketAccount GetDefault();

        /// <summary>
        /// Sets the default account
        /// </summary>
        void SetDefault(BitbucketAccount account);

        /// <summary>
        /// Insert the specified account.
        /// </summary>
        void Insert(BitbucketAccount account);

        /// <summary>
        /// Remove the specified account.
        /// </summary>
        void Remove(BitbucketAccount account);

        /// <summary>
        /// Update this instance in the database
        /// </summary>
        void Update(BitbucketAccount account);

        /// <summary>
        /// Checks to see whether a specific account exists (Username comparison)
        /// </summary>
        bool Exists(BitbucketAccount account);

        /// <summary>
        /// Find the specified account via it's username
        /// </summary>
        BitbucketAccount Find(int id);
    }
}