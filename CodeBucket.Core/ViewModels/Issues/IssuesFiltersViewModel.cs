using System;
using Cirrious.MvvmCross.ViewModels;
using System.Windows.Input;
using CodeBucket.Core.Filters;
using System.Linq;
using CodeBucket.Core.Messages;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssuesFiltersViewModel : BaseViewModel
    {
        public string Username { get; private set; }

        public string Repository { get; private set; }
    
        private readonly CollectionViewModel<FilterModel> _filters = new CollectionViewModel<FilterModel>();
        public CollectionViewModel<FilterModel> Filters
        {
            get { return _filters; }
        }

        public ICommand NewFilterCommand
        {
            get 
            { 
                return new MvxCommand<IssuesFilterModel>(x => {
                    var key = string.Format("{0}/{1}/issues/{2}", Username, Repository, Guid.NewGuid());
                    this.GetApplication().Account.Filters.AddFilter(key, x);
                    _filters.Items.Add(new FilterModel(this.GetApplication().Account.Filters.GetFilter(key)));
                }); 
            }
        }

        public ICommand EditFilterCommand
        {
            get 
            { 
                return new MvxCommand<FilterModel>(filter =>
                {
                    filter.Filter.SetData(filter.IssueModel);
                    this.GetApplication().Account.Filters.UpdateFilter(filter.Filter);
                    var filterString = string.Format("{0}/{1}/issues/", Username, Repository);
                    _filters.Items.Reset(this.GetApplication().Account.Filters.Where(x => x.Type.StartsWith(filterString, StringComparison.Ordinal)).Select(x => new FilterModel(x)));
                }); 
            }
        }


        public ICommand RemoveFilterCommand
        {
            get 
            { 
                return new MvxCommand<FilterModel>(x =>
                {
                    this.GetApplication().Account.Filters.RemoveFilter(x.Filter.Id);
                    _filters.Items.Remove(x);
                }); 
            }
        }

        public ICommand SelectFilterCommand
        {
            get
            {
                return new MvxCommand<FilterModel>(x =>
                {
                    Messenger.Publish(new IssuesFilterMessage(this) { Filter = x.IssueModel });
                    ChangePresentation(new MvxClosePresentationHint(this));
                });
            }
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            var filterString = string.Format("{0}/{1}/issues/", Username, Repository);
            _filters.Items.Reset(this.GetApplication().Account.Filters.Where(x => x.Type.StartsWith(filterString, StringComparison.Ordinal)).Select(x => new FilterModel(x)));
        }

        public class FilterModel
        {
            public CodeFramework.Core.Data.Filter Filter { get; set; }
            public IssuesFilterModel IssueModel { get; set; }

            public FilterModel(CodeFramework.Core.Data.Filter filter)
            {
                Filter = filter;
                IssueModel = filter.GetData<IssuesFilterModel>();
            }
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}

