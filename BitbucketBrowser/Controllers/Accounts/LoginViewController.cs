using System;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch;

namespace BitbucketBrowser.Controllers.Accounts
{
    public partial class LoginViewController : UIViewController
    {
        public Action LoginComplete;
        private string _username;

		public string Username
		{
			get { return _username; }
			set
			{
				_username = value;
				if (User != null)
					User.Text = _username;
			}
		}


        public LoginViewController()
            : base("LoginViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            View.BackgroundColor = UIColor.FromPatternImage(Images.LogoBehind);

            Logo.Image = Images.Logo;
            Title = "Login";
            if (Username != null)
                User.Text = Username;

            User.ShouldReturn = delegate
            {
                Password.BecomeFirstResponder();
                return true;
            };
            Password.ShouldReturn = delegate
            {
                Password.ResignFirstResponder();

                //Run this in another thread
                ThreadPool.QueueUserWorkItem(delegate { BeginLogin(); });
                return true;
            };
        }

        [Obsolete("Deprecated in iOS 6.0")]
        public override void ViewDidUnload()
        {
            base.ViewDidUnload();

            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;

            ReleaseDesignerOutlets();
        }

        private void BeginLogin()
        {
            MBProgressHUD hud = null;
            bool successful = false;
            string username = null, password = null, avatarUrl = null;

            //The nice hud
            InvokeOnMainThread(delegate {
                username = User.Text;
                password = Password.Text;
                hud = new MBProgressHUD(View) {Mode = MBProgressHUDMode.Indeterminate, TitleText = "Logging In..."};
                View.AddSubview(hud);
                hud.Show(true);
            });

            try
            {
                var client = new BitbucketSharp.Client(username, password);
                client.Account.SSHKeys.GetKeys();
                successful = true;
                
                var userInfo = client.Account.GetInfo();
                avatarUrl = userInfo.User.Avatar;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error = " + e.Message);
            }

            InvokeOnMainThread(delegate
            {
                //Dismiss the hud
                hud.Hide(true);
                hud.RemoveFromSuperview();

                if (!successful)
                {
                    Utilities.ShowAlert("Unable to Authenticate", "Unable to login as user " + username + ". Please check your credentials and try again. Remember, credentials are case sensitive!");
                    return;
                }

				var account = Application.Accounts.Find (User.Text);

				//Account does not exist! Add it!
				if (account == null)
			    {
               		account = new Account { Username = User.Text, Password = Password.Text, AvatarUrl = avatarUrl };
					Application.Accounts.Insert(account);
				}
				//Account already exists. Update the password just incase it changed...
				else
				{
					account.Password = Password.Text;
					account.Update();
                    Application.SetUser(account);
				}

                if (NavigationController != null)
                    NavigationController.PopViewControllerAnimated(true);

                if (LoginComplete != null)
                    LoginComplete();
            });
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                if (toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown)
                    return true;
            }
            else
            {
                // Return true for supported orientations
                return true;
            }

            return false;
        }
    }
}

