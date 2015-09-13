using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;

namespace CodeBucket.Views.Teams
{
    public class TeamsView : ViewModelCollectionDrivenDialogViewController
    {
        public TeamsView()
        {
            Title = "Teams";
            NoItemsText = "No Teams";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var vm = (TeamsViewModel) ViewModel;
			BindCollection(vm.Teams, x => new StyledStringElement(x, () => vm.GoToTeamCommand.Execute(x)));
        }
    }
}