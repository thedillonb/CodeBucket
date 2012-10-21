namespace CodeFramework.UI.Controllers
{
    public class FilterController : BaseDialogViewController
    {
        public FilterController()
            : base(true)
        {
            Title = "Filter";
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
        }
    }
}

