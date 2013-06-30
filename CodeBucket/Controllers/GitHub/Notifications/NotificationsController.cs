using CodeBucket.Controllers;
using CodeBucket.Elements;
using GitHubSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace CodeBucket.GitHub.Controllers.Notifications
{
    public class NotificationsController : ListController<NotificationModel>
    {
        private const string SavedSelection = "NOTIFICATION_SELECTION";
        private static string[] _sections = new [] { "Unread", "Participating", "All" };

        public NotificationsController()
        {
            MultipleSelections = _sections;
            MultipleSelectionsKey = SavedSelection;
            Title = "Notifications";
        }

        protected override List<NotificationModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            GitHubSharp.GitHubResponse<List<NotificationModel>> data = null;
            nextPage = -1;
            
            if (selected == 0)
                data = Application.GitHubClient.API.GetNotifications();
            else if (selected == 1)
                data = Application.GitHubClient.API.GetNotifications(false, true);
            else if (selected == 2)
                data = Application.GitHubClient.API.GetNotifications(true, false);
            else
                return new List<NotificationModel>();
            return data.Data;
        }

        protected override MonoTouch.Dialog.Element CreateElement(NotificationModel obj)
        {
            var sse = new NameTimeStringElement() { 
                Time = obj.UpdatedAt, 
                String = obj.Subject.Title, 
                Lines = 4, 
            };
            
            sse.Name = obj.Repository.Name;
            return sse;
        }
    }
}

