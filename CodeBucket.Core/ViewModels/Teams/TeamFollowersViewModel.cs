using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamFollowersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string Name { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public TeamFollowersViewModel(IApplicationService applicationService)
        {
            Title = "Followers";
            EmptyMessage = "There are no followers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(t =>
            {
                Users.Clear();
                return applicationService.Client
                    .ForAllItems(x => x.Teams.GetFollowers(Name),
                                 x => Users.AddRange(x.Select(ToViewModel)));
            });
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Username;
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}

