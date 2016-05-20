using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

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

        public IReactiveCommand<Unit> DismissCommand { get; }

        public NewCommentViewModel(Func<string, Task> doneAction)
        {
            DoneCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.Text).Select(x => x.Length > 0),
                _ => doneAction(Text));

            DismissCommand = ReactiveCommand.CreateAsyncTask(
                _ => Task.FromResult(Unit.Default));
        }
    }
}

