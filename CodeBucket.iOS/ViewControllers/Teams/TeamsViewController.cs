using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.DialogElements;
using System;
using UIKit;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamsViewController : ViewModelCollectionDrivenDialogViewController
    {
        public TeamsViewController()
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
                var e = new StringElement(x.Username);
                e.Clicked.Subscribe(_ => weakVm.Get()?.GoToTeamCommand.Execute(x));
                return e;
            });
        }
    }
}