using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels;
using MonoTouch.Dialog;

namespace CodeBucket.iOS.Views.User
{
    public class TeamsView : ViewModelCollectionDrivenDialogViewController
    {
        public override void ViewDidLoad()
        {
            Title = "Teams".t();
            NoItemsText = "No Teams".t();

            base.ViewDidLoad();

            var vm = (TeamsViewModel) ViewModel;
			this.BindCollection(vm.Teams, x => new StyledStringElement(x, () => vm.GoToTeamCommand.Execute(x)));
        }
    }
}