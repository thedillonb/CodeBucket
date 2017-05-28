using System.Threading.Tasks;
using CodeBucket.Client;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public interface IApplicationService
    {
		BitbucketClient Client { get; }
 
        Account Account { get; }

        Task<Account> GetDefaultAccount();

        void SetDefaultAccount(Account account);

        Task SaveAccount();

		void ActivateUser(Account account, BitbucketClient client);

        Task RefreshToken();

        Task Login(string code);
    }
}