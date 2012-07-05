using System;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.UIKit;
using RedPlum;
using System.Linq;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public abstract class Controller<T> : DialogViewController
    {
        public T Model { get; set; }

        private bool _loaded = false;

        public bool Loaded { get { return _loaded; } }

        public Controller(bool push = false, bool refresh = false)
            : base(new RootElement(""), push)
        {
            if (refresh)
                RefreshRequested += (sender, e) => Refresh(true);

           

            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null); 

            Autorotate = true;
        }

        class RefreshView : RefreshTableHeaderView
        {
            public RefreshView(RectangleF rect)
                : base(rect)
            {
                BackgroundColor = UIColor.Clear;
                StatusLabel.BackgroundColor = UIColor.Clear;
                LastUpdateLabel.BackgroundColor = UIColor.Clear;
            }

            public override void Draw(RectangleF rect)
            {
                //Stop the super class from doing stupid shit like drawing the shadow
            }
        }


        public override RefreshTableHeaderView MakeRefreshTableHeaderView(RectangleF rect)
        {
            return new RefreshView(rect);
        }

            
        protected abstract void OnRefresh();

        protected abstract T OnUpdate();

        public override void ViewDidLoad()
        {
            Root.Caption = this.Title;


            TableView.BackgroundColor = UIColor.White;
            UIImage background = UIImage.FromBundle("/Images/Cells/stuff");
            View.BackgroundColor = UIColor.FromPatternImage(background);

            if (Style == UITableViewStyle.Grouped)
            {
                //TableView.BackgroundColor = UIColor.Clear;
                //UIImage background = UIImage.FromBundle("/Images/Cells/background");
                //ParentViewController.View.BackgroundColor = UIColor.FromPatternImage(background);



            }
            else
            {
                var view = new UIView(new RectangleF(0, 0, View.Bounds.Width, 10));
                view.BackgroundColor = UIColor.Clear;
                TableView.TableFooterView = view;
            }


            base.ViewDidLoad();
        }

        /*
        public override void LoadView()
        {
            base.LoadView();
            if (Style == UITableViewStyle.Grouped)
            {
                TableView.BackgroundColor = UIColor.Clear;
                UIImage background = UIImage.FromBundle("/Images/Cells/background");
                ParentViewController.View.BackgroundColor = UIColor.FromPatternImage(background);
            }
        }
        */

        public void Refresh(bool force = false)
        {
            if (Model != null && !force)
            {
                try
                {
                    OnRefresh();
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => ErrorView.Show(this.View.Superview, e.Message));
                }

                InvokeOnMainThread(delegate { 
                    ReloadComplete(); 
                });
                _loaded = true;
                return;
            }

            MBProgressHUD hud = null;
            if (!force) {
                hud = new MBProgressHUD(this.View.Superview); 
                hud.Mode = MBProgressHUDMode.Indeterminate;
                hud.TitleText = "Loading...";
                this.View.Superview.AddSubview(hud);
                hud.Show(true);
            }

            ThreadPool.QueueUserWorkItem(delegate {
                try
                {
                    Model = OnUpdate();
                    Refresh();
                }
                catch (Exception e)
                {
                    InvokeOnMainThread(() => ErrorView.Show(this.View.Superview, e.Message));
                }


                if (hud != null)
                {
                    InvokeOnMainThread(delegate {
                        hud.Hide(true);
                        hud.RemoveFromSuperview();
                    });
                }
            });
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!_loaded)
            {
                Refresh();
                _loaded = true;
            }
        }
    }
}

