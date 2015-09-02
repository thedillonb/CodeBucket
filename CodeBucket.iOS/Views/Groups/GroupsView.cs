using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;

namespace CodeBucket.Views.Groups
{
    public class GroupsView : ViewModelCollectionDrivenDialogViewController
	{
        public override void ViewDidLoad()
        {
            Title = "Groups";
            NoItemsText = "No Groups";

            base.ViewDidLoad();

			var vm = (GroupsViewModel) ViewModel;
			BindCollection(vm.Organizations, x =>
			{
				var e = new StyledStringElement(x.Name);
				e.Tapped += () => vm.GoToGroupCommand.Execute(x);
				return e;
			});
        }
	}
}

