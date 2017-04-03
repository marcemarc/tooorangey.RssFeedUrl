using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tooorangey.RssFeedUrl.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static string ToTimeAgo(this DateTimeOffset dateTime)
        {
            var diff = DateTime.Now - dateTime;

            if (diff.TotalMinutes < 60)
                return string.Format("{0} minute{1} ago", diff.Minutes, diff.Minutes > 1 ? "s" : "");

            if (diff.TotalHours < 24)
                return string.Format("{0} hour{1} ago", diff.Hours, diff.Hours > 1 ? "s" : "");

            if (diff.TotalDays < 7)
                return string.Format("{0} day{1} ago", Math.Ceiling(diff.TotalDays), diff.TotalDays > 1 ? "s" : "");

            if (diff.TotalDays < 30)
            {
                var weeks = Math.Floor(diff.TotalDays / 7);
                return string.Format("{0} week{1} ago", weeks, weeks > 1 ? "s" : "");
            }
            return string.Format("dd MMMM yyyy");
        }
    }
}
