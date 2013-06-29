using GitHubSharp.Models;
using System.Collections.Generic;
using CodeBucket.Controllers;

namespace CodeBucket.GitHub.Controllers.Gists
{
    public class AccountGistsController : GistsController
    {
        private const string SavedSelection = "GIST_SELECTION";
        private static string[] _sections = new [] { "Owned", "Starred", "All" };

        public AccountGistsController(string username, bool push)
            : base(username, push)
        {
            MultipleSelections = _sections;
            MultipleSelectionsKey = SavedSelection;

//            NavigationItem.RightBarButtonItem = new MonoTouch.UIKit.UIBarButtonItem(MonoTouch.UIKit.UIBarButtonSystemItem.Compose, (s, e) => {
//                var composer = new Composer { Title = "New Gist", ActionButtonText = "Save" };
//                composer.NewComment(this, () => {
//                    var text = composer.Text;
//                    composer.CloseComposer();
//                });
//            });
        }

        protected override List<GistModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            GitHubSharp.GitHubResponse<List<GistModel>> data = null;

            if (selected == 0)
                data = Application.GitHubClient.API.GetGists(null, currentPage);
            else if (selected == 1)
                data = Application.GitHubClient.API.GetStarredGists(currentPage);
            else if (selected == 2)
                data = Application.GitHubClient.API.GetPublicGists(currentPage);
            else
            {
                nextPage = -1;
                return new List<GistModel>();
            }

            nextPage = data.Next == null ? -1 : currentPage + 1;
            return data.Data;
        }
    }
}

