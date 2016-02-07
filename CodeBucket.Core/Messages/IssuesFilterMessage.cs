using System;
using MvvmCross.Plugins.Messenger;
using CodeBucket.Core.Filters;

namespace CodeBucket.Core.Messages
{
    public class IssuesFilterMessage : MvxMessage
    {
        public IssuesFilterMessage(object sender) : base(sender) {}
        public IssuesFilterModel Filter;
    }
}

