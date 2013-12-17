using System;
using BitbucketSharp.Models;
using System.Threading;
using MonoTouch.Foundation;
using CodeFramework.Controllers;

namespace CodeBucket.Controllers
{
    public class ProfileController : Controller<UsersModel>
    {
        private string _username;

        public ProfileController(IView<UsersModel> view, string username)
            : base(view)
        {
            _username = username;
        }

        public override void Update(bool forced)
        {
            Model = Application.Client.Users[_username].GetInfo(forced);
        }
    }
}

