using System;
using CodeFramework.Controllers;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Controllers
{
    public class TagsController : ListController<TagsController.TagModel>
    {
        private readonly string _username;
        private readonly string _slug;

        public TagsController(IView<ListModel<TagsController.TagModel>> view, string username, string slug)
            : base(view)
        {
            _username = username;
            _slug = slug;
        }

        public override void Update(bool force)
        {
            var tags = Application.Client.Users[_username].Repositories[_slug].GetTags(force);
            Model = new ListModel<TagModel> {
                Data = tags.Select(x => new TagModel { Name = x.Key, Node = x.Value.Node }).OrderBy(x => x.Name).ToList()
            };
        }

        /// <summary>
        /// Bitbucket passes it's tag model around as a dictionary with the name as the key
        /// I don't care for that since I like my lists to inherit from ListController
        /// So I need to convert into an intermediate that has the two peices of information I need.
        /// </summary>
        public class TagModel
        {
            public string Name { get; set; }
            public string Node { get; set; }
        }
    }
}

