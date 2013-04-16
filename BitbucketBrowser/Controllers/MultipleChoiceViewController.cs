using System;
using MonoTouch.Dialog;
using BitbucketBrowser.Elements;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;

namespace BitbucketBrowser.Controllers
{
    public class MultipleChoiceViewController<T> : BaseDialogViewController
    {
        private T _obj;
        
        protected void OnValueSelected(System.Reflection.FieldInfo field)
        {
            var r = Root[0].Elements.Find(x => x.Caption.Equals(field.Name));
            if (r == null)
                return;
            var e = (StyledElement)r;
            var value = (bool)field.GetValue(_obj);
            field.SetValue(_obj, !value);
            e.Accessory = !value ? MonoTouch.UIKit.UITableViewCellAccessory.Checkmark : MonoTouch.UIKit.UITableViewCellAccessory.None;
            Root.Reload(e, UITableViewRowAnimation.None);
        }
        
        public MultipleChoiceViewController(string title, T obj)
            : base (true)
        {
            _obj = obj;
            Title = title;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;

            var sec = new Section();
            var fields = obj.GetType().GetFields();
            foreach (var s in fields)
            {
                var copy = s;
                sec.Add(new StyledElement(s.Name, () => OnValueSelected(copy)) { 
                    Accessory = (bool)s.GetValue(_obj) ? MonoTouch.UIKit.UITableViewCellAccessory.Checkmark : MonoTouch.UIKit.UITableViewCellAccessory.None 
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

