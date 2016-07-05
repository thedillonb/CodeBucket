using CodeBucket.Core.ViewModels.Source;
using UIKit;
using System;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesAndTagsViewController : BaseViewController<BranchesAndTagsViewModel>
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var branchesViewController = new Lazy<UIViewController>(() =>
            {
                var vc = new BranchesViewController(ViewModel.Username, ViewModel.Repository);
                vc.ViewModel.LoadCommand.ExecuteIfCan();
                vc.View.Hidden = true;
                AddChildViewController(vc);
                View.Add(vc.View);
                return vc;
            });

             var tagsViewController = new Lazy<UIViewController>(() =>
            {
                var vc = new TagsViewController(ViewModel.Username, ViewModel.Repository);
                vc.ViewModel.LoadCommand.ExecuteIfCan();
                vc.View.Hidden = true;
                AddChildViewController(vc);
                View.Add(vc.View);
                return vc;
            });

			var viewSegment = new UISegmentedControl(new object[] {"Branches", "Tags"});
            NavigationItem.TitleView = viewSegment;

            OnActivation(disposable => 
            {
                this.WhenAnyValue(x => x.ViewModel.SelectedFilter)
                    .Do(x => viewSegment.SelectedSegment = x)
                    .Do(_ => View.EndEditing(true))
                    .Subscribe(x =>
                    {
                        if (x == 0)
                        {
                            branchesViewController.Value.View.Hidden = false;
                            if (tagsViewController.IsValueCreated)
                                tagsViewController.Value.View.Hidden = true;
                        }
                        else
                        {
                            tagsViewController.Value.View.Hidden = false;
                            if (branchesViewController.IsValueCreated)
                                branchesViewController.Value.View.Hidden = true;
                        }
                        
                    })
                    .AddTo(disposable);
                
                viewSegment.GetChangedObservable()
                    .Subscribe(x => ViewModel.SelectedFilter = x)
                    .AddTo(disposable);
            });
		}
	}
}

