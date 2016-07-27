using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
	public class WebBrowserViewModel : ReactiveObject, IViewModel
    {
		public string Url { get; }

        public WebBrowserViewModel(string url)
        {
            Url = url;
        }
    }
}

