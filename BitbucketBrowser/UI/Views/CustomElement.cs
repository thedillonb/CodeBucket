using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;


namespace BitbucketBrowser.UI
{
    public abstract class CustomElement : Element, IElementSizing
    {       
        public string CellReuseIdentifier
        {
            get;set;    
        }

        public UITableViewCellStyle Style
        {
            get;set;    
        }

        public CustomElement (UITableViewCellStyle style, string cellIdentifier) : base(null)
        {
            this.CellReuseIdentifier = cellIdentifier;
            this.Style = style;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            return Height(tableView.Bounds);
        }

        
        public event NSAction Tapped;


        public override UITableViewCell GetCell (UITableView tv)
        {
            OwnerDrawnCell cell = tv.DequeueReusableCell(this.CellReuseIdentifier) as OwnerDrawnCell;

            if (cell == null)
            {
                cell = new OwnerDrawnCell(this, this.Style, this.CellReuseIdentifier);
            }
            else
            {
                cell.Element = this;
            }

            if (Tapped != null) {
                cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
            } 
            else 
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            }

            cell.BackgroundColor = UIColor.White;
            cell.Update();
            return cell;
        }   

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            if (Tapped != null)
                Tapped();
            tableView.DeselectRow (path, true);
        }

        public abstract void Draw(RectangleF bounds, CGContext context, UIView view);

        public abstract float Height(RectangleF bounds);

        class OwnerDrawnCell : UITableViewCell
        {
            OwnerDrawnCellView view;

            public OwnerDrawnCell(CustomElement element, UITableViewCellStyle style, string cellReuseIdentifier) : base(style, cellReuseIdentifier)
            {
                Element = element;
            }

            public CustomElement Element
            {
                get {
                    return view.Element;
                }
                set {
                    if (view == null)
                    {
                        view = new OwnerDrawnCellView (value);
                        ContentView.Add (view);
                    }
                    else
                    {
                        view.Element = value;
                    }
                }
            }



            public void Update()
            {
                SetNeedsDisplay();
                view.SetNeedsDisplay();
            }       

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();

                view.Frame = ContentView.Bounds;
            }
        }

        class OwnerDrawnCellView : UIView
        {               
            CustomElement element;

            public OwnerDrawnCellView(CustomElement element)
            {
                this.element = element;
                this.BackgroundColor = UIColor.Clear;
            }


            public CustomElement Element
            {
                get { return element; }
                set {
                    element = value; 
                }
            }

            public void Update()
            {
                SetNeedsDisplay();

            }

            public override void Draw (RectangleF rect)
            {
                CGContext context = UIGraphics.GetCurrentContext();
                element.Draw(rect, context, this);
            }
        }
    }
}

