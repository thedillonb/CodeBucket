using CodeBucket.Client;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public interface IApplicationService
    {
		BitbucketClient Client { get; }
 
		BitbucketAccount Account { get; }

        IAccountsService Accounts { get; }

		void ActivateUser(BitbucketAccount account, CodeBucket.Client.BitbucketClient client);
    }
}