using System;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueItemViewModel : ReactiveObject
    {
        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public IssueItemViewModel()
        {
        }
    }
}

