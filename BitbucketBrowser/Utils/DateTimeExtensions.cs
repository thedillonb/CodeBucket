namespace System
{
    public static class DateTimeExtensions
    {
        public static string ToDaysAgo(this DateTime d)
        {
            var dt = DateTime.Now.Subtract(d.ToLocalTime());
            if (dt.TotalDays > 1)
                return Convert.ToInt32(dt.TotalDays) + " days ago";
            if (dt.TotalHours > 1)
                return Convert.ToInt32(dt.TotalHours) + " hours ago";
            if (dt.TotalMinutes > 1)
                return Convert.ToInt32(dt.TotalMinutes) + " minutes ago";
            return "moments ago";
        }

        public static int TotalDaysAgo(this DateTime d)
        {
            return Convert.ToInt32(Math.Round(DateTime.Now.Subtract(d.ToLocalTime()).TotalDays));
        }
    }
}

