using System;
using CodeFramework.UI.Controllers;
using System.Collections.Generic;

namespace BitbucketBrowser.UI.Controllers.Issues
{
    public abstract class IssueTrackerSettingsController<T> : Controller<List<T>>
    {
        public IssueTrackerSettingsController()
        {
        }
    }
}

