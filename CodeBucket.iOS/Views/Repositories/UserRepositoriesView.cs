using CodeBucket.Core.ViewModels.Repositories;

namespace CodeBucket.iOS.Views.Repositories
{
	public class UserRepositoriesView : BaseRepositoriesView
	{
		public override void ViewDidLoad()
		{
			Title = "Repositories";
			base.ViewDidLoad();
		}
	}
}

