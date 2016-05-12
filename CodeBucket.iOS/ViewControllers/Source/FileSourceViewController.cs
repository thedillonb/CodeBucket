using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Source
{
    public abstract class FileSourceViewController<TViewModel> : WebViewController<TViewModel>
        where TViewModel : class
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

