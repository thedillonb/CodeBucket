using CodeFramework.Core.Services;
using CodeBucket.Core.Data;

namespace CodeBucket.Core.Services
{
    public interface IApplicationService
    {
		BitbucketSharp.Client Client { get; }
 
		BitbucketAccount Account { get; }

        IAccountsService Accounts { get; }

		void ActivateUser(BitbucketAccount account, BitbucketSharp.Client client);
    }
}