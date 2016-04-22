using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using BitbucketSharp;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamMembersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string Name { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public TeamMembersViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                Users.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Teams.GetMembers(Name), 
                                                             x => Users.Items.AddRange(x.Select(ToViewModel)));
            });
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

