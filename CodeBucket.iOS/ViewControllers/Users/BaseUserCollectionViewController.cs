using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Users;
using UIKit;
using System;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.User
{
    public abstract class BaseUserCollectionViewController : ViewModelCollectionDrivenDialogViewController
    {
        protected BaseUserCollectionViewController(string emptyString)
        {
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.User.ToEmptyListImage(), emptyString ?? "There are no users."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = (BaseUserCollectionViewModel)ViewModel;
            BindCollection(vm.Users, x => new UserElement(x));
        }
    }
}

