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
        protected readonly IReactiveList<Repository> RepositoryList = new ReactiveList<Repository>();

        public IReactiveCommand<object> GoToRepositoryCommand { get; }

        public IReadOnlyReactiveList<RepositoryItemViewModel> Repositories { get; }

        protected RepositoriesViewModel(IApplicationService applicationService)
        {
            var showDescription = applicationService.Account.RepositoryDescriptionInList;
            Repositories = RepositoryList.CreateDerivedCollection(x =>
            {
                var description = showDescription ? x.Description : string.Empty;
                return new RepositoryItemViewModel(x.Name, description, x.Owner?.Username, new Avatar(x.Owner?.Links?.Avatar?.Href));
            });

            GoToRepositoryCommand = ReactiveCommand.Create();
            GoToRepositoryCommand.OfType<Repository>().Subscribe(x =>
            {
                var id = new RepositoryIdentifier(x.FullName);
                var obj = new RepositoryViewModel.NavObject { Username = id.Owner, RepositorySlug = id.Name };
                ShowViewModel<RepositoryViewModel>(obj);
            });
        }
    }
}