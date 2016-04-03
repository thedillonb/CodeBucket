using System;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public abstract class RepositoriesViewModel : BaseViewModel
    {
        public bool ShowRepositoryDescription { get; }

        public IReactiveCommand<object> GoToRepositoryCommand { get; }

        public CollectionViewModel<Repository> Repositories { get; } = new CollectionViewModel<Repository>();

        protected RepositoriesViewModel(IApplicationService applicationService)
        {
            ShowRepositoryDescription = applicationService.Account.RepositoryDescriptionInList;

            GoToRepositoryCommand = ReactiveCommand.Create();
            GoToRepositoryCommand.OfType<Repository>().Subscribe(x => {
                var id = new RepositoryIdentifier(x.FullName);
                var obj = new RepositoryViewModel.NavObject { Username = id.Owner, RepositorySlug = id.Name };
                ShowViewModel<RepositoryViewModel>(obj);
            });
        }
    }
}