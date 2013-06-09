using System;
using MonoTouch.UIKit;
using RedPlum;
using System.Threading;
using MonoTouch;
using CodeBucket.Data;

namespace CodeBucket.Bitbucket.Controllers.Accounts
{
    public partial class LoginViewController : UIViewController
    {
        public Action<Account> LoginComplete;
        private string _username;

        public string Username {
            get { return _username; }
            set {
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

            Title = "Login";
            Logo.Image = Images.BitbucketLogo;
            if (Username != null)
                User.Text = Username;

            User.ShouldReturn = delegate {
                Password.BecomeFirstResponder();
                return true;
            };
            Password.ShouldReturn = delegate {
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
		/// <summary>
		/// Login the specified username and password.
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
        protected Account Login(string username, string password)
        {
            var client = new BitbucketSharp.Client(username, password);
            client.Account.SSHKeys.GetKeys();
            var userInfo = client.Account.GetInfo();

            //Username was actually a email address!
            //User the username instead
            if (username.Contains("@"))
                username = userInfo.User.Username;

            return new Account { Username = username, Password = password, AvatarUrl = userInfo.User.Avatar, AccountType = Account.Type.Bitbucket };
        }
		/// <summary>
		/// Begins the login process.
		/// </summary>
        private void BeginLogin()
        {
            MBProgressHUD hud = null;
            string username = null, password = null;
            Account loggedInAccount = null;

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
                loggedInAccount = Login(username, password);
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

				if (loggedInAccount == null)
                {
                    Utilities.ShowAlert("Unable to Authenticate", "Unable to login as user " + username + ". Please check your credentials and try again. Remember, credentials are case sensitive!");
                    return;
                }

				var account = Application.Accounts.Find(loggedInAccount.Username, Account.Type.Bitbucket);

				//Account does not exist! Add it!
				if (account == null)
			    {
					account = loggedInAccount;
					Application.Accounts.Insert(account);
				}
				//Account already exists. Update the password just incase it changed...
				else
				{
					account.Password = Password.Text;
					account.Update();
					Application.SetUser(account);
				}

                if (LoginComplete != null)
                    LoginComplete(account);
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

