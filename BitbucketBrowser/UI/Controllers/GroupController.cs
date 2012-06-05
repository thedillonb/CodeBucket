using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;


namespace BitbucketBrowser.UI
{
	public class GroupController : Controller<IList<GroupModel>>
	{
        public string Username { get; private set; }

		public GroupController(string username, bool push = true) 
            : base(push)
		{
			Style = UITableViewStyle.Plain;
            Username = username;
            Title = "Groups";
		}
		
        protected override void OnRefresh()
        {

        }

        protected override IList<GroupModel> OnUpdate()
        {
            return null;
        }
	}
}

