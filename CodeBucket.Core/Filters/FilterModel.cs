using System;

namespace CodeBucket.Core.Filters
{
    [Serializable]
    public abstract class FilterModel<TF>
    {
        public abstract TF Clone();
    }
}

