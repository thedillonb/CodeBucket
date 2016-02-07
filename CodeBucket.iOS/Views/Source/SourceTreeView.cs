using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.Views.Filters;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;

namespace CodeBucket.Views.Source
{
    public class SourceTreeView : ViewModelCollectionDrivenDialogViewController
    {
        public new SourceTreeViewModel ViewModel
        {
            get { return (SourceTreeViewModel) base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			NavigationItem.RightBarButtonItem = new UIKit.UIBarButtonItem(Theme.CurrentTheme.SortButton, UIKit.UIBarButtonItemStyle.Plain, 
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
                return new StyledStringElement(x.Name, () => ViewModel.GoToSourceTreeCommand.Execute(x), AtlassianIcon.Devtoolsfolderclosed.ToImage());
            if (x.Type.Equals("file", StringComparison.OrdinalIgnoreCase))
                return new StyledStringElement(x.Name, () => ViewModel.GoToSourceCommand.Execute(x), AtlassianIcon.Devtoolsfile.ToImage());
            return new StyledStringElement(x.Name) { Image = AtlassianIcon.Devtoolsfilebinary.ToImage() };
        }
    }
}

