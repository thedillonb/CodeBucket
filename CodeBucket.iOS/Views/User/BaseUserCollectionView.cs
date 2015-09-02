using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.User;
using CodeBucket.ViewControllers;

namespace CodeBucket.Views.User
{
    public abstract class BaseUserCollectionView : ViewModelCollectionDrivenDialogViewController
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = (BaseUserCollectionViewModel)ViewModel;
            BindCollection(vm.Users, x =>
            {
				var e = new UserElement(x.Username, string.Empty, string.Empty, x.Avatar);
                e.Tapped += () => vm.GoToUserCommand.Execute(x);
                return e;
            });
        }
    }
}

