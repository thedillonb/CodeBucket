using System;
using UIKit;
using Foundation;
using SDWebImage;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace CodeBucket.DialogElements
{
    public class CheckElement : StringElement
    {
        private readonly Subject<object> _tapped = new Subject<object>();
        public IObservable<object> Clicked
        {
            get { return _tapped.AsObservable(); }
        }

        private bool _checked;
        public bool Checked
        {
            get { return _checked; }
            set
            {
                if (value == _checked)
                    return;

                _checked = value;
                Accessory = _checked ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            }
        }

        public CheckElement(
            string caption = null, string value = null,
            UITableViewCellStyle style = UITableViewCellStyle.Value1) 
            : base (caption, value, style) 
        {
            Checked = false;
        }

        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);
            _tapped.OnNext(this);
        }
    }

    public class ButtonElement : StringElement
    {
        private readonly Subject<object> _tapped = new Subject<object>();
        public IObservable<object> Clicked
        {
            get { return _tapped.AsObservable(); }
        }

        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (value == _enabled)
                    return;

                _enabled = value;
                SelectionStyle = _enabled ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;
            }
        }

        private UITableViewCellSelectionStyle _selectionStyle = UITableViewCellSelectionStyle.None;
        public UITableViewCellSelectionStyle SelectionStyle
        {
            get { return _selectionStyle; }
            set
            {
                if (_selectionStyle == value)
                    return;

                _selectionStyle = value;
                var cell = GetActiveCell();
                if (cell != null)
                    cell.SelectionStyle = value;
            }
        }

        public ButtonElement (
            string caption, string value, 
            UITableViewCellStyle style = UITableViewCellStyle.Value1, 
            UIImage image = null) 
            : base (caption, value, style) 
        {
            Image = image;
            Enabled = true;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public ButtonElement(string caption, UIImage image = null)
            : this(caption, null, image: image)
        {
        }

        public ButtonElement(UIImage image = null)
            : this(null, null, image: image)
        {
        }

        protected override UITableViewCell InitializeCell(UITableViewCell cell)
        {
            var c = base.InitializeCell(cell);
            c.SelectionStyle = SelectionStyle;
            return c;
        }

        public override void Selected(UITableView tableView, NSIndexPath path)
        {
            base.Selected(tableView, path);

            if (Enabled)
                _tapped.OnNext(this);
        }
    }

    public class LoaderButtonElement : ButtonElement
    {
        private UIActivityIndicatorView _activityView;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (value == _isLoading)
                    return;
                
                _isLoading = value;
                _activityView = _isLoading ? new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray) : null;
                _activityView?.StartAnimating();

                var cell = GetActiveCell();
                if (cell != null)
                    cell.AccessoryView = _activityView;
            }
        }

        public LoaderButtonElement(string caption, UIImage image) 
            : base (caption, image: image) 
        {
        }

        protected override string GetKey(int style)
        {
            return "loader-button";
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            _activityView?.StartAnimating();
            cell.AccessoryView = _activityView;
            return cell;
        }
    }

    public class StringElement : Element 
    {
        public static UIColor DefaultTitleColor = UIColor.FromRGB(41, 41, 41);
        public static UIColor DefaultDetailColor = UIColor.FromRGB(80, 80, 80);
        public static UIColor BgColor = UIColor.White;

        static NSString [] skey = { new NSString (".1"), new NSString (".2"), new NSString (".3"), new NSString (".4") };

        public UITableViewCellStyle Style;
        public UIFont Font;
        public UIFont SubtitleFont;
        public UIColor TextColor;
        private UIImage _image;
        private Uri _imageUri;
        private string _value;
        private UITableViewCellAccessory _accessory = UITableViewCellAccessory.None;

        private string _caption;
        public string Caption
        {
            get { return _caption; }
            set
            {
                if (_caption == value)
                    return;

                _caption = value;
                var cell = GetActiveCell();
                if (cell != null && cell.TextLabel != null)
                    cell.TextLabel.Text = value ?? string.Empty;
            }
        }

        public string Value
        {
            get { return _value; }
            set
            {
                if (_value == value)
                    return;
                
                _value = value;
                var cell = GetActiveCell();
                if (cell != null && cell.DetailTextLabel != null)
                    cell.DetailTextLabel.Text = value ?? string.Empty;
            }
        }

        public UITableViewCellAccessory Accessory
        {
            get { return _accessory; }
            set
            {
                if (_accessory == value)
                    return;
                
                _accessory = value;
                var cell = GetActiveCell();
                if (cell != null)
                    cell.Accessory = value;
            }
        }

        private int _lines;
        public int Lines
        {
            get { return _lines; }
            set
            {
                if (_lines == value)
                    return;
                
                _lines = value;
                var cell = GetActiveCell();
                if (cell != null)
                    cell.TextLabel.Lines = value;
            }
        }

        private UILineBreakMode _lineBreakMode;
        public UILineBreakMode LineBreakMode
        {
            get { return _lineBreakMode; }
            set
            {
                if (_lineBreakMode == value)
                    return;
                
                _lineBreakMode = value;
                var cell = GetActiveCell();
                if (cell != null)
                    cell.TextLabel.LineBreakMode = value;
            }
        }

        public UIColor ImageTintColor { get; set; }

        public StringElement()
        {
            Lines = 1;
            Font = UIFont.PreferredBody;
            SubtitleFont = UIFont.PreferredSubheadline;
            TextColor = DefaultTitleColor;
        }


        public StringElement (UIImage image)
            : this(null, image)
        {
        }

        public StringElement (string caption)
            : this()
        {
            Caption = caption;
        }

        public StringElement (string caption, UIImage image) 
            : this (caption) 
        {
            Image = image;
        }

        public StringElement (string caption, string value) 
            : this (caption) 
        {
            Style = UITableViewCellStyle.Value1;
            Value = value;
        }

        public StringElement (string caption, string value, UITableViewCellStyle style)
            : this (caption, value) 
        { 
            this.Style = style;
        }

        public StringElement (string caption, UITableViewCellStyle style)
            : this (caption, null, style) 
        { 
        }

        public UIImage Image {
            get { return _image; }
            set 
            {
                _image = value;
                var cell = GetActiveCell();
                if (cell != null)
                    cell.ImageView.Image = value;
            }
        }

        // Loads the image from the specified uri (use this or Image)
        public Uri ImageUri {
            get { return _imageUri; }
            set {
                if (_imageUri == value)
                    return;
                
                _imageUri = value;
                var cell = GetActiveCell();
                if (cell != null && value != null)
                    cell.ImageView.SetImage(new NSUrl(value.AbsoluteUri));
            }
        }

        protected virtual string GetKey (int style)
        {
            return skey [style];
        }

        protected virtual UITableViewCell CreateTableViewCell(UITableViewCellStyle style, string key)
        {
            return new UITableViewCell (style, key);
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var key = GetKey ((int) Style);
            var cell = tv.DequeueReusableCell(key) ?? CreateTableViewCell(Style, key);
            return InitializeCell(cell);
        }

        protected virtual UITableViewCell InitializeCell(UITableViewCell cell)
        {
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            cell.TextLabel.Text = Caption;
            cell.TextLabel.TextColor = TextColor;
            cell.ImageView.Image = Image;
            cell.Accessory = Accessory;

            if (ImageUri != null)
                cell.ImageView.SetImage(new NSUrl(ImageUri.AbsoluteUri), Image);

            if (cell.DetailTextLabel != null)
            {
                cell.DetailTextLabel.Text = Value ?? "";
                cell.DetailTextLabel.TextColor = DefaultDetailColor;
            }
            return cell;
        }

        public override bool Matches (string text)
        {
            var cap = Caption ?? string.Empty;
            var val = Value ?? string.Empty;
            return cap.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1 || val.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1;
        }
    }
}

