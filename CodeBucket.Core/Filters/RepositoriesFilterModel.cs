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
            Name = 0, 
            Owner = 1,
            [Description("Last Updated")]
            LastUpdated = 2,
            [Description("Created Date")]
            CreatedOn = 5, 
        };
    }
}

