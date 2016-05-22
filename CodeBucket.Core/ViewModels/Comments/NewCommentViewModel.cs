using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeBucket.Core.Services;
using ReactiveUI;
using Splat;

namespace CodeBucket.Core.ViewModels.Comments
{
    public class NewCommentViewModel : ReactiveObject
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { this.RaiseAndSetIfChanged(ref _text, value); }
        }

        public IReactiveCommand<Unit> DoneCommand { get; }

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<Unit> DiscardCommand { get; }

        public NewCommentViewModel(
            Func<string, Task> doneAction,
            IAlertDialogService alertDialogService = null)
        {
            alertDialogService = alertDialogService ?? Locator.Current.GetService<IAlertDialogService>();

            DoneCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.Text).Select(x => x?.Length > 0),
                async _ =>
                {
                    await doneAction(Text);
                    Text = string.Empty;
                    DismissCommand.ExecuteIfCan();
                });

            DiscardCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                if (Text?.Length > 0)
                {
                    var result = await alertDialogService.PromptYesNo(
                        "Discard Comment", "Are you sure you want to discard this comment?");
                    if (!result)
                        return;
                }

                Text = string.Empty;
                DismissCommand.ExecuteIfCan();
            });
        }
    }
}

