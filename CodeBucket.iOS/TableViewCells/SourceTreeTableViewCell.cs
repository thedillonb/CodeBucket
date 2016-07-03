using System;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Source;
using Foundation;
using ReactiveUI;

namespace CodeBucket.TableViewCells
{
    public class SourceTreeTableViewCell : BaseTableViewCell<SourceTreeItemViewModel>
    {
        public static NSString Key = new NSString("SourceTreeTableViewCell");

        public SourceTreeTableViewCell(IntPtr handle)
            : base(handle)
        {
            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    TextLabel.Text = x.Name;
                    ImageView.Image = GetIcon(x.Type).ToImage();
                });
        }

        private static AtlassianIcon GetIcon(SourceTreeItemViewModel.SourceTreeItemType type)
        {
            switch (type)
            {
                case SourceTreeItemViewModel.SourceTreeItemType.Directory:
                    return AtlassianIcon.Devtoolsfolderclosed;
                case SourceTreeItemViewModel.SourceTreeItemType.File:
                    return AtlassianIcon.Devtoolsfile;
                default:
                    return AtlassianIcon.Devtoolsfilebinary;
            }
        }
    }
}

