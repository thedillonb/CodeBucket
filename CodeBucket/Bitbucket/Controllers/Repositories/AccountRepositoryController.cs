using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using BitbucketSharp.Models;
using System.Collections.Generic;

namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class AccountRepositoryController : RepositoryController
    {
        private static string SavedSelection = "REPO_SELECTION";
        private static string[] _sections = new [] { "Owned", "Following" };
        
        public AccountRepositoryController(string username)
            : base(username, true)
        {
            MultipleSelections = _sections;
            MultipleSelectionsKey = SavedSelection;
        }

        protected override List<RepositoryDetailedModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
  
            //Set the show property based on what is selected
            ShowOwner = selected != 0;

            //No need for paging in bitbucket land
            nextPage = -1;

            if (selected == 0)
                return Application.Client.Users[Username].GetInfo(force).Repositories;
            else if (selected == 1)
                return Application.Client.Account.GetRepositories(force);
            else
                return new List<RepositoryDetailedModel>();
        }
    }

}

