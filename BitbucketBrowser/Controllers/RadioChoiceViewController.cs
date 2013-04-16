using System;
using MonoTouch.Dialog;
using BitbucketBrowser.Elements;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace BitbucketBrowser.Controllers
{
    public class RadioChoiceViewController : BaseDialogViewController
    {
        public event Action<string> ValueSelected;

        protected void OnValueSelected(string value)
        {
            var handler = ValueSelected;
            if (handler != null)
                handler(value);
        }

        public RadioChoiceViewController(string title, IEnumerable<string> values, string selected)
            : this(title)
        {
            SetValues(values, selected);
        }

        public RadioChoiceViewController(string title)
            : base (true)
        {
            Title = title;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
        }

        public void SetValues(IEnumerable<string> values, string selected)
        {
            var sec = new Section();
            foreach (var s in values)
            {
                var copy = s;
                sec.Add(new StyledElement(s, () => OnValueSelected(copy)) { 
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

