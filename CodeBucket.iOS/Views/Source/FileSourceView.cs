using UIKit;
using CodeBucket.Core.ViewModels;

namespace CodeBucket.Views.Source
{
    public abstract class FileSourceView : WebView
    {
		private bool _loaded = false;

		public new FileSourceViewModel ViewModel
		{ 
			get { return (FileSourceViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		protected FileSourceView()
			: base(false)
		{
		}

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            NavigationItem.RightBarButtonItem = null;
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//Stupid but I can't put this in the ViewDidLoad...
			if (!_loaded)
			{
                (ViewModel as ILoadableViewModel)?.LoadCommand.Execute(null);
				_loaded = true;
			}

			Title = ViewModel.Title;
		}
    }
}

