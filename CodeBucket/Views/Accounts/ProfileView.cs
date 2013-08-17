using CodeBucket.Controllers;
using CodeFramework.Controllers;
using MonoTouch.Dialog.Utilities;
using BitbucketSharp.Models;
using CodeFramework.Views;
using CodeBucket.Bitbucket.Controllers.Followers;
using CodeBucket.Bitbucket.Controllers.Events;
using CodeBucket.Bitbucket.Controllers.Groups;
using MonoTouch.Dialog;
using CodeBucket.Bitbucket.Controllers.Repositories;

namespace CodeBucket.Views.Accounts
{
    public class ProfileView : ListView, IImageUpdated, IView<UsersModel>
    {
        private HeaderView _header;
        public string Username { get; private set; }

        public new ProfileController Controller
        {
            get { return (ProfileController)base.Controller; }
            protected set { base.Controller = value; }
        }

        public ProfileView(string username)
        {
            Title = username;
            Username = username;
            Controller = new ProfileController(this, username);
        }

        void IView<UsersModel>.Render(UsersModel model)
        {
            _header.Subtitle = model.User.FirstName ?? "" + " " + (model.User.LastName ?? "");
            _header.Image = ImageLoader.DefaultRequestImage(new System.Uri(model.User.Avatar), this);
            _header.SetNeedsDisplay();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _header = new HeaderView(View.Bounds.Width) { Title = Username };
            Root.Add(new Section(_header));

            var followers = new StyledStringElement("Followers".t(), () => NavigationController.PushViewController(new UserFollowersController(Username), true), Images.Heart);
            var events = new StyledStringElement("Events".t(), () => NavigationController.PushViewController(new EventsController(Username), true), Images.Buttons.Event);
            var groups = new StyledStringElement("Groups".t(), () => NavigationController.PushViewController(new GroupController(Username), true), Images.Buttons.Group);
            var repos = new StyledStringElement("Repositories".t(), () => {
                NavigationController.PushViewController(new RepositoryController(Username) { Model = Controller.IsModelValid ? null : Controller.Model.Repositories }, true);
            }, Images.Repo);
            Root.Add(new [] { new Section { followers, events, groups }, new Section { repos } });
        }

        public void UpdatedImage (System.Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }
    }
}

