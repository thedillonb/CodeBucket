using System;
using UIKit;
using System.Linq;
using CodeBucket.Views;
using System.Reactive.Linq;
using CodeBucket.DialogElements;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers
{
    public abstract class PrettyDialogViewController<TViewModel> : BaseTableViewController<TViewModel> where TViewModel : class
    {
        private readonly Lazy<RootElement> _rootElement;
        protected readonly SlideUpTitleView SlideUpTitle;
        protected readonly ImageAndTitleHeaderView HeaderView;
        private readonly UIView _backgroundHeaderView;

        public override string Title
        {
            get
            {
                return base.Title;
            }
            set
            {
                HeaderView.Text = value;
                SlideUpTitle.Text = value;
                base.Title = value;
                RefreshHeaderView();
            }
        }

        public RootElement Root => _rootElement.Value;

        protected PrettyDialogViewController()
            : base(UITableViewStyle.Grouped)
        {
            _rootElement = new Lazy<RootElement>(() => new RootElement(TableView));
            _backgroundHeaderView = new UIView();

            HeaderView = new ImageAndTitleHeaderView();
            SlideUpTitle = new SlideUpTitleView(44f) { Offset = 100f };
            NavigationItem.TitleView = SlideUpTitle;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationController.NavigationBar.ShadowImage = new UIImage();
            HeaderView.BackgroundColor = NavigationController.NavigationBar.BarTintColor;
            HeaderView.TextColor = NavigationController.NavigationBar.TintColor;
            HeaderView.SubTextColor = NavigationController.NavigationBar.TintColor.ColorWithAlpha(0.8f);
            _backgroundHeaderView.BackgroundColor = HeaderView.BackgroundColor;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (NavigationController != null)
                NavigationController.NavigationBar.ShadowImage = null;
        }

        protected void RefreshHeaderView(string text = null, string subtext = null, UIImage image = null)
        {
            HeaderView.Text = text ?? HeaderView.Text;
            HeaderView.SubText = subtext ?? HeaderView.SubText;
            HeaderView.Image = image ?? HeaderView.Image;
            TableView.TableHeaderView = HeaderView;
            TableView.ReloadData();
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            TableView.BeginUpdates();
            TableView.TableHeaderView = HeaderView;
            TableView.EndUpdates();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new DialogTableViewSource(Root);
            TableView.Source = source;
            TableView.TableHeaderView = HeaderView;
            TableView.SectionHeaderHeight = 0;

            var frame = TableView.Bounds;
            frame.Y = -frame.Size.Height;
            _backgroundHeaderView.Frame = frame;
            _backgroundHeaderView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            _backgroundHeaderView.Layer.ZPosition = -1f;
            TableView.InsertSubview(_backgroundHeaderView, 0);

            OnActivation(disposable =>
            {
                source
                    .DidScrolled
                    .Where(_ => NavigationController != null)
                    .Subscribe(p =>
                    {
                        if (p.Y > 0)
                            NavigationController.NavigationBar.ShadowImage = null;
                        if (p.Y <= 0 && NavigationController.NavigationBar.ShadowImage == null)
                            NavigationController.NavigationBar.ShadowImage = new UIImage();
                        SlideUpTitle.Offset = 108 + 28 - p.Y;
                    })
                    .AddTo(disposable);
            });
        }
    }
}

