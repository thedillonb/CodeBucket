using System;
using CodeFramework.Core.ViewModels;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Wiki
{
	public class WikiViewModel : LoadableViewModel
    {
        public WikiViewModel()
        {
        }

		protected override Task Load(bool forceCacheInvalidation)
		{
			throw new NotImplementedException();
		}

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
			public string Page { get; set; }
		}
    }
}

