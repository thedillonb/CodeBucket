using System;
using CodeFramework.ViewControllers;
using CodeBucket.Core.ViewModels.Source;
using MonoTouch.Dialog;
using CodeBucket.iOS.Views.Filters;

namespace CodeBucket.iOS.Views.Source
{
    public class SourceTreeView : ViewModelCollectionDrivenViewController
    {
        public new SourceTreeViewModel ViewModel
        {
            get { return (SourceTreeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			NavigationItem.RightBarButtonItem = new MonoTouch.UIKit.UIBarButtonItem(Theme.CurrentTheme.SortButton, MonoTouch.UIKit.UIBarButtonItemStyle.Plain, 
				(s, e) => ShowFilterController(new SourceFilterViewController(ViewModel.Content))); 
            BindCollection(ViewModel.Content, CreateElement);
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Title = string.IsNullOrEmpty(ViewModel.Path) ? ViewModel.Repository : ViewModel.Path.Substring(ViewModel.Path.LastIndexOf('/') + 1);
		}

		private Element CreateElement(SourceTreeViewModel.SourceModel x)
        {
            if (x.Type.Equals("dir", StringComparison.OrdinalIgnoreCase))
                return new StyledStringElement(x.Name, () => ViewModel.GoToSourceTreeCommand.Execute(x), Images.Folder);
            if (x.Type.Equals("file", StringComparison.OrdinalIgnoreCase))
				return new StyledStringElement(x.Name, () => ViewModel.GoToSourceCommand.Execute(x), Images.File);
            return new StyledStringElement(x.Name) { Image = Images.File };
        }
    }
}

