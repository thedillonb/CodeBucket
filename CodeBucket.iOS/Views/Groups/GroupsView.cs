using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;

namespace CodeBucket.Views.Groups
{
    public class GroupsView : ViewModelCollectionDrivenDialogViewController
	{
        public GroupsView()
        {
            Title = "Groups";
            NoItemsText = "No Groups";
        }

        public override void ViewDidLoad()
        {
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

