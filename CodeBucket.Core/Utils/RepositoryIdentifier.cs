namespace CodeBucket.Core.Utils
{
    public class RepositoryIdentifier
    {
        public string Owner { get; set; }
        public string Name { get; set; }

        public RepositoryIdentifier(string owner, string name)
        {
            Owner = owner;
            Name = name;
        }

        public static RepositoryIdentifier FromFullName(string id)
        {
            if (id == null)
                return new RepositoryIdentifier(null, null);

            var split = id.Split(new[] { '/' }, 2);
            if (split.Length != 2)
                return new RepositoryIdentifier(null, null);
            return new RepositoryIdentifier(split[0], split[1]);
        }
    }
}