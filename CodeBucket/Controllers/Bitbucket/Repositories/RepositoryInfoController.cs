using BitbucketSharp.Models;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using MonoTouch.Dialog.Utilities;
using CodeBucket.Bitbucket.Controllers;
using CodeBucket.Bitbucket.Controllers.Issues;
using CodeBucket.Bitbucket.Controllers.Followers;
using CodeBucket.Bitbucket.Controllers.Events;
using CodeBucket.Bitbucket.Controllers.Wikis;
using CodeBucket.Bitbucket.Controllers.Branches;
using CodeBucket.Bitbucket.Controllers.Changesets;
using CodeBucket.Controllers;
using CodeFramework.Views;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class RepositoryInfoController : BaseModelDrivenController, IImageUpdated
    {
        private HeaderView _header;

        public new RepositoryDetailedModel Model 
        { 
            get { return (RepositoryDetailedModel)base.Model; } 
            set { base.Model = value; }
        }

        public string Username { get; private set; }

        public string Slug { get; private set; }

        public RepositoryInfoController(string username, string slug,  string name)
            : base(typeof(RepositoryDetailedModel))
        {
            Username = username;
            Slug = slug;
            Title = name;

            _header = new HeaderView(View.Bounds.Width) { Title = name, ShadowImage = false };
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Images.Buttons.Gear, ShowExtraMenu));
            NavigationItem.RightBarButtonItem.Enabled = false;
        }

        public RepositoryInfoController(RepositoryDetailedModel model)
            : this(model.Owner, model.Slug, model.Name)
        {
            Model = model;
        }

        private void ShowExtraMenu()
        {
            var sheet = MonoTouch.Utilities.GetSheet(string.Empty);

            if (Application.Account.GetPinnedRepository(Model.Owner, Model.Slug) == null)
                sheet.AddButton("Pin to Slideout Menu".t());
            else
                sheet.AddButton("Unpin from Slideout Menu".t());

            //sheet.AddButton("Watch This Repo");
            sheet.AddButton("Fork Repository".t());
            sheet.AddButton("Show in Bitbucket".t());
            var cancelButton = sheet.AddButton("Cancel".t());
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Clicked += HandleSheetButtonClick;

            sheet.ShowInView(this.View);
        }

        void HandleSheetButtonClick (object sender, UIButtonEventArgs e)
        {
            if (Model == null)
                return;

            // Pin to menu
            if (e.ButtonIndex == 0)
            {
                //Is it pinned already or not?
                var pinnedRepo = Application.Account.GetPinnedRepository(Model.Owner, Model.Slug);
                if (pinnedRepo == null)
                    Application.Account.AddPinnedRepository(Model.Owner, Model.Slug, Model.Name, Model.Logo);
                else
                    Application.Account.RemovePinnedRepository(pinnedRepo.Id);
            }
            // Watch this repo
//            else if (e.ButtonIndex == 1)
//            {
//            }
            // Fork this repo
            else if (e.ButtonIndex == 1)
            {
                var alert = new UIAlertView();
                alert.Title = "Fork".t();
                alert.Message = "What would you like to name your fork?".t();
                alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
                var forkButton = alert.AddButton("Fork!".t());
                var cancelButton = alert.AddButton("Cancel".t());
                alert.CancelButtonIndex = cancelButton;
                alert.DismissWithClickedButtonIndex(cancelButton, true);
                alert.GetTextField(0).Text = Model.Name;
                alert.Clicked += (object sender2, UIButtonEventArgs e2) => {
                    if (e2.ButtonIndex == forkButton)
                    {
                        var text = alert.GetTextField(0).Text;
                        this.DoWork("Forking...".t(), () => {
                            var fork = Application.Client.Users[Model.Owner].Repositories[Model.Slug].ForkRepository(text);
                            BeginInvokeOnMainThread(() => {
                                NavigationController.PushViewController(new RepositoryInfoController(fork), true);
                            });
                        }, (ex) => {
                            //We typically get a 'BAD REQUEST' but that usually means that a repo with that name already exists
                            MonoTouch.Utilities.ShowAlert("Unable to fork".t(), "A repository by that name may already exist in your collection or an internal error has occured.".t());
                        });
                    }
                };

                alert.Show();
            }
            // Show in Bitbucket
            else if (e.ButtonIndex == 2)
            {
                try 
                {
                    UIApplication.SharedApplication.OpenUrl(new NSUrl("http://bitbucket.org/" + Model.Owner + "/" + Model.Slug));
                }
                catch { }
            }
        }


        protected override void OnRender()
        {
            var model = Model;
            Title = Model.Name;
            var root = new RootElement(Title) { UnevenRows = true };
            _header.Subtitle = "Updated ".t() + (model.UtcLastUpdated).ToDaysAgo();

            if (!string.IsNullOrEmpty(model.Logo))
                _header.Image = ImageLoader.DefaultRequestImage(new Uri(model.LargeLogo(64)), this);

            root.Add(new Section(_header));
            var sec1 = new Section();

            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrWhiteSpace(model.Description))
            {
                var element = new MultilinedElement(model.Description)
                {
                    BackgroundColor = UIColor.White
                };
                element.CaptionColor = element.ValueColor;
                element.CaptionFont = element.ValueFont;
                sec1.Add(element);
            }

            sec1.Add(new SplitElement(new SplitElement.Row
                                      {
                Text1 = model.Scm,
                Image1 = Images.ScmType,
                Text2 = model.Language,
                Image2 = Images.Language
            }));


            //Calculate the best representation of the size
            string size;
            if (model.Size / 1024f < 1)
                size = string.Format("{0}B", model.Size);
            else if ((model.Size / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}KB", model.Size / 1024f);
            else if ((model.Size / 1024f / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}MB", model.Size / 1024f / 1024f);
            else
                size = string.Format("{0:0.##}GB", model.Size / 1024f / 1024f / 1024f);


            sec1.Add(new SplitElement(new SplitElement.Row
                                      {
                Text1 = model.IsPrivate ? "Private".t() : "Public".t(),
                Image1 = model.IsPrivate ? Images.Locked : Images.Unlocked,
                Text2 = size,
                Image2 = Images.Size
            }));

            sec1.Add(new SplitElement(new SplitElement.Row
                                      {
                Text1 = (model.UtcCreatedOn).ToString("MM/dd/yy"),
                Image1 = Images.Create,
                Text2 = model.ForkCount.ToString() + (model.ForkCount == 1 ? " Fork".t() : " Forks".t()),
                Image2 = Images.Fork
            }));


            var owner = new StyledStringElement("Owner".t(), model.Owner) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            owner.Tapped += () => NavigationController.PushViewController(new ProfileController(model.Owner), true);
            sec1.Add(owner);
            var followers = new StyledStringElement("Followers".t(), "" + model.FollowersCount) { Accessory = UITableViewCellAccessory.DisclosureIndicator };
            followers.Tapped += () => NavigationController.PushViewController(new RepoFollowersController(model.Owner, model.Slug), true);
            sec1.Add(followers);


            var events = new StyledStringElement("Events".t(), () => NavigationController.PushViewController(new RepoEventsController(model.Owner, model.Slug), true), Images.Buttons.Event);

            var sec2 = new Section { events };

            if (model.HasIssues)
                sec2.Add(new StyledStringElement("Issues".t(), () => NavigationController.PushViewController(new IssuesController(model.Owner, model.Slug), true), Images.Buttons.Flag));

            if (model.HasWiki)
                sec2.Add(new StyledStringElement("Wiki".t(), () => NavigationController.PushViewController(new WikiInfoController(model.Owner, model.Slug), true), Images.Pencil));

            var sec3 = new Section
            {
                new StyledStringElement("Changes".t(), () => NavigationController.PushViewController(new ChangesetController(model.Owner, model.Slug), true), Images.Changes),
                new StyledStringElement("Branches".t(), () => NavigationController.PushViewController(new BranchController(model.Owner, model.Slug), true), Images.Branch),
                new StyledStringElement("Tags".t(), () => NavigationController.PushViewController(new TagController(model.Owner, model.Slug), true), Images.Tag)
            };

            root.Add(new[] { sec1, sec2, sec3 });

            if (!string.IsNullOrEmpty(model.Website))
            {
                var web = new StyledStringElement("Website".t(), () => UIApplication.SharedApplication.OpenUrl(NSUrl.FromString(model.Website)), Images.Webpage);
                root.Add(new Section { web });
            }

            Root = root;
            NavigationItem.RightBarButtonItem.Enabled = true;
        }

        protected override object OnUpdateModel(bool forced)
        {
            return Application.Client.Users[Username].Repositories[Slug].GetInfo(forced);
        }

        public void UpdatedImage(Uri uri)
        {
            _header.Image = ImageLoader.DefaultRequestImage(uri, this);
            if (_header.Image != null)
                _header.SetNeedsDisplay();
        }

    }
}