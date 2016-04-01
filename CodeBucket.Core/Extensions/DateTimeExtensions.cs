namespace System
{
    public static class DateTimeExtensions
    {
        public static int TotalDaysAgo(this DateTime d)
        {
            return Convert.ToInt32(Math.Round(DateTime.Now.Subtract(d.ToLocalTime()).TotalDays));
        }

		public static int TotalDaysAgo(this DateTimeOffset d)
		{
			return Convert.ToInt32(Math.Round(DateTimeOffset.Now.Subtract(d).TotalDays));
		}
    }
}

