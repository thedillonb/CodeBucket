using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.DialogElements;
using System;
using UIKit;
using CodeBucket.Views;
using System.Linq;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamsViewController : ViewModelDrivenDialogViewController<TeamsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Userstatus.ToEmptyListImage(), "There are no teams."));

            ViewModel.Teams.ChangedObservable()
              .Subscribe(x =>
              {
                  var elements = x.Select(y =>
                  {
                      var e = new StringElement(y.Name);
                      e.BindClick(y.GoToCommand);
                      return e;
                  });

                  Root.Reset(new Section { elements });
              });
        }
    }
}