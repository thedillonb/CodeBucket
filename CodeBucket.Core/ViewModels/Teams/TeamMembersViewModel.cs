using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using BitbucketSharp;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamMembersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public TeamMembersViewModel(string name, IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Members";
            EmptyMessage = "There are no members.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                Users.Clear();
                return applicationService.Client.ForAllItems(x => x.Teams.GetMembers(name), 
                                                             x => Users.AddRange(x.Select(ToViewModel)));
            });
        }
    }
}

