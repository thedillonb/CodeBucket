using CodeBucket.Utils;

namespace CodeBucket.Views.Issues
{
	public class IssueAddView : IssueModifyView
    {
		public override void ViewDidLoad()
		{
			Title = "New Issue";

			base.ViewDidLoad();
		}
    }
}

