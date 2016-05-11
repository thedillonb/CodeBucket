using CodeBucket.Core.ViewModels.Source;
using CodeBucket.DialogElements;
using System;
using UIKit;
using System.Linq;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesViewController : ViewModelCollectionDrivenDialogViewController<BranchesViewModel>
    {
        public BranchesViewController()
        {
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsbranch.ToEmptyListImage(), "There are no branches."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel
                .Branches
                .ChangedObservable()
                .Subscribe(x =>
                {
                    var items = x.Select(y =>
                    {
                        var e = new StringElement(y.Name);
                        e.BindClick(y.GoToCommand);
                        return e;
                    });

                    Root.Reset(new Section { items });
                });
        }
    }
}

