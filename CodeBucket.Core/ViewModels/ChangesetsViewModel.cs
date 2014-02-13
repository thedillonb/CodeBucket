using System.Collections.Generic;
using BitbucketSharp.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels
{
	public class ChangesetsViewModel : CommitsViewModel
    {
		protected override List<ChangesetModel> GetRequest(string startNode)
        {
			var data = this.GetApplication().Client.Users[Username].Repositories[Repository].Changesets.GetChangesets(30, startNode);
			return data.Changesets.OrderByDescending(x => x.Utctimestamp).ToList();
        }

		public new class NavObject : CommitsViewModel.NavObject
        {
        }
    }
}

