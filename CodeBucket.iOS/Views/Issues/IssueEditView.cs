using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeBucket.Core.ViewModels.Issues;

namespace CodeBucket.iOS.Views.Issues
{
	public class IssueEditView : IssueModifyView
    {
        public new IssueEditViewModel ViewModel
        {
            get { return (IssueEditViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

		public override void ViewDidLoad()
		{
			Title = "Edit Issue";

			base.ViewDidLoad();

            var status = new StyledStringElement("Status", ViewModel.Status, UITableViewCellStyle.Value1);
            status.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            status.Tapped += () => 
            {
                var ctrl = new IssueAttributesView(IssueModifyViewModel.Statuses, ViewModel.Status) { Title = "Status" };
                ctrl.SelectedValue = x => ViewModel.Status = x.ToLower();
                NavigationController.PushViewController(ctrl, true);
            };

            var delete = new StyledStringElement("Delete", () => ViewModel.DeleteCommand.Execute(null), Images.BinClosed) { BackgroundColor = UIColor.FromRGB(1.0f, 0.7f, 0.7f) };
            delete.Accessory = UITableViewCellAccessory.None;

            Root[0].Insert(1, UITableViewRowAnimation.None, status);
            Root.Insert(Root.Count, UITableViewRowAnimation.None, new Section { delete });

            ViewModel.Bind(x => x.Status, x => {
                status.Value = x;
                Root.Reload(status, UITableViewRowAnimation.None);
            }, true);
		}
    }
}

