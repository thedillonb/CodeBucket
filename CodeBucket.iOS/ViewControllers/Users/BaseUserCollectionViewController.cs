using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Users;
using UIKit;
using System;
using CodeBucket.Views;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels;

namespace CodeBucket.ViewControllers.User
{
    public abstract class BaseUserCollectionViewController : ViewModelDrivenDialogViewController
    {
        protected BaseUserCollectionViewController()
            : base(UITableViewStyle.Plain)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = (BaseUserCollectionViewModel)ViewModel;

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.User.ToEmptyListImage(), vm.EmptyMessage ?? "There are no users."));

            vm.Users.ChangedObservable()
              .Select(users => users.Select(y => new UserElement(y)))
              .Subscribe(elements => Root.Reset(new Section { elements }));

            vm.Bind(x => x.Title, true)
              .Subscribe(x => Title = x);

            Appeared.Take(1)
                    .Select(_ => ViewModel)
                    .OfType<ILoadableViewModel>()
                    .Select(x => x.LoadCommand.IsExecuting)
                    .Switch()
                    .Where(x => x == false)
                    .Select(_ => !vm.Users.Any())
                    .DistinctUntilChanged()
                    .Subscribe(TableView.SetEmpty);
        }
    }
}

