using UIKit;
using Foundation;
using System;

namespace CodeBucket.DialogElements
{
    public interface IElementSizing 
    {
        nfloat GetHeight (UITableView tableView, NSIndexPath indexPath);
    }
}

