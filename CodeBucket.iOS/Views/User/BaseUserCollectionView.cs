using CodeFramework.iOS.Elements;
using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels.User;

namespace CodeBucket.iOS.Views.User
{
    public abstract class BaseUserCollectionView : ViewModelCollectionDrivenViewController
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

