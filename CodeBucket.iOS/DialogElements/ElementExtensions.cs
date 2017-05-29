using System;
using ReactiveUI;
using UIKit;
using System.Reactive.Disposables;
using System.Reactive;

namespace CodeBucket.DialogElements
{
    public static class ElementExtensions
    {
        public static IDisposable BindLoader<T>(this LoaderButtonElement @this, ReactiveCommand<Unit, T> cmd)
        {
            var invoke = @this.Clicked.SelectUnit().BindCommand(cmd);
            var canExecute = cmd.CanExecute.Subscribe(x => @this.Enabled = x);
            var isExecuting = cmd.IsExecuting.Subscribe(x => @this.IsLoading = x);
            return new CompositeDisposable(isExecuting, invoke, canExecute);
        }

        public static IDisposable BindClick(this ButtonElement @this, ReactiveCommand<Unit, Unit> cmd)
        {
            return @this.Clicked.SelectUnit().BindCommand(cmd);
        }

        public static IDisposable BindCaption<T>(this StringElement stringElement, IObservable<T> caption)
        {
            return caption.Subscribe(x => stringElement.Caption = x?.ToString());
        }

        public static IDisposable BindValue<T>(this StringElement stringElement, IObservable<T> value)
        {
            return value.Subscribe(x => stringElement.Value = x?.ToString());
        }

        public static IDisposable BindDisclosure(this ButtonElement stringElement, IObservable<bool> value)
        {
            return value.Subscribe(x => {
                stringElement.SelectionStyle = x ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;
                stringElement.Accessory = x ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
            });
        }

        public static IDisposable BindText<T>(this SplitButtonElement.Button button, IObservable<T> value)
        {
            return value.Subscribe(x => button.Text = x?.ToString());
        }

        public static IDisposable BindText<T>(this SplitViewElement.SplitButton button, IObservable<T> value)
        {
            return value.Subscribe(x => button.Text = x?.ToString());
        }
    }
}

