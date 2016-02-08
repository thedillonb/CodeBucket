using System;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Repositories;
using UIKit;
using BitbucketSharp.Models;
using CodeBucket.Elements;
using Humanizer;
using CodeBucket.Core.Utils;
using CodeBucket.DialogElements;

namespace CodeBucket.Views.Repositories
{
    public class RepositoryView : PrettyDialogViewController
    {

        public new RepositoryViewModel ViewModel
        {
            get { return (RepositoryViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.SetImage(null, Images.RepoPlaceholder);

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem.Enabled = false;

            ViewModel.Bind(x => x.Repository, x =>
            {
                NavigationItem.RightBarButtonItem.Enabled = true;
                Render(x);
            });

            ViewModel.Bind(x => x.Branches, () => Render(ViewModel.Repository));
            ViewModel.Bind(x => x.Issues, () => Render(ViewModel.Repository));
            ViewModel.Bind(x => x.HasReadme, () => Render(ViewModel.Repository));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationItem.RightBarButtonItem.Clicked += ShowExtraMenu;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            NavigationItem.RightBarButtonItem.Clicked -= ShowExtraMenu;
        }
		
        private void ShowExtraMenu(object o, EventArgs args)
        {
            var repoModel = ViewModel.Repository;
            if (repoModel == null)
                return;

            var sheet = new UIActionSheet();
			var pinButton = sheet.AddButton(ViewModel.IsPinned ? "Unpin from Slideout Menu" : "Pin to Slideout Menu");
            var forkButton = sheet.AddButton("Fork Repository");
			var showButton = sheet.AddButton("Show in Bitbucket");
            var cancelButton = sheet.AddButton("Cancel");
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (s, e) => {
                BeginInvokeOnMainThread(() => {
                // Pin to menu
                if (e.ButtonIndex == pinButton)
                {
                    ViewModel.PinCommand.Execute(null);
                }
                else if (e.ButtonIndex == forkButton)
                {
                    ViewModel.ForkCommand.Execute(null);
                }
                // Show in Bitbucket
                else if (e.ButtonIndex == showButton)
                {
					ViewModel.GoToUrlCommand.Execute(ViewModel.HtmlUrl);
                }
                });

                sheet.Dispose();
            };

            sheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
        }

        SplitViewElement _split1 = new SplitViewElement(AtlassianIcon.Locked.ToImage(), AtlassianIcon.Pagedefault.ToImage());
        SplitViewElement _split2 = new SplitViewElement(AtlassianIcon.Calendar.ToImage(), AtlassianIcon.Filezip.ToImage());
        SplitViewElement _split3 = new SplitViewElement(AtlassianIcon.Devtoolsrepository.ToImage(), AtlassianIcon.Devtoolsbranch.ToImage());

		public void Render(RepositoryDetailedModel model)
        {
            if (model == null)
                return;
            
			Title = model.Name;

            var avatar = new Avatar(model.Logo).ToUrl(128);
            var root = new RootElement(Title) { UnevenRows = true };
            HeaderView.SubText = string.IsNullOrWhiteSpace(model.Description) ? "Updated " + model.UtcLastUpdated.Humanize() : model.Description;
            HeaderView.SetImage(avatar, Images.RepoPlaceholder);
            RefreshHeaderView();

            var stargazersCommand = ViewModel.GoToStargazersCommand;
            var split = new SplitButtonElement();
            split.AddButton("Watchers", model.FollowersCount.ToString(), () => stargazersCommand.Execute(null));
            split.AddButton("Forks", model.ForkCount.ToString());
            split.AddButton("Branches", ViewModel.Branches?.Count.ToString() ?? "-");

            var sec1 = new Section();

            _split1.Button1.Image = model.IsPrivate ? AtlassianIcon.Locked.ToImage() : AtlassianIcon.Unlocked.ToImage();
            _split1.Button1.Text = model.IsPrivate ? "Private" : "Public";
            _split1.Button2.Text = string.IsNullOrEmpty(model.Language) ? "N/A" : model.Language;
            sec1.Add(_split1);

            _split3.Button1.Text = model.Scm.ApplyCase(LetterCasing.Title);
            _split3.Button2.Text = "Issues".ToQuantity(ViewModel.Issues);
            sec1.Add(_split3);

            _split2.Button1.Text = (model.UtcCreatedOn).ToString("MM/dd/yy");
            _split2.Button2.Text = model.Size.Bytes().ToString("#.##");
            sec1.Add(_split2);

            var owner = new StyledStringElement("Owner", model.Owner) { Image = AtlassianIcon.User.ToImage(),  Accessory = UITableViewCellAccessory.DisclosureIndicator };
			owner.Tapped += () => ViewModel.GoToOwnerCommand.Execute(null);
            sec1.Add(owner);

			if (model.ForkOf != null)
            {
                var parent = new StyledStringElement("Forked From", model.ForkOf.Name) { Image = AtlassianIcon.Devtoolsfork.ToImage(),  Accessory = UITableViewCellAccessory.DisclosureIndicator };
				parent.Tapped += () => ViewModel.GoToForkParentCommand.Execute(model.ForkOf);
                sec1.Add(parent);
            }

            var events = new StyledStringElement("Events", () => ViewModel.GoToEventsCommand.Execute(null), AtlassianIcon.Blogroll.ToImage());
            var sec2 = new Section { events };

			if (model.HasWiki)
                sec2.Add(new StyledStringElement("Wiki", () => ViewModel.GoToWikiCommand.Execute(null), AtlassianIcon.Edit.ToImage()));

            if (model.HasIssues)
                sec2.Add(new StyledStringElement("Issues", () => ViewModel.GoToIssuesCommand.Execute(null), AtlassianIcon.Flag.ToImage()));

            if (ViewModel.HasReadme)
                sec2.Add(new StyledStringElement("Readme", () => ViewModel.GoToReadmeCommand.Execute(null), AtlassianIcon.Pagedefault.ToImage()));

            var sec3 = new Section
            {
                new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null), AtlassianIcon.Devtoolscommit.ToImage()),
                new StyledStringElement("Pull Requests", () => ViewModel.GoToPullRequestsCommand.Execute(null), AtlassianIcon.Devtoolspullrequest.ToImage()),
                new StyledStringElement("Source", () => ViewModel.GoToSourceCommand.Execute(null), AtlassianIcon.Filecode.ToImage()),
            };

            root.Add(new[] { new Section { split }, sec1, sec2, sec3 });

            if (!String.IsNullOrEmpty(ViewModel.Repository.Website))
            {
                root.Add(new Section
                {
                    new StyledStringElement("Website", () => ViewModel.GoToUrlCommand.Execute(ViewModel.Repository.Website), AtlassianIcon.Homepage.ToImage())
                });
            }

            Root = root;
        }
    }
}