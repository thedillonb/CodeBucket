using System;
using CodeBucket.Core.ViewModels.Groups;
using Foundation;
using ReactiveUI;

namespace CodeBucket.TableViewCells
{
    public class GroupTableViewCell : BaseTableViewCell<GroupItemViewModel>
    {
        public static NSString Key = new NSString("GroupTableViewCell");

        public GroupTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.WhenAnyValue(x => x.ViewModel).Subscribe(x =>
            {
                TextLabel.Text = x.Name;
            });
        }
    }
}

