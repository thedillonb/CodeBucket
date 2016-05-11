using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.DialogElements;
using System.Reactive.Linq;
using System.Linq;
using UIKit;

namespace CodeBucket.ViewControllers.Source
{
    public class SourceTreeViewController : ViewModelCollectionDrivenDialogViewController<SourceTreeViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel
                .Content
                .ChangedObservable()
                .Select(x => x.Select(CreateElement))
                .Subscribe(x => Root.Reset(new Section { x }));
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

        private static Element CreateElement(SourceTreeItemViewModel x)
        {
            var e = new StringElement(x.Name, GetIcon(x.Type).ToImage());
            e.Clicked.Select(_ => x).BindCommand(x.GoToCommand);
            return e;
        }
    }
}

