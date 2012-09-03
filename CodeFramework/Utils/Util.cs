using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.Dialog;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using MonoTouch.CoreLocation;

namespace MonoTouch
{
    public static class Utilities
    {
        /// <summary>
        ///   A shortcut to the main application
        /// </summary>
        public static UIApplication MainApp = UIApplication.SharedApplication;

        public readonly static string BaseDir = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.Personal), "..");

        //
        // Since we are a multithreaded application and we could have many
        // different outgoing network connections (api.twitter, images,
        // searches) we need a centralized API to keep the network visibility
        // indicator state
        //
        static object networkLock = new object ();
        static int active;

        public static void PushNetworkActive ()
        {
            lock (networkLock){
                active++;
                MainApp.NetworkActivityIndicatorVisible = true;
            }
        }

        public static void PopNetworkActive ()
        {
            lock (networkLock){
                active--;
                if (active == 0)
                    MainApp.NetworkActivityIndicatorVisible = false;
            }
        }

        public static DateTime LastUpdate (string key)
        {
            var s = Defaults.StringForKey (key);
            if (s == null)
                return DateTime.MinValue;
            long ticks;
            if (Int64.TryParse (s, out ticks))
                return new DateTime (ticks, DateTimeKind.Utc);
            else
                return DateTime.MinValue;
        }

        public static bool NeedsUpdate (string key, TimeSpan timeout)
        {
            return DateTime.UtcNow - LastUpdate (key) > timeout;
        }

        public static void RecordUpdate (string key)
        {
            Defaults.SetString (key, DateTime.UtcNow.Ticks.ToString ());
        }


        public static NSUserDefaults Defaults = NSUserDefaults.StandardUserDefaults;

        const long TicksOneDay = 864000000000;
        const long TicksOneHour = 36000000000;
        const long TicksMinute = 600000000;

        public static string StripHtml (string str)
        {
            if (str.IndexOf ('<') == -1)
                return str;
            var sb = new StringBuilder ();
            for (int i = 0; i < str.Length; i++){
                char c = str [i];
                if (c != '<'){
                    sb.Append (c);
                    continue;
                }

                for (i++; i < str.Length; i++){
                    c =  str [i];
                    if (c == '"' || c == '\''){
                        var last = c;
                        for (i++; i < str.Length; i++){
                            c = str [i];
                            if (c == last)
                                break;
                            if (c == '\\')
                                i++;
                        }
                    } else if (c == '>')
                        break;
                }
            }
            return sb.ToString ();
        }

        public static string CleanName (string name)
        {
            if (name.Length == 0)
                return "";

            bool clean = true;
            foreach (char c in name){
                if (Char.IsLetterOrDigit (c) || c == '_')
                    continue;
                clean = false;
                break;
            }
            if (clean)
                return name;

            var sb = new StringBuilder ();
            foreach (char c in name){
                if (!Char.IsLetterOrDigit (c))
                    break;

                sb.Append (c);
            }
            return sb.ToString ();
        }

        public static RootElement MakeProgressRoot (string caption)
        {
            return new RootElement (caption){
                new Section (){
                    new ActivityElement ()
                }
            };
        }

        static long lastTime;
        [Conditional ("TRACE")]
        public static void ReportTime (string s)
        {
            long now = DateTime.UtcNow.Ticks;

            Debug.WriteLine (string.Format ("[{0}] ticks since last invoke: {1}", s, now-lastTime));
            lastTime = now;
        }
        
        [Conditional ("TRACE")]
        public static void Log (string format, params object [] args)
        {
            Debug.WriteLine (String.Format (format, args));
        }

        public static void LogException (string text, Exception e)
        {
            using (var s = System.IO.File.AppendText (Utilities.BaseDir + "/Documents/crash.log")){
                var msg = String.Format ("On {0}, message: {1}\nException:\n{2}", DateTime.Now, text, e.ToString());
                s.WriteLine (msg);
                Console.WriteLine (msg);
            }
        }

        static UIActionSheet sheet;
        public static UIActionSheet GetSheet (string title)
        {
            sheet = new UIActionSheet (title);
            return sheet;
        }

        static CultureInfo americanCulture;
        public static CultureInfo AmericanCulture {
            get {
                if (americanCulture == null)
                    americanCulture = new CultureInfo ("en-US");
                return americanCulture;
            }
        }
        #region Location

        internal class MyCLLocationManagerDelegate : CLLocationManagerDelegate {
            Action<CLLocation> callback;

            public MyCLLocationManagerDelegate (Action<CLLocation> callback)
            {
                this.callback = callback;
            }

            public override void UpdatedLocation (CLLocationManager manager, CLLocation newLocation, CLLocation oldLocation)
            {
                manager.StopUpdatingLocation ();
                locationManager = null;
                callback (newLocation);
            }

            public override void Failed (CLLocationManager manager, NSError error)
            {
                callback (null);
            }

        }

        static CLLocationManager locationManager;
        static public void RequestLocation (Action<CLLocation> callback)
        {
            locationManager = new CLLocationManager () {
                DesiredAccuracy = CLLocation.AccuracyBest,
                Delegate = new MyCLLocationManagerDelegate (callback),
                DistanceFilter = 1000f
            };
            if (CLLocationManager.LocationServicesEnabled)
                locationManager.StartUpdatingLocation ();
        }   
        #endregion
    }
}

