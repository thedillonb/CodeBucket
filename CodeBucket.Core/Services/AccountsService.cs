using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Akavache;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public class AccountsService : IAccountsService
    {
        public Task<IEnumerable<Account>> GetAccounts()
            => BlobCache.UserAccount.GetAllObjects<Account>()
                        .Select(x => x.OrderBy(y => y.Username).AsEnumerable())
                        .ToTask();

        public Task Save(Account account)
            => BlobCache.UserAccount.InsertObject(GetKey(account), account).ToTask();

        public Task Remove(Account account)
            => BlobCache.UserAccount.Invalidate(GetKey(account)).ToTask();

        public Task<Account> Get(string domain, string username) => Get(GetKey(username, domain));

        public Task<Account> Get(string key)
            => BlobCache.UserAccount.GetObject<Account>("account_" + key)
                        .Catch(Observable.Return<Account>(null))
                        .ToTask();

        private string GetKey(Account account)
            => GetKey(account.Username, account.Domain);

        private string GetKey(string username, string domain)
            => "account_" + username + domain;
    }
}
