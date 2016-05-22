using System;
using ReactiveUI;
using UIKit;
using System.Reactive.Disposables;

namespace CodeBucket.DialogElements
{
    public static class ElementExtensions
    {
        public static IDisposable BindLoader<T>(this LoaderButtonElement @this, IReactiveCommand<T> cmd)
        {
            var invoke = @this.Clicked.InvokeCommand(cmd);
            var canExecute = cmd.CanExecuteObservable.Subscribe(x => @this.Enabled = x);
            var isExecuting = cmd.IsExecuting.Subscribe(x => @this.IsLoading = x);
            return new CompositeDisposable(isExecuting, invoke, canExecute);
        }

        public static IDisposable BindClick<T>(this ButtonElement @this, IReactiveCommand<T> cmd)
        {
            return @this.Clicked.InvokeCommand(cmd);
        }

        public static IDisposable BindCaption<T>(this StringElement stringElement, IObservable<T> caption)
        {
            return caption.SubscribeSafe(x => stringElement.Caption = x.ToString());
        }

        public static IDisposable BindValue<T>(this StringElement stringElement, IObservable<T> value)
        {
            return value.SubscribeSafe(x => stringElement.Value = x.ToString());
        }

        public static IDisposable BindDisclosure(this ButtonElement stringElement, IObservable<bool> value)
        {
            return value.SubscribeSafe(x => {
                stringElement.SelectionStyle = x ? UITableViewCellSelectionStyle.Default : UITableViewCellSelectionStyle.None;
                stringElement.Accessory = x ? UITableViewCellAccessory.DisclosureIndicator : UITableViewCellAccessory.None;
            });
        }

        public static IDisposable BindText<T>(this SplitButtonElement.Button button, IObservable<T> value)
        {
            return value.SubscribeSafe(x => button.Text = x.ToString());
        }

        public static IDisposable BindText<T>(this SplitViewElement.SplitButton button, IObservable<T> value)
        {
            return value.SubscribeSafe(x => button.Text = x.ToString());
        }
    }
}

