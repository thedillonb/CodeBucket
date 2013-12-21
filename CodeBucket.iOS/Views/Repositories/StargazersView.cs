using CodeFramework.iOS.Elements;
using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.iOS.Views.User;

namespace CodeBucket.iOS.Views.Repositories
{
    public class StargazersView : BaseUserCollectionView
    {
		public override void ViewDidLoad()
		{
			Title = "Stargazers".t();
			NoItemsText = "No Stargazers".t();
			base.ViewDidLoad();
		}
    }
}

