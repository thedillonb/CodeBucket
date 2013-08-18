using CodeBucket.Controllers;
using CodeFramework.Controllers;
using MonoTouch.Dialog.Utilities;
using BitbucketSharp.Models;
using CodeFramework.Views;
using MonoTouch.Dialog;

namespace CodeBucket.ViewControllers
{
    public class ProfileViewController : BaseControllerDrivenViewController, IImageUpdated, IView<UsersModel>
    {
        private HeaderView _header;
        public string Username { get; private set; }

        public new ProfileController Controller
        {
            get { return (ProfileController)base.Controller; }
            protected set { base.Controller = value; }
        }

        public ProfileViewController(string username)
        {
            Title = username;
            Username = username;
            Controller = new ProfileController(this, username);
        }

        public void Render(UsersModel model)
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

            var followers = new StyledStringElement("Followers".t(), () => NavigationController.PushViewController(new UserFollowersViewController(Username), true), Images.Heart);
            var events = new StyledStringElement("Events".t(), () => NavigationController.PushViewController(new EventsViewController(Username), true), Images.Buttons.Event);
            var groups = new StyledStringElement("Groups".t(), () => NavigationController.PushViewController(new GroupViewController(Username), true), Images.Buttons.Group);
            var repos = new StyledStringElement("Repositories".t(), () => {
                var viewController = new RepositoriesViewController(Username);
                viewController.Controller.Model = Controller.IsModelValid ? null : new ListModel<RepositoryDetailedModel> { Data = Controller.Model.Repositories };
                NavigationController.PushViewController(viewController, true);
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

