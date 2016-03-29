using System;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesViewModel : BaseViewModel, ILoadableViewModel
    {
        public string Username { get; private set; }

        public string Repository { get; private set; }

        public CollectionViewModel<BranchModel> Branches { get; } = new CollectionViewModel<BranchModel>();

        public IReactiveCommand<object> GoToBranchCommand { get; }

        public IReactiveCommand LoadCommand { get; }

        public BranchesViewModel(IApplicationService applicationService)
        {
            GoToBranchCommand = ReactiveCommand.Create();
            GoToBranchCommand.OfType<BranchModel>()
                .Select(x => new CommitsViewModel.NavObject { Username = Username, Repository = Repository, Branch = x.Node })
                .Subscribe(x => ShowViewModel<CommitsViewModel>(x));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var branches = await applicationService.Client.Repositories.GetBranches(Username, Repository);
                Branches.Items.Reset(branches.Values);
            });
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}

