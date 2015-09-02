using CodeBucket.Core.ViewModels.Source;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;

namespace CodeBucket.Views.Source
{
    public class ChangesetBranchesView : ViewModelCollectionDrivenDialogViewController
    {
        public override void ViewDidLoad()
        {
            Title = "Changeset Branch";
            NoItemsText = "No Branches";

            base.ViewDidLoad();

            var vm = (ChangesetBranchesViewModel) ViewModel;
            BindCollection(vm.Branches, x => new StyledStringElement(x.Name, () => vm.GoToBranchCommand.Execute(x)));
        }
    }
}

