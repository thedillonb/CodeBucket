namespace CodeBucket.Core.ViewModels
{
	public class WebBrowserViewModel : BaseViewModel
    {
		public string Url { get; }

        public string PageTitle
        {
            get { return Title; }
            set { Title = value; }
        }

        public WebBrowserViewModel(string url)
        {
            Url = url;
            Title = "Web";
        }
    }
}

