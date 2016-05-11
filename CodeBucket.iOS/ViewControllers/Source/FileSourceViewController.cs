using CodeBucket.Core.ViewModels;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Source
{
    public abstract class FileSourceViewController : WebViewController<FileSourceViewModel>
    {
		protected FileSourceViewController()
			: base(false, false)
		{
		}

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            NavigationItem.RightBarButtonItem = null;
        }
    }
}

