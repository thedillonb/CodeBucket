using System;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.UIKit;


namespace BitbucketBrowser.UI
{
    public abstract class Controller<T> : DialogViewController
    {
        public T Model { get; set; }

        private bool _loaded = false;

        public Controller(bool push = false, bool refresh = false)
            : base(new RootElement(""), push)
        {
            //View.BackgroundColor = UIColor.FromRGB(0.85f, 0.85f, 0.85f);
            if (refresh)
                RefreshRequested += (sender, e) => Refresh(true);
        }

        protected abstract void OnRefresh();

        protected abstract T OnUpdate();

        public override void ViewDidLoad()
        {
            Root.Caption = this.Title;
            base.ViewDidLoad();
        }

        public void Refresh(bool force = false)
        {
            if (Model != null && !force)
            {
                OnRefresh();
                InvokeOnMainThread(delegate { ReloadComplete(); });
                return;
            }

            ThreadPool.QueueUserWorkItem(delegate {
                Model = OnUpdate();
                Refresh();
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

