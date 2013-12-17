using System;
using BitbucketSharp.Models;
using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using System.Collections.Generic;
using System.Linq;
using MonoTouch;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;


namespace CodeBucket.Controllers
{
    public class ChangesetController : ListController<ChangesetModel>
    {
        private const int RequestLimit = 30;

        public string User { get; private set; }

        public string Slug { get; private set; }

        public ChangesetController(IView<ListModel<ChangesetModel>> view, string user, string slug)
            : base(view)
        {
            User = user;
            Slug = slug;
        }

        public override void Update(bool force)
        {
            Model = new ListModel<ChangesetModel> { Data = GetData() };
            Model.More = () => {
                var data = GetData(Model.Data.Last().Node);
                if (data.Count > 1)
                {
                    data.RemoveAt(0);
                    Model.Data.AddRange(data);
                }
                else
                    Model.More = null;

                Render();
            };
        }

        protected List<ChangesetModel> GetData(string startNode = null)
        {
            var data = Application.Client.Users[User].Repositories[Slug].Changesets.GetChangesets(RequestLimit, startNode);
            return data.Changesets.OrderByDescending(x => x.Utctimestamp).ToList();
        }
    }
}

