namespace CodeBucket.Core.ViewModels
{
	public class WebBrowserViewModel : BaseViewModel
    {
		public string Url { get; }

        public WebBrowserViewModel(string url)
        {
            Url = url;
        }
    }
}

