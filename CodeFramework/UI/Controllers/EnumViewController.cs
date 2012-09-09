using System;
using MonoTouch.Dialog;
using CodeFramework.UI.Elements;

namespace CodeFramework.UI.Controllers
{
    public class EnumViewController : BaseDialogViewController
    {
        public event Action<string> ValueSelected;

        protected void OnValueSelected(string value)
        {
            var handler = ValueSelected;
            if (handler != null)
                handler(value);
        }

        public EnumViewController(string title, string[] values, string selected)
            : base (true)
        {
            Title = title;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            SetValues(values, selected);
        }

        public void SetValues(string[] values, string selected)
        {
            var sec = new Section();
            foreach (var s in values)
            {
                var copy = s;
                sec.Add(new StyledElement(s, () => {
                    OnValueSelected(copy);
                }) { 
                    Accessory = s.Equals(selected, StringComparison.InvariantCultureIgnoreCase) ? 
                        MonoTouch.UIKit.UITableViewCellAccessory.Checkmark : MonoTouch.UIKit.UITableViewCellAccessory.None 
                });
            }
            Root.Add(sec);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            Root.Caption = Title;
        }
    }
}

