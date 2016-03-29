using CodeBucket.Core.ViewModels.Source;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using System;
using UIKit;

namespace CodeBucket.Views.Source
{
    public class BranchesView : ViewModelCollectionDrivenDialogViewController
    {
        public BranchesView()
        {
            Title = "Branches";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsbranch.ToEmptyListImage(), "There are no branches."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            var vm = (BranchesViewModel) ViewModel;
            var weakVm = new WeakReference<BranchesViewModel>(vm);
            BindCollection(vm.Branches, x => 
            {
                var e = new StringElement(x.Branch);
                e.Clicked.Subscribe(_ => weakVm.Get()?.GoToBranchCommand.Execute(x));
                return e;
            });
        }
    }
}

