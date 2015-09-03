using System;
using CodeBucket.ViewControllers;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.Repositories;
using UIKit;
using BitbucketSharp.Models;
using CodeBucket.Elements;
using Humanizer;

namespace CodeBucket.Views.Repositories
{
	public class RepositoryView : ViewModelDrivenDialogViewController
    {
		private readonly HeaderView _header = new HeaderView();

        public new RepositoryViewModel ViewModel
        {
            get { return (RepositoryViewModel)base.ViewModel; }
            protected set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

//            _header.Image = Images.RepoPlaceholder;

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
            NavigationItem.RightBarButtonItem.Enabled = false;

            ViewModel.Bind(x => x.Repository, x =>
            {
				ViewModel.ImageUrl = x.LargeLogo(64);
                NavigationItem.RightBarButtonItem.Enabled = true;
                Render(x);
            });

            ViewModel.Bind(x => x.HasReadme, () =>
            {
                // Not very efficient but it'll work for now.
                if (ViewModel.Repository != null)
                    Render(ViewModel.Repository);
            });
        }
		
        private void ShowExtraMenu()
        {
            var repoModel = ViewModel.Repository;
            if (repoModel == null)
                return;

            var sheet = MonoTouch.Utilities.GetSheet(repoModel.Name);
			var pinButton = sheet.AddButton(ViewModel.IsPinned ? "Unpin from Slideout Menu" : "Pin to Slideout Menu");
            var forkButton = sheet.AddButton("Fork Repository");
			var showButton = sheet.AddButton("Show in Bitbucket");
            var cancelButton = sheet.AddButton("Cancel");
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (s, e) => {
                BeginInvokeOnMainThread(() =>
                {
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
            };

            sheet.ShowInView(this.View);
        }

		public void Render(RepositoryDetailedModel model)
        {
			Title = model.Name;
            var root = new RootElement(Title) { UnevenRows = true };
			_header.Title = Title;
            _header.Subtitle = "Updated " + model.UtcLastUpdated.Humanize();
			_header.ImageUri = ViewModel.ImageUrl;

            root.Add(new Section(_header));
            var sec1 = new Section();

            if (!string.IsNullOrEmpty(model.Description) && !string.IsNullOrWhiteSpace(model.Description))
            {
                var element = new MultilinedElement(model.Description)
                {
                    BackgroundColor = UIColor.White,
                    CaptionColor = Theme.CurrentTheme.MainTitleColor, 
                    ValueColor = Theme.CurrentTheme.MainTextColor
                };
                element.CaptionColor = element.ValueColor;
                element.CaptionFont = element.ValueFont;
                sec1.Add(element);
            }

            sec1.Add(new SplitElement(new SplitElement.Row {
				Text1 = model.IsPrivate ? "Private" : "Public",
				Image1 = model.IsPrivate ? Images.Locked : Images.Unlocked,
				Text2 = string.IsNullOrEmpty(model.Language) ? "N/A" : model.Language,
                Image2 = Images.Language
            }));


            //Calculate the best representation of the size
            string size;
            if (model.Size / 1024f < 1)
                size = string.Format("{0:0.##}B", model.Size);
            else if ((model.Size / 1024f / 1024f) < 1)
                size = string.Format("{0:0.##}KB", model.Size / 1024f);
            else
                size = string.Format("{0:0.##}MB", model.Size / 1024f / 1024f);
//
//            sec1.Add(new SplitElement(new SplitElement.Row {
//				Text1 = model + (model.HasIssues == 1 ? " Issue" : " Issues"),
//                Image1 = Images.Flag,
//				Text2 = model.ForkCount.ToString() + (model.ForkCount == 1 ? " Fork" : " Forks"),
//                Image2 = Images.Fork
//            }));
//
            sec1.Add(new SplitElement(new SplitElement.Row {
				Text1 = (model.UtcCreatedOn).ToString("MM/dd/yy"),
                Image1 = Images.Create,
                Text2 = size,
                Image2 = Images.Size
            }));

            var owner = new StyledStringElement("Owner", model.Owner) { Image = Images.Person,  Accessory = UITableViewCellAccessory.DisclosureIndicator };
			owner.Tapped += () => ViewModel.GoToOwnerCommand.Execute(null);
            sec1.Add(owner);

			if (model.ForkOf != null)
            {
				var parent = new StyledStringElement("Forked From", model.ForkOf.Name) { Image = Images.Fork,  Accessory = UITableViewCellAccessory.DisclosureIndicator };
				parent.Tapped += () => ViewModel.GoToForkParentCommand.Execute(model.ForkOf);
                sec1.Add(parent);
            }

			var followers = new StyledStringElement("Watchers", "" + model.FollowersCount) { Image = Images.Star, Accessory = UITableViewCellAccessory.DisclosureIndicator };
			followers.Tapped += () => ViewModel.GoToStargazersCommand.Execute(null);
            sec1.Add(followers);

			var events = new StyledStringElement("Events", () => ViewModel.GoToEventsCommand.Execute(null), Images.Event);
            var sec2 = new Section { events };

			if (model.HasWiki)
				sec2.Add(new StyledStringElement("Wiki", () => ViewModel.GoToWikiCommand.Execute(null), Images.Pencil));

            if (model.HasIssues)
				sec2.Add(new StyledStringElement("Issues", () => ViewModel.GoToIssuesCommand.Execute(null), Images.Flag));

            if (ViewModel.HasReadme)
                sec2.Add(new StyledStringElement("Readme", () => ViewModel.GoToReadmeCommand.Execute(null), Images.File));

            var sec3 = new Section
            {
				new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null), Images.Commit),
				new StyledStringElement("Pull Requests", () => ViewModel.GoToPullRequestsCommand.Execute(null), Images.Hand),
				new StyledStringElement("Source", () => ViewModel.GoToSourceCommand.Execute(null), Images.Script),
            };

            root.Add(new[] { sec1, sec2, sec3 });

            if (!String.IsNullOrEmpty(ViewModel.Repository.Website))
            {
                root.Add(new Section
                {
                    new StyledStringElement("Website", () => ViewModel.GoToUrlCommand.Execute(ViewModel.Repository.Website), Images.Webpage)
                });
            }

            Root = root;
        }
    }
}