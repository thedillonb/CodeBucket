using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.DialogElements;
using System;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Views.Issues
{
	public class IssueEditView : IssueModifyViewController<IssueEditViewModel>
    {
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var status = new ButtonElement("Status", ViewModel.Status, UITableViewCellStyle.Value1);
            var delete = new ButtonElement("Delete", AtlassianIcon.Delete.ToImage()) { Accessory = UITableViewCellAccessory.None };

            //Root[0].Insert(1, UITableViewRowAnimation.None, status);
            //Root.Insert(Root.Count, UITableViewRowAnimation.None, new Section { delete });

            OnActivation(d =>
            {
                ViewModel.WhenAnyValue(x => x.Status).Subscribe(x => status.Value = x).AddTo(d);
                delete.Clicked.BindCommand(ViewModel.DeleteCommand).AddTo(d);
                status.Clicked.Subscribe(_ =>
                {
                    var ctrl = new IssueAttributesViewController(
                        IssueAttributesViewController.Statuses, ViewModel.Status) { Title = "Status" };
                    ctrl.SelectedObservable
                        .Do(x => ViewModel.Status = x.ToLower())
                        .Subscribe(__ => NavigationController.PopToViewController(this, true));
                    NavigationController.PushViewController(ctrl, true);
                }).AddTo(d);
            });

		}
    }
}

