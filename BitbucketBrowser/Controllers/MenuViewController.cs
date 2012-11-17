using BitbucketBrowser.UI.Controllers;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.UI.Elements;
using MonoTouch.Foundation;
using System.Drawing;
using System.Linq;
using BitbucketBrowser.UI.Controllers.Accounts;
using BitbucketBrowser.UI;
using BitbucketBrowser.UI.Controllers.Events;
using BitbucketBrowser.UI.Controllers.Repositories;
using BitbucketBrowser.UI.Controllers.Groups;
using System.Threading;

namespace BitbucketBrowser.Controllers
{
    
    public class MenuController : DialogViewController
    {
        public MenuController()
            : base(UITableViewStyle.Plain, new RootElement("CodeBucket"))
        {
            Autorotate = true;
            if (Application.Account != null && !string.IsNullOrEmpty(Application.Account.Username))
                Root.Caption = Application.Account.Username;
        }
        
        class NavElement : StyledElement
        {
            public NavElement(string title, NSAction tapped, UIImage image)
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
        
        protected virtual void DoShit(UIViewController controller)
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

            
            Root.Add(new Section() {
                new NavElement("Profile", () => DoShit(new ProfileController(Application.Account.Username, false) { Title = "Profile" }), Images.Person),
                new NavElement("Events", () => DoShit(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true }), Images.Event),
                new NavElement("Repositories", () => DoShit(new AccountRepositoryController(Application.Account.Username) { Title = "Repositories" }), Images.Repo),
                new NavElement("Groups", () => DoShit(new GroupController(Application.Account.Username, false) { Title = "Groups" }), Images.Group),
                new NavElement("Explore", () => DoShit(new ExploreController() { Title = "Explore" }), UIImage.FromBundle("/Images/Tabs/search")),
            });

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
            Root.Caption = Application.Account.Username;
            Title = Root.Caption;

            //Grab the avatar!
            if (string.IsNullOrEmpty(Application.Account.AvatarUrl))
            {
                ThreadPool.QueueUserWorkItem(delegate {
                    try 
                    {
                        var userInfo = Application.Client.Account.GetInfo();
                        Application.Account.AvatarUrl = userInfo.User.Avatar;
                        BeginInvokeOnMainThread(() => Application.Account.Update());
                    }
                    catch
                    {
                        //Swallow this exception...
                    }
                });
            }
        }
        
    }
}

