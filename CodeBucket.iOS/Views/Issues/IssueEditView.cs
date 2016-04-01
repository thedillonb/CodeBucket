using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.DialogElements;
using System;

namespace CodeBucket.Views.Issues
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

            var status = new StringElement("Status", ViewModel.Status, UITableViewCellStyle.Value1);
            var delete = new StringElement("Delete", AtlassianIcon.Delete.ToImage()) { Accessory = UITableViewCellAccessory.None };

            Root[0].Insert(1, UITableViewRowAnimation.None, status);
            Root.Insert(Root.Count, UITableViewRowAnimation.None, new Section { delete });

            OnActivation(d =>
            {
                d(ViewModel.Bind(x => x.Status, true).Subscribe(x => status.Value = x));
                d(delete.Clicked.BindCommand(ViewModel.DeleteCommand));
                d(status.Clicked.Subscribe(_ =>
                {
                    var ctrl = new IssueAttributesView(IssueModifyViewModel.Statuses, ViewModel.Status) { Title = "Status" };
                    ctrl.SelectedValue = x => ViewModel.Status = x.ToLower();
                    NavigationController.PushViewController(ctrl, true);
                }));
            });

		}
    }
}

