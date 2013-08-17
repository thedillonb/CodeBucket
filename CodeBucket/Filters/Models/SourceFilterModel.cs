using System;
using CodeFramework.Filters.Models;

namespace CodeBucket.Filters.Models
{
    public class SourceFilterModel : FilterModel
    {
        public int OrderBy { get; set; }
        public bool Ascending { get; set; }

        public SourceFilterModel()
        {
            OrderBy = (int)Order.FoldersThenFiles;
            Ascending = true;
        }

        public override FilterModel Clone()
        {
            return (SourceFilterModel)this.MemberwiseClone();
        }

        public enum Order : int
        { 
            Alphabetical, 
            [EnumDescription("Folders Then Files")]
            FoldersThenFiles,
        };
    }
}

