using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public interface IAccountsService
    {
        Task<IEnumerable<Account>> GetAccounts();

        Task Save(Account account);

        Task Remove(Account account);

        Task<Account> Get(string domain, string username);

        Task<Account> Get(string key);
    }
}