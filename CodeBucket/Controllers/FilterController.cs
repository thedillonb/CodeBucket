using System.Collections.Generic;
using CodeBucket.Elements;
using MonoTouch.UIKit;
using System.Linq;
using System;

namespace CodeBucket.Controllers
{

    public abstract class FilterController : BaseDialogViewController
    {
        public static int[] IntegerCeilings = new[] { 6, 11, 21, 31, 41, 51, 61, 71, 81, 91, 101, 251, 501, 1001, 2001, 4001, 8001, 16001, int.MaxValue };

        public FilterController()
            : base(true)
        {
            Title = "Filter & Sort";
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
        }

        public abstract void ApplyFilter();

        public class EnumChoiceElement : StyledElement
        {
            public int Obj;
            public EnumChoiceElement(string title, string defaultVal, IEnumerable<string> values)
                : base(title, defaultVal, UITableViewCellStyle.Value1)
            {
                Accessory = UITableViewCellAccessory.DisclosureIndicator;
                int i = 0;
                foreach (var a in values)
                {
                    if (a.Equals(defaultVal))
                    {
                        Obj = i;
                        break;
                    }
                    i++;
                }
            }
        }

        protected EnumChoiceElement CreateEnumElement(string title, string defaultVal, IEnumerable<string> values)
        {
            var element = new EnumChoiceElement(title, defaultVal, values);
            element.Tapped += () =>
            {
                var en = new RadioChoiceViewController(element.Caption, values, element.Value);
                en.ValueSelected += obj =>
                {
                    element.Value = obj;

                    int i = 0;
                    foreach (var a in values)
                    {
                        if (a.Equals(obj))
                        {
                            element.Obj = i;
                            break;
                        }
                        i++;
                    }

                    NavigationController.PopViewControllerAnimated(true);
                };
                NavigationController.PushViewController(en, true);
            };
            
            return element;
        }

        protected EnumChoiceElement CreateEnumElement(string title, int defaultVal, System.Type enumType)
        {
            var values = new List<string>();
            foreach (var x in System.Enum.GetValues(enumType).Cast<System.Enum>())
                values.Add(x.Description());

            return CreateEnumElement(title, values[defaultVal], values);
        }

        public class MultipleChoiceElement<T> : StyledElement
        {
            public T Obj;
            public MultipleChoiceElement(string title, T obj)
                : base(title, CreateCaptionForMultipleChoice(obj), UITableViewCellStyle.Value1)
            {
                Obj = obj;
                Accessory = UITableViewCellAccessory.DisclosureIndicator;
            }
        }

        protected MultipleChoiceElement<T> CreateMultipleChoiceElement<T>(string title, T o)
        {
            var element = new MultipleChoiceElement<T>(title, o);
            element.Tapped += () =>
            {
                var en = new MultipleChoiceViewController<T>(element.Caption, o);
                en.ViewDisappearing += (sender, e) => {
                    element.Value = CreateCaptionForMultipleChoice(o);
                };
                NavigationController.PushViewController(en, true);
            };

            return element;
        }

        private static string CreateCaptionForMultipleChoice<T>(T o)
        {
            var fields = o.GetType().GetFields();
            var sb = new System.Text.StringBuilder();
            int trueCounter = 0;
            foreach (var f in fields)
            {
                if ((bool)f.GetValue(o))
                {
                    sb.Append(f.Name);
                    sb.Append(", ");
                    trueCounter++;
                }
            }
            var str = sb.ToString();
            if (str.EndsWith(", "))
            {
                if (trueCounter == fields.Length)
                    return "Any";
                else
                    return str.Substring(0, str.Length - 2);
            }
            return "None";
        }
    }
}

