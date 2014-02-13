using CodeFramework.ViewControllers;
using MonoTouch.Dialog;
using CodeBucket.Core.ViewModels.Teams;

namespace CodeBucket.iOS.Views.Teams
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