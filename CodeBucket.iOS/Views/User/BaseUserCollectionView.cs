using CodeBucket.DialogElements;
using CodeBucket.ViewControllers;
using CodeBucket.Core.Utils;
using UIKit;
using System;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Views.User
{
    public abstract class BaseUserCollectionView : ViewModelCollectionDrivenDialogViewController
    {
        protected BaseUserCollectionView(string emptyString)
        {
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.User.ToEmptyListImage(), emptyString ?? "There are no users."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = (BaseUserCollectionViewModel)ViewModel;
            var weakVm = new WeakReference<BaseUserCollectionViewModel>(vm);
            BindCollection(vm.Users, x =>
            {
                var e = new UserElement(x.Username, string.Empty, string.Empty, new Avatar(x.Avatar));
                e.Clicked.Subscribe(_ => weakVm.Get()?.GoToUserCommand.Execute(x));
                return e;
            });
        }
    }
}

