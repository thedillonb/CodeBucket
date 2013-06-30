using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using MonoTouch.Foundation;
using CodeBucket.Cells;

namespace CodeBucket.Elements
{
	
	public class RepositoryElement : Element, IElementSizing, IColorizeBackground
	{       
		private string _name;
		private string _scm;
		private int _followers;
		private int _forks;
		private string _description;
		private string _owner;
		
		public UITableViewCellStyle Style { get; set;}
		public UIColor BackgroundColor { get; set; }
		public bool ShowOwner { get; set; }
		
		public RepositoryElement(string name, string scm, int followers, int forks, string description, string owner)
			: base(null)
		{
			_name = name;
			_scm = scm;
			_followers = followers;
			_forks = forks;
			_description = description;
			_owner = owner;
			this.Style = UITableViewCellStyle.Default;
			ShowOwner = true;
		}
		
		public RepositoryElement(RepositoryDetailedModel m) :
			this(m.Name, m.Scm, m.FollowersCount, m.ForkCount, m.Description, m.Owner)
		{
		}

		public RepositoryElement(GitHubSharp.Models.RepositoryModel model)
			: this(model.Name, "git", model.Watchers, model.Forks, model.Description, model.Owner != null ? model.Owner.Login : null)
		{
		}
		
		public float GetHeight (UITableView tableView, NSIndexPath indexPath)
		{
			return 67f;
		}
		
		protected override NSString CellKey {
			get {
				return new NSString("RepositoryCellView");
			}
		}
		
		
		public event NSAction Tapped;
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var cell = tv.DequeueReusableCell(CellKey) as RepositoryCellView;
			if (cell == null)
				cell = RepositoryCellView.Create();
			return cell;
		}
		
		public override bool Matches(string text)
		{
			return _name.ToLower().Contains(text.ToLower());
		}
		
		public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			base.Selected(dvc, tableView, path);
			if (Tapped != null)
				Tapped();
			tableView.DeselectRow (path, true);
		}
		
		void IColorizeBackground.WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
		{
			var c = cell as RepositoryCellView;
			if (c != null)
				c.Bind(_name, _scm, _followers.ToString(), _forks.ToString(), _description, ShowOwner ? _owner : null);
		}
	}
}

