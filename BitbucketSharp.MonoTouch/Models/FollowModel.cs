using System.Collections.Generic;

namespace BitbucketSharp.Models
{
    public class FollowerModel : UserModel { }

    public class FollowersModel
    {
        public int Count { get; set; }
        public List<FollowerModel> Followers { get; set; }
    }
}
