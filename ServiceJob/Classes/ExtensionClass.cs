using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceJob.Classes
{
    public static class ExtensionClass
    {
        public static IEnumerable<IEnumerable<T>> SplitArray<T>(
            this IEnumerable<T> source,
            int count)
        {
            return source
                .Select((x, y) => new {Index = y, Value = x})
                .GroupBy(x => x.Index / count)
                .Select(x => x.Select(y => y.Value).ToList())
                .ToList();
        }
        public static DateTime? ToDateTime(this string datetime, char dateSpliter = '-', char timeSpliter = ':', char millisecondSpliter = ',')
        {
            try
            {
                datetime = datetime.Trim();
                datetime = datetime.Replace("  ", " ");
                string[] body = datetime.Split(' ');
                string[] date = body[0].Split(dateSpliter);
                int year = int.Parse(date[0]);
                int month = int.Parse(date[1]);
                int day = int.Parse(date[2]);
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }
    }
}