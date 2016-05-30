using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.TableViewSources;
using ReactiveUI;
using System;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssuesViewController : BaseTableViewController<IssuesViewModel, IssueItemViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var viewSegment = new CustomUISegmentedControl(new [] { "All", "Open", "Mine", "Custom" }, 3);
            var segmentBarButton = new UIBarButtonItem(viewSegment);
			segmentBarButton.Width = View.Frame.Width - 10f;

            ToolbarItems = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                segmentBarButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };

            TableView.Source = new IssueTableViewSource(TableView, ViewModel.Items);

            var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            NavigationItem.RightBarButtonItem = addButton;

            OnActivation(disposable =>
            {
                addButton
                    .GetClickedObservable()
                    .InvokeCommand(ViewModel.GoToNewIssueCommand)
                    .AddTo(disposable);

                viewSegment
                    .GetChangedObservable()
                    .Subscribe(x => ViewModel.SelectedFilter = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.SelectedFilter)
                    .Subscribe(x => viewSegment.SelectedSegment = x)
                    .AddTo(disposable);
            });
        }

        protected override void Navigate(UIViewController viewController)
        {
            base.Navigate(viewController);
        }

        public override void ViewWillAppear(bool animated)
        {
			base.ViewWillAppear(animated);
            NavigationController.SetToolbarHidden(false, animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, animated);
        }

        private class CustomUISegmentedControl : UISegmentedControl
        {
            readonly int _multipleTouchIndex;
            public CustomUISegmentedControl(object[] args, int multipleTouchIndex)
                : base(args)
            {
                this._multipleTouchIndex = multipleTouchIndex;
            }

            public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
            {
                var previousSelected = SelectedSegment;
                base.TouchesEnded(touches, evt);
                if (previousSelected == SelectedSegment && SelectedSegment == _multipleTouchIndex)
                    SendActionForControlEvents(UIControlEvent.ValueChanged);
            }
        }
    }
}

