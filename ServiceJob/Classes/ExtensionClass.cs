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

        public static DateTime? ToDateTime(this string datetime, char dateSpliter = '-', char timeSpliter = ':',
            char millisecondSpliter = ',')
        {
            try
            {
                datetime = datetime.Trim();
                datetime = datetime.Replace("  ", " ");
                var body = datetime.Split(' ');
                var date = body[0].Split(dateSpliter);
                var year = int.Parse(date[0]);
                var month = int.Parse(date[1]);
                var day = int.Parse(date[2]);
                return new DateTime(year, month, day);
            }
            catch
            {
                return null;
            }
        }
        //public static IEnumerable<List<string>> GetListToModel<T>(this IEnumerable<IEnumerable<string>> source,
        //    bool onEdit = false) where T : new()
        //{
        //    var typeModel = typeof(T);
        //    var propertiesModel = typeModel.GetProperties();
        //    var convertList = new List<T>();
        //    foreach (var dataDrug in source)
        //    {
        //        var inlistmodel = (T)propertiesModel.Select((property, key) =>
        //        {
        //            var type = nameof(property.Name);
        //            return new
        //            {
        //                 property = dataDrug.ToArray()[key]
        //            };
        //        });
        //        convertList.Add(inlistmodel);
        //    }
        //    //var newlistDrugs =
        //    //    source.Select(x =>
        //    //    {
        //    //        var enumX = x.ToList();
        //    //        foreach (var parametr in enumX)
        //    //        {
        //    //            var a = model;
        //    //        }
        //    //        //return model 
        //    //        //{
        //    //        //    //Id = newKey++,
        //    //        //    //NameDrug = enumX[0],
        //    //        //    //IncludeDate = enumX[1].ToDateTime(),
        //    //        //    //OutDate = enumX[2].ToDateTime()
        //    //        //};
        //    //    }).ToList();
        //    return convertList;
        //    throw new NotImplementedException();
        //}
    }
}