using System;
using System.Collections.Generic;
using CodeBucket.Core.Utils;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Events
{
    public class EventItemViewModel : ReactiveObject, ICanGoToViewModel
    {
        public Avatar Avatar { get; }

        public IList<EventTextBlock> Header { get; } = new List<EventTextBlock>(6);

        public IList<EventTextBlock> Body { get; } = new List<EventTextBlock>();

        public Action Tapped { get; set; }

        public bool Multilined { get; set; }

        public string EventType { get; }

        public string CreatedOn { get; }

        public IReactiveCommand<object> GoToCommand { get; } = ReactiveCommand.Create();

        public EventItemViewModel(Avatar avatar, string eventType, string createdOn)
        {
            Avatar = avatar;
            EventType = eventType;
            CreatedOn = createdOn;
            GoToCommand.Subscribe(_ => Tapped?.Invoke());
        }
    }

    public class EventTextBlock
    {
        public string Text { get; set; }

        public EventTextBlock()
        {
        }

        public EventTextBlock(string text)
        {
            Text = text;
        }
    }

    public class EventAnchorBlock : EventTextBlock
    {
        public EventAnchorBlock(string text, Action tapped) : base(text)
        {
            Tapped = tapped;
        }

        public Action Tapped { get; set; }

        public EventAnchorBlock(Action tapped)
        {
            Tapped = tapped;
        }
    }
}

