using System;
using Foundation;
using UIKit;
using CodeBucket.Core.Services;

namespace CodeBucket.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public string OSVersion
        {
            get
            {
                var v = MonoTouch.Utilities.iOSVersion;
                return String.Format("{0}.{1}", v.Item1, v.Item2);
            }
        }

        public string ApplicationVersion
        {
            get
            {
                string shortVersion = string.Empty;
                string bundleVersion = string.Empty;

                try
                {
                    shortVersion = NSBundle.MainBundle.InfoDictionary["CFBundleShortVersionString"].ToString();
                } catch { }

                try
                {
                    bundleVersion = NSBundle.MainBundle.InfoDictionary["CFBundleVersion"].ToString();
                } catch { }
       
                return string.IsNullOrEmpty(bundleVersion) ? shortVersion : string.Format("{0} ({1})", shortVersion, bundleVersion);
            }
        }

        public string DeviceName
        {
            get
            {
                return UIDevice.CurrentDevice.Name;
            }
        }
    }
}

