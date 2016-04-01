using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using System;
using UIKit;

namespace CodeBucket.Views.Teams
{
    public class TeamsView : ViewModelCollectionDrivenDialogViewController
    {
        public TeamsView()
        {
            Title = "Teams";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Userstatus.ToEmptyListImage(), "There are no teams."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var vm = (TeamsViewModel) ViewModel;
            var weakVm = new WeakReference<TeamsViewModel>(vm);
            BindCollection(vm.Teams, x => 
            {
                var e = new StringElement(x);
                e.Clicked.Subscribe(_ => weakVm.Get()?.GoToTeamCommand.Execute(x));
                return e;
            });
        }
    }
}