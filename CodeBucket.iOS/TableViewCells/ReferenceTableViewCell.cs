using System;
using CodeBucket.Core.ViewModels.Source;
using Foundation;
using ReactiveUI;

namespace CodeBucket.TableViewCells
{
    public class ReferenceTableViewCell : BaseTableViewCell<GitReferenceItemViewModel>
    {
        public static NSString Key = new NSString("ReferenceTableViewCell");

        public ReferenceTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.WhenAnyValue(x => x.ViewModel).Subscribe(x =>
            {
                TextLabel.Text = x?.Name;
            });
        }
    }
}


