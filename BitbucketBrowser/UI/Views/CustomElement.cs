using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;


namespace BitbucketBrowser.UI
{
    public abstract class CustomElement : Element, IElementSizing, IColorizeBackground
    {       
        public string CellReuseIdentifier
        {
            get;set;    
        }

        public UITableViewCellStyle Style
        {
            get;set;    
        }

        public UIColor BackgroundColor { get; set; }

        public CustomElement (UITableViewCellStyle style, string cellIdentifier) : base(null)
        {
            this.CellReuseIdentifier = cellIdentifier;
            this.Style = style;
        }

        public float GetHeight (UITableView tableView, NSIndexPath indexPath)
        {
            try
            {
                if (tableView.Style == UITableViewStyle.Grouped)
                    return Height(new RectangleF(tableView.Bounds.Location, new SizeF(tableView.Bounds.Width - 20, tableView.Bounds.Height)));
                else
                    return Height(tableView.Bounds);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Attempted to get cell height resulted in exception: " + e.Message);
            }
            return 0f;
        }

        
        public event NSAction Tapped;

        public bool IsTappedAssigned { get { return Tapped != null; } }

        protected virtual void OnCreateCell(UITableViewCell cell)
        {
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            OwnerDrawnCell cell = tv.DequeueReusableCell(this.CellReuseIdentifier) as OwnerDrawnCell;

            if (cell == null)
            {
                cell = new OwnerDrawnCell(this, this.Style, this.CellReuseIdentifier);
                OnCreateCell(cell);
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

            cell.BackgroundColor = BackgroundColor;
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
                view.SetNeedsDisplay();
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
                base.Draw(rect);
                try
                {
                    CGContext context = UIGraphics.GetCurrentContext();
                    element.Draw(rect, context, this);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to draw: " + e.Message);
                }
            }
        }

        public virtual void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            cell.BackgroundColor = BackgroundColor;
        }

    }
}

