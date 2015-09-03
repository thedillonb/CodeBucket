using System.ComponentModel;

namespace CodeBucket.Core.Filters
{
    public class RepositoriesFilterModel : FilterModel<RepositoriesFilterModel>
    {
		public Order OrderBy { get; set; }
        public bool Ascending { get; set; }

        public RepositoriesFilterModel()
        {
            OrderBy = Order.Name;
            Ascending = true;
        }

        public override RepositoriesFilterModel Clone()
        {
            return (RepositoriesFilterModel)this.MemberwiseClone();
        }

        public enum Order : int
        { 
            Name, 
            Owner,
            [Description("Last Updated")]
            LastUpdated,
            Followers,
            Forks,
            [Description("Created Date")]
            CreatedOn, 
        };
    }
}

