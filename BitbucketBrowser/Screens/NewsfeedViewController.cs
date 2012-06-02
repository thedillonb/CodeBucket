using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BitbucketBrowser
{
	[Register ("NewsfeedViewController")]
	public partial class NewsfeedViewController : UITableViewController
	{
		private readonly UISegmentedControl _segmentedView;
		
		public NewsfeedViewController() : base(UITableViewStyle.Plain)
		{
			_segmentedView = new UISegmentedControl(new object[] { "Hello", "Bob" });
			_segmentedView.ControlStyle = UISegmentedControlStyle.Bar;
		}
		
		public override void DidReceiveMemoryWarning()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidAppear(bool animated)
		{

			base.ViewDidAppear(animated);
			//NavigationController.NavigationItem.TitleView = _segmentedView;
			ParentViewController.NavigationItem.TitleView = _segmentedView;

		}
		
		public override void ViewDidUnload()
		{
			base.ViewDidUnload();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

