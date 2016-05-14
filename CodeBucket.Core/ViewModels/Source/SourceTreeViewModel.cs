using System.Linq;
using System;
using System.Reactive.Linq;
using ReactiveUI;
using CodeBucket.Core.Services;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Source
{
    public class SourceTreeViewModel : BaseViewModel, ILoadableViewModel, IProvidesSearch, IListViewModel<SourceTreeItemViewModel>
    {
        public IReadOnlyReactiveList<SourceTreeItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

//		private void GoToSubmodule(SourceModel x)
//        {
//            var nameAndSlug = x.GitUrl.Substring(x.GitUrl.IndexOf("/repos/", System.StringComparison.Ordinal) + 7);
//            var repoId = new RepositoryIdentifier(nameAndSlug.Substring(0, nameAndSlug.IndexOf("/git", System.StringComparison.Ordinal)));
//            var sha = x.GitUrl.Substring(x.GitUrl.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
//            ShowViewModel<SourceTreeViewModel>(new NavObject {Username = repoId.Owner, Repository = repoId.Name, Branch = sha});
//        }

        public SourceTreeViewModel(
            string username, string repository, string branch, string path = null,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            branch = branch ?? "master";
            path = path ?? "";

            Title = string.IsNullOrEmpty(path) ? repository : path.Substring(path.LastIndexOf('/') + 1);

            var content = new ReactiveList<SourceTreeItemViewModel>();
            Items = content.CreateDerivedCollection(
                x => x,
                x => x.Name.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t =>
            {
                var data = await applicationService.Client.Repositories.GetSource(username, repository, branch, path);
                var dirs = data.Directories.Select(x =>
                {
                    var vm = new SourceTreeItemViewModel(x, SourceTreeItemViewModel.SourceTreeItemType.Directory);
                    vm.GoToCommand
                      .Select(_ => new SourceTreeViewModel(username, repository, branch, path + "/" + x))
                      .Subscribe(NavigateTo);
                    return vm;
                });

                var files = data.Files.Select(x =>
                {
                    var name = x.Path.Substring(x.Path.LastIndexOf("/", StringComparison.Ordinal) + 1);
                    var vm = new SourceTreeItemViewModel(name, SourceTreeItemViewModel.SourceTreeItemType.File);
                    vm.GoToCommand
                      .Select(_ => new SourceViewModel(username, repository, branch, x.Path, name))
                      .Subscribe(NavigateTo);
                    return vm;
                });

                content.Reset(dirs.Concat(files));
            });
        }
    }
}

