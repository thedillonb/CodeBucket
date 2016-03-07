using UIKit;
using System;

namespace CodeBucket.Utilities
{
    public static class NetworkActivity
    {
        /// <summary>
        ///   A shortcut to the main application
        /// </summary>
        public static UIApplication MainApp = UIApplication.SharedApplication;

        //
        // Since we are a multithreaded application and we could have many
        // different outgoing network connections (api.twitter, images,
        // searches) we need a centralized API to keep the network visibility
        // indicator state
        //
        static readonly object NetworkLock = new object ();
        static int _active;

        public static void PushNetworkActive ()
        {
            lock (NetworkLock){
                _active++;
                MainApp.NetworkActivityIndicatorVisible = true;
            }
        }

        public static void PopNetworkActive ()
        {
            lock (NetworkLock){
                if (_active == 0)
                    return;

                _active--;
                if (_active == 0)
                    MainApp.NetworkActivityIndicatorVisible = false;
            }
        }
    }

    public class LoadingIndicator
    {
        private int _value;

        public void Up()
        {
            _value++;
            NetworkActivity.PushNetworkActive();
        }

        public void Down()
        {
            if (_value == 0)
                return;
            _value--;
            NetworkActivity.PopNetworkActive();
        }

        ~LoadingIndicator()
        {
            for (var i = 0; i < _value; i++)
            {
                NetworkActivity.PopNetworkActive();
            }
        }
    }
}

