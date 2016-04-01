using CodeBucket.Core.ViewModels.Source;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using System;
using UIKit;

namespace CodeBucket.Views.Source
{
    public class ChangesetBranchesView : ViewModelCollectionDrivenDialogViewController
    {
        public ChangesetBranchesView()
        {
            Title = "Branches";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsbranch.ToEmptyListImage(), "There are no branches."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var vm = (ChangesetBranchesViewModel) ViewModel;
            var weakVm = new WeakReference<ChangesetBranchesViewModel>(vm);
            BindCollection(vm.Branches, x => 
            {
                var e = new StringElement(x.Name);
                e.Clicked.Subscribe(_ => weakVm.Get()?.GoToBranchCommand.Execute(x));
                return e;
            });
        }
    }
}

