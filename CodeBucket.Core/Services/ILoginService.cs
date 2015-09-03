using CodeBucket.Core.Data;
using System.Threading.Tasks;

namespace CodeBucket.Core.Services
{
    public interface ILoginService
    {
		Task<BitbucketSharp.Client> LoginAccount(BitbucketAccount account);
    }
}