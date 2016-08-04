using System;
using CodeBucket.Client;
using CodeBucket.Core.Services;
using CodeBucket.DialogElements;
using CodeBucket.TableViewSources;
using CodeBucket.Utilities;
using ReactiveUI;
using Splat;
using UIKit;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestApproveViewController : TableViewController
    {
        private readonly Lazy<RootElement> _root;
        public IReactiveCommand<PullRequest> MergeCommand { get; }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { this.RaiseAndSetIfChanged(ref _message, value); }
        }

        private bool _deleteSourceBranch;
        public bool DeleteSourceBranch
        {
            get { return _deleteSourceBranch; }
            set { this.RaiseAndSetIfChanged(ref _deleteSourceBranch, value); }
        }

        protected RootElement Root => _root.Value;

        public PullRequestApproveViewController(string username, string repository, int pullRequestId,
                                                IApplicationService applicationService = null)
            : base(UITableViewStyle.Plain)
        {
            _root = new Lazy<RootElement>(() => new RootElement(TableView));

            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Approve";

            MergeCommand = ReactiveCommand.CreateAsyncTask(t =>
            {
                return applicationService.Client.PullRequests.Merge(
                    username, repository, pullRequestId, Message, DeleteSourceBranch);
            });

            MergeCommand.IsExecuting.SubscribeStatus("Merging...");

            var merge = new UIBarButtonItem(UIBarButtonSystemItem.Done);
            NavigationItem.RightBarButtonItem = merge;

            OnActivation(d =>
            {
                this.WhenAnyObservable(x => x.MergeCommand.CanExecuteObservable)
                    .Subscribe(x => merge.Enabled = x)
                    .AddTo(d);

                merge.GetClickedObservable()
                     .InvokeCommand(this, x => x.MergeCommand)
                     .AddTo(d);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var root = _root.Value;
            TableView.Source = new DialogTableViewSource(root);
            TableView.TableFooterView = new UIView();

            var deleteBranch = new BooleanElement("Delete Source Branch");
            var content = new ExpandingInputElement("Merge Message (Optional)");

            var contentSection = new Section { deleteBranch, content };
            root.Reset(contentSection);

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.Message)
                    .Subscribe(x => content.Value = x)
                    .AddTo(disposable);

                content
                    .Changed
                    .Subscribe(x => Message = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.DeleteSourceBranch)
                    .Subscribe(x => deleteBranch.Value = x)
                    .AddTo(disposable);

                deleteBranch
                    .Changed
                    .Subscribe(x => DeleteSourceBranch = x)
                    .AddTo(disposable);
            });

        }
    }
}

