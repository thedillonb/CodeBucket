using System.ComponentModel;

namespace CodeBucket.Core.Filters
{
    public class IssuesFilterModel
    {
        public string AssignedTo { get; set; }
        public string ReportedBy { get; set; }
        public StatusModel Status { get; set; }
        public KindModel Kind { get; set; }
        public PriorityModel Priority { get; set; }
		public Order OrderBy { get; set; }

        public IssuesFilterModel()
        {
            Kind = new KindModel();
            Status = new StatusModel();
            Priority = new PriorityModel();
            OrderBy = Order.Local_Id;
        }

        public IssuesFilterModel Clone()
        {
            var model = (IssuesFilterModel)MemberwiseClone();
            model.Status = Status.Clone();
            model.Kind = Kind.Clone();
            model.Priority = Priority.Clone();
            return model;
        }

        public static IssuesFilterModel CreateAllFilter()
        {
            return new IssuesFilterModel();
        }

        public static IssuesFilterModel CreateOpenFilter()
        {
            return new IssuesFilterModel 
            { 
                Status = new StatusModel 
                { 
                    New = true, Open = true, OnHold = false, Resolved = false, 
                    Wontfix = false, Duplicate = false, Invalid = false 
                }
            };
        }

        public static IssuesFilterModel CreateMineFilter(string username)
        {
            return new IssuesFilterModel { AssignedTo = username };
        }


        public bool IsFiltering()
        {
            return !(string.IsNullOrEmpty(AssignedTo) && string.IsNullOrEmpty(ReportedBy) && Status.IsDefault() && Kind.IsDefault() && Priority.IsDefault());
        }

        public enum Order
        { 
            [Description("Number")]
            Local_Id, 
            Title,
            [Description("Last Updated")]
            Utc_Last_Updated, 
            [Description("Created Date")]
            Created_On, 
            Version, 
            Milestone, 
            Component, 
            Status, 
            Priority 
        };

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(IssuesFilterModel))
                return false;
            IssuesFilterModel other = (IssuesFilterModel)obj;
            return object.Equals(AssignedTo, other.AssignedTo) && object.Equals(ReportedBy, other.ReportedBy) && object.Equals(Status, other.Status) && object.Equals(Kind, other.Kind) && object.Equals(Priority, other.Priority) && object.Equals(OrderBy, other.OrderBy);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AssignedTo != null ? AssignedTo.GetHashCode() : 0) ^ (ReportedBy != null ? ReportedBy.GetHashCode() : 0) ^ (Status != null ? Status.GetHashCode() : 0) ^ (Kind != null ? Kind.GetHashCode() : 0) ^ (Priority != null ? Priority.GetHashCode() : 0) ^ OrderBy.GetHashCode();
            }
        }

        public class StatusModel
        {
            public bool New { get; set; }
            public bool Open { get; set; }
            public bool Resolved { get; set; }
            public bool OnHold { get; set; }
            public bool Invalid { get; set; }
            public bool Duplicate { get; set; }
            public bool Wontfix { get; set; }

            public StatusModel()
            {
                New = Open = Resolved = OnHold = Invalid = Duplicate = Wontfix = true;
            }

            public bool IsDefault()
            {
                return this.Equals(new StatusModel());
            }

            public StatusModel Clone() => (StatusModel)MemberwiseClone();

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(StatusModel))
                    return false;
                StatusModel other = (StatusModel)obj;
                return New == other.New && Open == other.Open && Resolved == other.Resolved && OnHold == other.OnHold && Invalid == other.Invalid && Duplicate == other.Duplicate && Wontfix == other.Wontfix;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return New.GetHashCode() ^ Open.GetHashCode() ^ Resolved.GetHashCode() ^ OnHold.GetHashCode() ^ Invalid.GetHashCode() ^ Duplicate.GetHashCode() ^ Wontfix.GetHashCode();
                }
            }
        }

        public class KindModel
        {
            public bool Bug { get; set; }
            public bool Enhancement { get; set; }
            public bool Proposal { get; set; }
            public bool Task { get; set; }

            public KindModel()
            {
                Bug = Enhancement = Proposal = Task = true;
            }

            public bool IsDefault() => Equals(new KindModel());

            public KindModel Clone() => (KindModel)MemberwiseClone();

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(KindModel))
                    return false;
                KindModel other = (KindModel)obj;
                return Bug == other.Bug && Enhancement == other.Enhancement && Proposal == other.Proposal && Task == other.Task;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return Bug.GetHashCode() ^ Enhancement.GetHashCode() ^ Proposal.GetHashCode() ^ Task.GetHashCode();
                }
            }
        }

        public class PriorityModel
        {
            public bool Trivial { get; set; }
            public bool Minor { get; set; }
            public bool Major { get; set; }
            public bool Critical { get; set; }
            public bool Blocker { get; set; }

            public PriorityModel()
            {
                Trivial = Minor = Major = Critical = Blocker = true;
            }

            public bool IsDefault() => Equals(new PriorityModel());

            public PriorityModel Clone() => (PriorityModel)MemberwiseClone();

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;
                if (ReferenceEquals(this, obj))
                    return true;
                if (obj.GetType() != typeof(PriorityModel))
                    return false;
                PriorityModel other = (PriorityModel)obj;
                return Trivial == other.Trivial && Minor == other.Minor && Major == other.Major && Critical == other.Critical && Blocker == other.Blocker;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return Trivial.GetHashCode() ^ Minor.GetHashCode() ^ Major.GetHashCode() ^ Critical.GetHashCode() ^ Blocker.GetHashCode();
                }
            }
            
        }
    }
}

