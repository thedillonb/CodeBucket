using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;

namespace CodeBucket.Views.Teams
{
    public class TeamsView : ViewModelCollectionDrivenDialogViewController
    {
        public override void ViewDidLoad()
        {
            Title = "Teams";
            NoItemsText = "No Teams";

            base.ViewDidLoad();

            var vm = (TeamsViewModel) ViewModel;
			this.BindCollection(vm.Teams, x => new StyledStringElement(x, () => vm.GoToTeamCommand.Execute(x)));
        }
    }
}