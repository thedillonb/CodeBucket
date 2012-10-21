using System;
using System.Linq;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using MonoTouch.Dialog;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;

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

        public static DateTime LastUpdate (string key)
        {
            var s = Defaults.StringForKey (key);
            if (s == null)
                return DateTime.MinValue;
            long ticks;
            return Int64.TryParse (s, out ticks) ? new DateTime (ticks, DateTimeKind.Utc) : DateTime.MinValue;
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

            bool clean = name.All(c => Char.IsLetterOrDigit(c) || c == '_');
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
                new Section
                    {
                    new ActivityElement ()
                }
            };
        }

        static long _lastTime;
        [Conditional ("TRACE")]
        public static void ReportTime (string s)
        {
            long now = DateTime.UtcNow.Ticks;

            Debug.WriteLine (string.Format ("[{0}] ticks since last invoke: {1}", s, now-_lastTime));
            _lastTime = now;
        }
        
        [Conditional ("TRACE")]
        public static void Log (string format, params object [] args)
        {
            Debug.WriteLine (String.Format (format, args));
        }

        public static void LogException (string text, Exception e)
        {
            using (var s = File.AppendText (BaseDir + "/Documents/crash.log")){
                var msg = String.Format ("On {0}, message: {1}\nException:\n{2}", DateTime.Now, text, e);
                s.WriteLine (msg);
                Console.WriteLine (msg);
            }
        }

        static UIActionSheet _sheet;
        public static UIActionSheet GetSheet (string title)
        {
            _sheet = new UIActionSheet (title);
            return _sheet;
        }

        static CultureInfo _americanCulture;
        public static CultureInfo AmericanCulture {
            get { return _americanCulture ?? (_americanCulture = new CultureInfo("en-US")); }
        }


        public static void ShowAlert(string title, string message)
        {
            var alert = new UIAlertView {Title = title, Message = message};
            alert.DismissWithClickedButtonIndex(alert.AddButton("Ok"), true);
            alert.Show();
        }

        public static bool IsTall
        {
            get 
            { 
                return UIDevice.CurrentDevice.UserInterfaceIdiom 
                    == UIUserInterfaceIdiom.Phone 
                        && UIScreen.MainScreen.Bounds.Height 
                        * UIScreen.MainScreen.Scale >= 1136;
            }     
        }
    }
}

