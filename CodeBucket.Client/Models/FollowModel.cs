using System.Collections.Generic;

namespace CodeBucket.Client.Models
{
    public class FollowerModel : UserModel { }

    public class FollowersModel
    {
        public int Count { get; set; }
        public List<FollowerModel> Followers { get; set; }
    }
}
