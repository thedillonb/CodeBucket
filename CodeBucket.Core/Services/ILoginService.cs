using CodeBucket.Core.Data;
using System.Threading.Tasks;

namespace CodeBucket.Core.Services
{
    public interface ILoginService
    {
		Task<LoginData> LoginWithToken(string clientId, string clientSecret, string code, string redirect, string requestDomain, string apiDomain, BitbucketAccount existingAccount);

		Task<BitbucketSharp.Client> LoginAccount(BitbucketAccount account);

		LoginData Authenticate(string user, string pass, BitbucketAccount existingAccount);
    }

	public class LoginData
	{
		public BitbucketSharp.Client Client { get; set; }
		public BitbucketAccount Account { get; set; }
	}
}