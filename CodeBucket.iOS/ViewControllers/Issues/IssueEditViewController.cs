using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.DialogElements;
using System;
using ReactiveUI;
using System.Reactive.Linq;
using Humanizer;

namespace CodeBucket.ViewControllers.Issues
{
	public class IssueEditViewController : IssueModifyViewController<IssueEditViewModel>
    {
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var status = new ButtonElement("Status", ViewModel.Status, UITableViewCellStyle.Value1);
            Root[0].Insert(1, UITableViewRowAnimation.None, status);

            OnActivation(d =>
            {
                this.WhenAnyValue(x => x.ViewModel.Status)
                    .Subscribe(x => status.Value = x.Humanize(LetterCasing.Title))
                    .AddTo(d);
                
                status.Clicked.Subscribe(_ =>
                {
                    var ctrl = new IssueAttributesViewController(
                        IssueAttributesViewController.Statuses, ViewModel.Status) { Title = "Status" };
                    ctrl.SelectedObservable
                        .Do(x => ViewModel.Status = x)
                        .Subscribe(__ => NavigationController.PopToViewController(this, true));
                    NavigationController.PushViewController(ctrl, true);
                }).AddTo(d);
            });

		}
    }
}

