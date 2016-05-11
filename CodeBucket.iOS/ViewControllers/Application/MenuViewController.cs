using System;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.App;
using UIKit;
using System.Linq;
using CodeBucket.Core.Utils;
using CodeBucket.DialogElements;
using System.Collections.Generic;
using CodeBucket.ViewControllers.Accounts;
using CodeBucket.Core.Services;
using CoreGraphics;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Application
{
    public class MenuViewController : DialogViewController
    {
        private readonly ProfileButton _profileButton = new ProfileButton();
        private readonly UILabel _title;
		private Section _favoriteRepoSection;

        public MenuViewModel ViewModel { get; } = new MenuViewModel();

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title {
            get {
                return _title == null ? base.Title : " " + _title.Text;
            }
            set {
                if (_title != null)
                    _title.Text = " " + value;
                base.Title = value;
            }
        }

        public MenuViewController()
            : base(UITableViewStyle.Plain)
        {
            _title = new UILabel(new CGRect(0, 40, 320, 40));
            _title.TextAlignment = UITextAlignment.Left;
            _title.BackgroundColor = UIColor.Clear;
            _title.Font = UIFont.SystemFontOfSize(16f);
            _title.TextColor = UIColor.FromRGB(246, 246, 246);
            NavigationItem.TitleView = _title;

            OnActivation(d => d(_profileButton.GetClickedObservable().Subscribe(_ => ProfileButtonClicked())));
        }

	    private void CreateMenuRoot()
		{
            ICollection<Section> root = new LinkedList<Section>();

            root.Add(new Section
            {
                new MenuElement("Profile", () => ViewModel.GoToProfileCommand.Execute(null), AtlassianIcon.User.ToImage()),
            });

            var teamEvents = ViewModel.Teams
                .Where(_ => ViewModel.ShowTeamEvents)
                .Select(team => new MenuElement(team.Name, team.GoToCommand.ExecuteIfCan, AtlassianIcon.Blogroll.ToImage()));

            var eventsSection = new Section { HeaderView = new MenuSectionView("Events") };
            eventsSection.Add(new MenuElement(ViewModel.Username, () => ViewModel.GoToMyEvents.Execute(null), AtlassianIcon.Blogroll.ToImage()));
            eventsSection.AddAll(teamEvents);
            root.Add(eventsSection);

            var repoSection = new Section() { HeaderView = new MenuSectionView("Repositories") };
            repoSection.Add(new MenuElement("Owned", () => ViewModel.GoToOwnedRepositoriesCommand.Execute(null), AtlassianIcon.Devtoolsrepository.ToImage()));
            repoSection.Add(new MenuElement("Shared", () => ViewModel.GoToSharedRepositoriesCommand.Execute(null), AtlassianIcon.Spacedefault.ToImage()));
            repoSection.Add(new MenuElement("Watched", () => ViewModel.GoToStarredRepositoriesCommand.Execute(null), AtlassianIcon.Star.ToImage()));
            repoSection.Add(new MenuElement("Explore", () => ViewModel.GoToExploreRepositoriesCommand.Execute(null), AtlassianIcon.Search.ToImage()));
            root.Add(repoSection);
            
			if (ViewModel.PinnedRepositories.Any())
			{
				_favoriteRepoSection = new Section { HeaderView = new MenuSectionView("Favorite Repositories") };
                _favoriteRepoSection.AddAll(ViewModel.PinnedRepositories.Select(x => new PinnedRepoElement(x)));
				root.Add(_favoriteRepoSection);
			}
			else
			{
				_favoriteRepoSection = null;
			}

			if (ViewModel.ExpandTeamsAndGroups)
			{
                if (ViewModel.Groups.Count > 0)
                {
                    var groupsTeamsSection = new Section { HeaderView = new MenuSectionView("Groups") };
                    groupsTeamsSection.AddAll(ViewModel.Groups.Select(g => new MenuElement(g.Name, 
                        () => g.GoToCommand.Execute(null), AtlassianIcon.Group.ToImage())));
                    root.Add(groupsTeamsSection);
                }
                if (ViewModel.Teams.Count > 0)
                {
                    var groupsTeamsSection = new Section { HeaderView = new MenuSectionView("Teams") };
                    groupsTeamsSection.AddAll(ViewModel.Teams.Select(g => new MenuElement(g.Name,
                        () => g.GoToCommand.Execute(null), AtlassianIcon.Group.ToImage())));
                    root.Add(groupsTeamsSection);
                }
			}
			else
			{
                var groupsTeamsSection = new Section() { HeaderView = new MenuSectionView("Collaborations") };
                groupsTeamsSection.Add(new MenuElement("Groups", () => ViewModel.GoToGroupsCommand.Execute(null), AtlassianIcon.Group.ToImage()));
                groupsTeamsSection.Add(new MenuElement("Teams", () => ViewModel.GoToTeamsCommand.Execute(null), AtlassianIcon.Userstatus.ToImage()));
                root.Add(groupsTeamsSection);
			}

            var infoSection = new Section() { HeaderView = new MenuSectionView("Info & Preferences") };
            root.Add(infoSection);
            infoSection.Add(new MenuElement("Settings", () => ViewModel.GoToSettingsCommand.Execute(null), AtlassianIcon.Configure.ToImage()));
            infoSection.Add(new MenuElement("Feedback & Support", () => ViewModel.GoToFeedbackCommand.Execute(null), AtlassianIcon.Comment.ToImage()));
            infoSection.Add(new MenuElement("Accounts", ProfileButtonClicked, AtlassianIcon.User.ToImage()));
            Root.Reset(root);
		}

        private void ProfileButtonClicked()
        {
            var vc = new AccountsViewController();
            vc.NavigationItem.LeftBarButtonItem = new UIBarButtonItem { Image = Images.Buttons.Cancel };
            vc.NavigationItem.LeftBarButtonItem.Clicked += (sender, e) => DismissViewController(true, null);
            PresentViewController(new ThemedNavigationController(vc), true, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			TableView.SeparatorInset = UIEdgeInsets.Zero;
			TableView.SeparatorColor = UIColor.FromRGB(50, 50, 50);

            //Add some nice looking colors and effects
            TableView.TableFooterView = new UIView(new CGRect(0, 0, View.Bounds.Width, 0));
            TableView.BackgroundColor = UIColor.FromRGB(34, 34, 34);
            TableView.ScrollsToTop = false;

            _profileButton.Uri = ViewModel.Avatar.ToUri();
            ViewModel.WhenAnyValue(x => x.Groups).Subscribe(_ => CreateMenuRoot());
            ViewModel.WhenAnyValue(x => x.Teams).Subscribe(_ => CreateMenuRoot());
            ViewModel.LoadCommand.Execute(null);
        }

		private class PinnedRepoElement : MenuElement
		{
            public IReactiveCommand<object> DeleteCommand { get; }

            public PinnedRepoElement(PinnedRepositoryItemViewModel viewModel)
                : base(viewModel.Name, Images.RepoPlaceholder, viewModel.Avatar.ToUri())
			{
                Clicked.InvokeCommand(viewModel.GoToCommand);
                DeleteCommand = viewModel.DeleteCommand;
			}
		}

		private void DeletePinnedRepo(PinnedRepoElement el)
		{
            el.DeleteCommand.ExecuteIfCan();

			if (_favoriteRepoSection.Elements.Count == 1)
			{
				Root.Remove(_favoriteRepoSection);
				_favoriteRepoSection = null;
			}
			else
			{
				_favoriteRepoSection.Remove(el);
			}
		}

        public override Source CreateSizingSource()
		{
			return new EditSource(this);
		}


        public override void ViewWillAppear(bool animated)
        {
            UpdateProfilePicture();
            CreateMenuRoot();
            base.ViewWillAppear(animated);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            UpdateProfilePicture();
        }

        private void UpdateProfilePicture()
        {
            var size = new CGSize(32, 32);
            if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
            {
                size = new CGSize(24, 24);
            }

            _profileButton.Frame = new CGRect(new CGPoint(0, 4), size);

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(_profileButton);
        }


        private class EditSource : Source
		{
			private readonly MenuViewController _parent;
			public EditSource(MenuViewController dvc) 
				: base (dvc)
			{
				_parent = dvc;
			}

			public override bool CanEditRow(UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				if (_parent._favoriteRepoSection == null)
					return false;
				if (_parent.Root[indexPath.Section] == _parent._favoriteRepoSection)
					return true;
				return false;
			}

			public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
			{
				if (_parent._favoriteRepoSection != null && _parent.Root[indexPath.Section] == _parent._favoriteRepoSection)
					return UITableViewCellEditingStyle.Delete;
				return UITableViewCellEditingStyle.None;
			}

			public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
			{
				switch (editingStyle)
				{
					case UITableViewCellEditingStyle.Delete:
						var section = _parent.Root[indexPath.Section];
						var element = section[indexPath.Row];
						_parent.DeletePinnedRepo(element as PinnedRepoElement);
						break;
				}
			}
		}
    }
}

