using System;
using System.Collections.Generic;
using System.Linq;
using ServiceJob.Models;

namespace ServiceJob.Classes
{
    /// <summary>
    ///     Класс расширений
    ///     SplitArray создании массива коллекции из строки
    ///     ToDateTime проверка на создание формата даты год-месяц-день в случае неуспеха вернуть null
    ///     WhereIf создание множественной фильтрации коллекции: условие true -> применение фильтра -> вернуть новую коллекцию;
    ///     WhereIf условие false -> вернуть текущую коллекцию
    /// </summary>
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

        /// <summary>
        ///     WhereIf - Фильтрация данных
        /// </summary>
        /// <typeparam name="T">Тип коллекции данных</typeparam>
        /// <param name="source">Коллекция данных</param>
        /// <param name="condition">Условие</param>
        /// <param name="predicate">Условие которое надо применить к коллекции</param>
        /// <returns>Отфильтрованные данные</returns>
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition,
            Func<T, int, bool> predicate)
        {
            return condition ? WhereIterator(source, predicate) : source;
        }

        private static IEnumerable<TSource> WhereIterator<TSource>(IEnumerable<TSource> source,
            Func<TSource, int, bool> predicate)
        {
            var index = -1;
            foreach (var element in source)
            {
                checked
                {
                    index++;
                }

                if (predicate(element, index)) yield return element;
            }
        }

        public static string GetNameSelection(this double originalPrice)
        {
            if (originalPrice <= 50.0d) return "before50on";

            if (originalPrice > 50.0d && originalPrice <= 500.0d) return "after50before500on";

            return originalPrice > 500.0d ? "after500" : "";
        }

        public static void CalcListJvnlp(this List<object>[] listDrugs, List<string> actNarcoticDrugs,
            ListCriteriasModels criteriaLoads, Dictionary<string, string[]> typeCriterias, int newColumnCount)
        {
            for (var i = 3; i < listDrugs.Length; i++)
            {
                var drug = listDrugs[i];
                var isContainsNarcotic = actNarcoticDrugs.Any(nDrug => nDrug.Equals(drug[0].ToString().ToLower()));
                var originalPrice = Convert.ToDouble(drug[6]);
                var nds = double.Parse(criteriaLoads.nds);
                drug[11 + newColumnCount] = originalPrice * Convert.ToDouble(nds); // рассчёт цены с НДС
                var selection = originalPrice.GetNameSelection(); // Ценовая группа
                var typeDrug = isContainsNarcotic ? "_n" : "_non"; // Наркотический или ненаркотический
                var criterias =
                    typeCriterias[selection + typeDrug].Select(double.Parse)
                        .ToArray(); // критерии рассчёта для текущего препарата
                drug.CalcCriteria(criterias, originalPrice, nds, newColumnCount); // рассчёт
            }
        }

        private static void CalcCriteria(this List<object> price, double[] currentCriteria, double originalPrice,
            double nds, int newColumnCount)
        {
            price[12 + newColumnCount] = (originalPrice + originalPrice * (currentCriteria[0] / 100)) * nds;
            for (int cell = 13 + newColumnCount, i = 0; i < currentCriteria.Length; cell++, i++)
            {
                if (cell == 13 + newColumnCount)
                {
                    price[cell] = originalPrice + originalPrice * (currentCriteria[0] / 100);
                    continue;
                }

                var notNds = double.Parse(price[13 + newColumnCount].ToString());
                price[cell] = (notNds + originalPrice * (currentCriteria[i] / 100)) * nds;
            }
        }
    }
}