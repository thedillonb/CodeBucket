using System;
using System.Linq;
using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers;
using System.Collections.Generic;
using CodeBucket.Bitbucket.Controllers.Issues;
using MonoTouch;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeBucket.ViewControllers;

namespace CodeBucket.Controllers
{
    public class EventsController : ListController<EventModel>
    {
        private const int _dataLimit = 30;

        public string Username { get; private set; }

        public EventsController(IListView<EventModel> view, string username)
			: base(view)
        {
            Username = username;
        }

        public override void Update(bool force)
        {
            var sizeRequest = GetTotalItemCount();
            Model = new ListModel<EventModel> { Data = GetData() };
            if (Model.Data.Count < _dataLimit)
                return;

            if (Model.Data.Count < sizeRequest)
            {
                Model.More = () => {
                    var data = GetData(Model.Data.Count);
                    Model.Data.AddRange(data);
                    if (Model.Data.Count >= sizeRequest || data.Count < _dataLimit)
                        Model.More = null;
                    Render();
                };
            }
        }

        protected virtual List<EventModel> GetData(int start = 0, int limit = _dataLimit)
        {
            var events = Application.Client.Users[Username].GetEvents(start, limit);
            return events.Events.OrderByDescending(x => x.UtcCreatedOn).ToList();
        }

        protected virtual int GetTotalItemCount()
        {
            return Application.Client.Users[Username].GetEvents(0, 0).Count;
        }
    }
}