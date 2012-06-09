using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;


namespace BitbucketBrowser.UI
{
	public class BranchController : Controller<List<BranchModel>>
	{
        public string Username { get; private set; }

        public string Slug { get; private set; }

		public BranchController(string username, string slug) 
            : base(true, true)
		{
			Style = UITableViewStyle.Plain;
            Username = username;
            Slug = slug;
            Title = "Branches";
		}
		
        protected override void OnRefresh()
        {

        }

        protected override List<BranchModel> OnUpdate()
        {
            return new List<BranchModel>();
        }
	}
}

