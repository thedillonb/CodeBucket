using System;
namespace System
{
    public static class DateTimeExtensions
    {
        public static string ToDaysAgo(this DateTime d)
        {
            var dt = DateTime.Now.Subtract(d);
            if (dt.TotalDays > 1)
                return Convert.ToInt32(dt.TotalDays) + " days ago";
            else if (dt.TotalHours > 1)
                return Convert.ToInt32(dt.TotalHours) + " hours ago";
            else if (dt.TotalMinutes > 1)
                return Convert.ToInt32(dt.TotalMinutes) + " minutes ago";
            else
                return "moments ago";
        }
    }
}

