using System;
using CodeBucket.Core.ViewModels.Teams;
using Foundation;
using ReactiveUI;

namespace CodeBucket.TableViewCells
{
    public class TeamTableViewCell : BaseTableViewCell<TeamItemViewModel>
    {
        public static NSString Key = new NSString("TeamTableViewCell");

        public TeamTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.WhenAnyValue(x => x.ViewModel).Subscribe(x =>
            {
                TextLabel.Text = x?.Name;
            });
        }
    }
}

