namespace CodeBucket.Views
{
    public class DescriptionModel
    {
        public string Body { get; }

        public int FontSize { get; }

        public bool ContinuousResize { get; }

        public DescriptionModel(string body, int fontSize, bool continuousResize = false)
        {
            Body = body;
            FontSize = fontSize;
            ContinuousResize = continuousResize;
        }
    }
}

