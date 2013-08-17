using System;
using CodeFramework.Filters.Models;

namespace CodeBucket.Filters.Models
{
    public class RepositoriesFilterModel : FilterModel
    {
        public int OrderBy { get; set; }
        public bool Ascending { get; set; }

        public RepositoriesFilterModel()
        {
            OrderBy = (int)Order.Name;
            Ascending = true;
        }

        public override FilterModel Clone()
        {
            return (RepositoriesFilterModel)this.MemberwiseClone();
        }

        public enum Order : int
        { 
            Name, 
            Owner,
            [EnumDescription("Last Updated")]
            LastUpdated,
            Followers,
            Forks,
            [EnumDescription("Created Date")]
            CreatedOn, 
        };
    }
}

