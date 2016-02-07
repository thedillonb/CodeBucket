using CodeBucket.ViewControllers;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.App;
using UIKit;
using System.Linq;
using CodeBucket.Elements;
using CodeBucket.Core.Utils;
using CodeBucket.Views.Accounts;

namespace CodeBucket.Views.App
{
	public class MenuView : MenuBaseViewController
    {
		private Section _favoriteRepoSection;

	    public new MenuViewModel ViewModel
	    {
	        get { return (MenuViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
	    }

	    protected override void CreateMenuRoot()
		{
            var username = ViewModel.Account.Username;
			Title = username;
            var root = new RootElement(username);

            root.Add(new Section
            {
                new MenuElement("Profile", () => ViewModel.GoToProfileCommand.Execute(null), AtlassianIcon.User.ToImage()),
            });

            var eventsSection = new Section { HeaderView = new MenuSectionView("Events") };
            eventsSection.Add(new MenuElement(username, () => ViewModel.GoToMyEvents.Execute(null), AtlassianIcon.Blogroll.ToImage()));
			if (ViewModel.Teams != null && ViewModel.Account.ShowTeamEvents)
                ViewModel.Teams.ForEach(team => eventsSection.Add(new MenuElement(team, () => ViewModel.GoToTeamEventsCommand.Execute(team), AtlassianIcon.Blogroll.ToImage())));
            root.Add(eventsSection);

            var repoSection = new Section() { HeaderView = new MenuSectionView("Repositories") };
            repoSection.Add(new MenuElement("Owned", () => ViewModel.GoToOwnedRepositoriesCommand.Execute(null), AtlassianIcon.Devtoolsrepository.ToImage()));
            repoSection.Add(new MenuElement("Shared", () => ViewModel.GoToSharedRepositoriesCommand.Execute(null), AtlassianIcon.Spacedefault.ToImage()));
            repoSection.Add(new MenuElement("Watched", () => ViewModel.GoToStarredRepositoriesCommand.Execute(null), AtlassianIcon.Star.ToImage()));
            repoSection.Add(new MenuElement("Explore", () => ViewModel.GoToExploreRepositoriesCommand.Execute(null), AtlassianIcon.Search.ToImage()));
            root.Add(repoSection);
            
			if (ViewModel.PinnedRepositories.Any())
			{
				_favoriteRepoSection = new Section() { HeaderView = new MenuSectionView("Favorite Repositories") };
				foreach (var pinnedRepository in ViewModel.PinnedRepositories)
					_favoriteRepoSection.Add(new PinnedRepoElement(pinnedRepository, ViewModel.GoToRepositoryCommand));
				root.Add(_favoriteRepoSection);
			}
			else
			{
				_favoriteRepoSection = null;
			}

			if (ViewModel.Account.ExpandTeamsAndGroups)
			{
                if (ViewModel.Groups?.Count > 0)
                {
                    var groupsTeamsSection = new Section { HeaderView = new MenuSectionView("Groups") };
                    ViewModel.Groups.ForEach(x => groupsTeamsSection.Add(new MenuElement(x.Name, () => ViewModel.GoToGroupCommand.Execute(x), AtlassianIcon.Group.ToImage())));
                    root.Add(groupsTeamsSection);
                }
                if (ViewModel.Teams?.Count > 0)
                {
                    var groupsTeamsSection = new Section { HeaderView = new MenuSectionView("Teams") };
                    ViewModel.Teams.ForEach(x => groupsTeamsSection.Add(new MenuElement(x, () => ViewModel.GoToTeamCommand.Execute(x), AtlassianIcon.Userstatus.ToImage())));
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
            infoSection.Add(new MenuElement("Accounts", () => ProfileButtonClicked(this, System.EventArgs.Empty), AtlassianIcon.User.ToImage()));
            Root = root;
		}

        protected override void ProfileButtonClicked(object sender, System.EventArgs e)
        {
            PresentViewController(new UINavigationController(new AccountsView()), true, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			TableView.SeparatorInset = UIEdgeInsets.Zero;
			TableView.SeparatorColor = UIColor.FromRGB(50, 50, 50);

			ProfileButton.Uri = new System.Uri(ViewModel.Account.AvatarUrl);
			ViewModel.Bind(x => x.Groups, CreateMenuRoot);
			ViewModel.Bind(x => x.Teams, CreateMenuRoot);
            ViewModel.LoadCommand.Execute(null);
        }

		private class PinnedRepoElement : MenuElement
		{
			public CodeFramework.Core.Data.PinnedRepository PinnedRepo
			{
				get;
				private set; 
			}

            protected override string GetKey(int style)
            {
                return "pinned-repository";
            }

            protected override void OnCellCreated(UITableViewCell cell)
            {
                base.OnCellCreated(cell);
                cell.ImageView.Layer.MasksToBounds = true;
            }

            public override UITableViewCell GetCell(UITableView tv)
            {
                var cell = base.GetCell(tv);
                cell.ImageView.Layer.MasksToBounds = true;
                cell.ImageView.Layer.CornerRadius = cell.ImageView.Bounds.Height / 2f;
                return cell;
            }
    
			public PinnedRepoElement(CodeFramework.Core.Data.PinnedRepository pinnedRepo, System.Windows.Input.ICommand command)
                : base(pinnedRepo.Name, () => command.Execute(new RepositoryIdentifier { Owner = pinnedRepo.Owner, Name = pinnedRepo.Slug }), Images.RepoPlaceholder)
			{
				PinnedRepo = pinnedRepo;
				ImageUri = new System.Uri(PinnedRepo.ImageUri);
			}
		}

		private void DeletePinnedRepo(PinnedRepoElement el)
		{
			ViewModel.DeletePinnedRepositoryCommand.Execute(el.PinnedRepo);

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

		public override DialogViewController.Source CreateSizingSource(bool unevenRows)
		{
			return new EditSource(this);
		}

		private class EditSource : SizingSource
		{
			private readonly MenuView _parent;
			public EditSource(MenuView dvc) 
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

