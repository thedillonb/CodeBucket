using System;
using BitbucketSharp.Models;
using CodeBucket.Core.ViewModels.Commits;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Source
{
    public class BranchesViewModel : BaseViewModel, ILoadableViewModel
    {
        public IReadOnlyReactiveList<ReferenceItemViewModel> Branches { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public BranchesViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Branches";

            var branches = new ReactiveList<ReferenceModel>();
            Branches = branches.CreateDerivedCollection(branch =>
            {
                var vm = new ReferenceItemViewModel(branch.Branch);
                vm.GoToCommand
                  .Select(_ => new CommitsViewModel(username, repository, branch.Node))
                  .Subscribe(NavigateTo);
                return vm;
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var items = await applicationService.Client.Repositories.GetBranches(username, repository);
                branches.Reset(items.Values);
            });
        }
    }
}

