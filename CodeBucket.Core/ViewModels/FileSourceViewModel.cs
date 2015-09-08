using System.Linq;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels
{
	public abstract class FileSourceViewModel : LoadableViewModel
    {
		private static readonly string[] BinaryMIMEs = new string[] 
		{ 
			"image/", "video/", "audio/", "model/", "application/pdf", "application/zip", "application/gzip"
		};

		private string _filePath;
		private bool _isText;

		public string FilePath
		{
			get { return _filePath; }
			protected set 
			{
				_filePath = value;
				RaisePropertyChanged(() => FilePath);
			}
		}

		public bool IsText
		{
            get { return _isText; }
			protected set 
			{
                _isText = value;
                RaisePropertyChanged(() => IsText);
			}
		}

		public string Title
		{
			get;
			protected set;
		}

		public string HtmlUrl
		{
			get;
			protected set;
		}

		public ICommand GoToHtmlUrlCommand
		{
			get { return new MvxCommand(() => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = HtmlUrl }), () => !string.IsNullOrEmpty(HtmlUrl)); }
		}

		public ICommand ShareCommand
		{
			get
			{
				return new MvxCommand(() => GetService<IShareService>().ShareUrl(HtmlUrl), () => !string.IsNullOrEmpty(HtmlUrl));
			}
		}

		protected static bool IsBinary(string mime)
		{
			var lowerMime = mime.ToLower();
		    return BinaryMIMEs.Any(lowerMime.StartsWith);
		}
    }
}

