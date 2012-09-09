using System;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.UIKit;
using RedPlum;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using MonoTouch;
using CodeFramework.UI.Views;
using CodeFramework.UI.Elements;

namespace CodeFramework.UI.Controllers
{
    public abstract class Controller<T> : BaseDialogViewController
    {
        public T Model { get; set; }
        public bool Loaded { get; private set; }
        protected ErrorView _currentError;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeFramework.UI.Controllers.Controller`1"/> class.
        /// </summary>
        /// <param name='push'>True if navigation controller should push, false if otherwise</param>
        /// <param name='refresh'>True if the data can be refreshed, false if otherwise</param>
        public Controller(bool push = false, bool refresh = false)
            : base(push)
        {
            if (refresh)
                RefreshRequested += (sender, e) => Refresh(true);
        }

        //Called when the UI needs to be updated with the model data            
        protected abstract void OnRefresh();

        //Called when the controller needs to request the model from the server
        protected abstract T OnUpdate();

        public void Refresh(bool force = false)
        {
            InvokeOnMainThread(delegate {
                if (_currentError != null)
                    _currentError.RemoveFromSuperview();
                _currentError = null;
            });

            if (Model != null && !force)
            {
                try
                {
                    OnRefresh();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("There was an issue attempting to refresh: " + ex.Message);
                }

                InvokeOnMainThread(delegate { 
                    if (TableView.TableFooterView != null)
                        TableView.TableFooterView.Hidden = this.Root.Count == 0;

                    ReloadComplete(); 
                });
                Loaded = true;
                return;
            }

            if (!force)
            {
                this.DoWork(UpdateAndRefresh, (ex) => {
                    _currentError = ErrorView.Show(this.View.Superview, ex.Message);
                });
            }
            else
            {
                this.DoWorkNoHud(UpdateAndRefresh, (ex) => {
                    Utilities.ShowAlert("Unable to refresh!", "There was an issue while attempting to refresh. " + ex.Message);
                }, ReloadComplete);
            }
        }

        private void UpdateAndRefresh()
        {
            Model = OnUpdate();
            if (Model != null)
                Refresh();
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!Loaded)
            {
                Refresh();
                Loaded = true;
            }
        }
    }
}

