using BitbucketBrowser;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketBrowser.Elements;
using MonoTouch.Foundation;
using System.Drawing;
using System.Linq;

namespace CodeBucket.Controllers
{
    public abstract class MenuBaseController : DialogViewController
    {
		public MenuBaseController()
            : base(UITableViewStyle.Plain, new RootElement("CodeBucket"))
        {
            Autorotate = true;
            if (Application.Account != null && !string.IsNullOrEmpty(Application.Account.Username))
                Root.Caption = Application.Account.Username;
        }

		/// <summary>
		/// Invoked when it comes time to set the root so the child classes can create their own menus
		/// </summary>
		protected abstract void OnCreateMenu(RootElement root);
        
        protected virtual void NavPush(UIViewController controller)
        {
            NavigationController.PushViewController(controller, false);
        }
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.ChangeUser, UIBarButtonItemStyle.Bordered, (s, e) => {
                var n = new UINavigationController(new SettingsController());
                this.PresentModalViewController(n, true);
            });

			//Set the menu
			OnCreateMenu(Root);
            
			//Add some nice looking colors and effects
            TableView.BackgroundColor = UIColor.Clear;
            UIImage background = Images.Linen;
            View.BackgroundColor = UIColor.FromPatternImage(background);            
            TableView.SeparatorColor = UIColor.FromRGBA(128, 128, 128, 128);
            var view = new UIView(new RectangleF(0, 0, View.Bounds.Width, 10));
            view.BackgroundColor = UIColor.Clear;
            TableView.TableFooterView = view;
        }
        
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
			var root = new RootElement(Application.Account.Username);
            Title = root.Caption;
			OnCreateMenu(root);
			Root = root;
        }

		protected class MenuElement : StyledElement
		{
			public MenuElement(string title, NSAction tapped, UIImage image)
				: base(title, tapped, image)
			{
				BackgroundColor = UIColor.Clear;
				TextColor = UIColor.White;
				DetailColor = UIColor.White;
			}
			
			public override UITableViewCell GetCell(UITableView tv)
			{
				var cell = base.GetCell(tv);
				cell.SelectedBackgroundView = new UIView { BackgroundColor = UIColor.FromRGBA(41, 41, 41, 200) };
				
				var f = cell.Subviews.Count(x => x.Tag == 1111);
				if (f == 0)
				{
					var v2 = new UIView(new RectangleF(0, cell.Frame.Height - 3, cell.Frame.Width, 1))
					{BackgroundColor = UIColor.FromRGBA(41, 41, 41, 64), Tag = 1111};
					cell.AddSubview(v2);
					
					var v = new UIView(new RectangleF(0, cell.Frame.Height - 2, cell.Frame.Width, 1))
					{ BackgroundColor = UIColor.FromRGBA(41, 41, 41, 200), Tag = 1111};
					cell.AddSubview(v);
				}
				
				return cell;
			}
		}
    }
}

