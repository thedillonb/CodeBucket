
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeBucket.iOS
{
    public partial class Launch : UIViewController
    {
        public Launch() 
            : base("Launch", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
            
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View.BackgroundColor = UIColor.Red; //UIColor.FromRGB(51, 88, 162);
            base.ViewDidLoad();
        }
    }
}

