using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ServiceJob.Interface;
using ServiceJob.Models;

namespace ServiceJob.Classes
{
    public class CalculateDrugs : ICalculateDrugs
    {
        public List<object>[] JvnlpCalculated { get; private set; }
        public List<object>[] IncludeCalculated { get; private set; }

        public async Task<string> ReadLastDateUpdate(string headerList)
        {
            string date;
            try
            {
                using var fs = new FileStream("lastdateupdate.json", FileMode.Open);
                date = await JsonSerializer.DeserializeAsync<string>(fs);
            }
            catch
            {
                var match = Regex.Match(headerList, @"(0[1-9]|[12][0-9]|3[01])[- .](0[1-9]|1[012])[- .](19|20)\d\d");
                date = match.Value;
                using var fs = new FileStream("lastdateupdate.json", FileMode.OpenOrCreate);
                await JsonSerializer.SerializeAsync(fs, date);
            }

            return date;
        }

        public List<object>[] SearchIncludeDrugs(List<object>[] regDrugs, string lastDateUpdate)
        {
            var includeDrugs = regDrugs.Where(
                (item, j) =>
                {
                    if (j <= 2)
                        return true;
                    var dateInc = item[9].ToString()
                        .Trim(' ')
                        .Substring(0, 10);
                    var isContains = DateTime.Parse(dateInc) > DateTime.Parse(lastDateUpdate);
                    return isContains;
                }).ToArray();

            return includeDrugs;
        }

        public void Start(List<object>[] regDrugs, int newColumnCount)
        {
            var narcoticDrugs = new ProcessingNDrugs().GetDrugs(); // все наркотические препараты из сохраненного списка

            var actNarcoticDrugs = narcoticDrugs
                .Where(nDrug => nDrug.OutDate.Equals(null)) // действующие наркотические препараты
                .Select(drug => drug.NameDrug.ToLower()).ToList();

            var criteriaLoads =
                new PriceCriteria<ListCriteriasModels>().LoadAsync().Result; // процентные критерии расчёта

            var typeCriterias = new Dictionary<string, string[]>
            {
                {"before50on_n", criteriaLoads.before50on.narcotik},
                {"before50on_non", criteriaLoads.before50on.nonarcotik},
                {"after50before500on_n", criteriaLoads.after50before500on.narcotik},
                {"after50before500on_non", criteriaLoads.after50before500on.nonarcotik},
                {"after500_n", criteriaLoads.after500.narcotik},
                {"after500_non", criteriaLoads.after500.nonarcotik}
            };

            var includeDrugs = SearchIncludeDrugs(
                regDrugs,
                ReadLastDateUpdate(regDrugs[0][0].ToString()).Result); // новые включенные позиции

            ExtendCellDrugs(regDrugs, includeDrugs, newColumnCount); // создание списков для расчёта

            JvnlpCalculated.CalcListJvnlp(actNarcoticDrugs, criteriaLoads, typeCriterias, newColumnCount);
            IncludeCalculated.CalcListJvnlp(actNarcoticDrugs, criteriaLoads, typeCriterias, newColumnCount);
        }

        private void ExtendCellDrugs(List<object>[] regDrugs, List<object>[] inDrugs, int newColumnCount)
        {
            JvnlpCalculated = regDrugs.Select((item, i) =>
            {
                var exItem = new object[17 + newColumnCount];
                item.CopyTo(exItem);
                return exItem.ToList();
            }).ToArray();
            JvnlpCalculated[2][11 + newColumnCount] = "Цена производителя с НДС";
            JvnlpCalculated[2][12 + newColumnCount] = "Предельная отпускная цена организаций оптовой торговли с НДС";
            JvnlpCalculated[2][13 + newColumnCount] = "Предельная отпускная цена организаций оптовой торговли без НДС";
            JvnlpCalculated[2][14 + newColumnCount] = "г.Улан-Удэ";
            JvnlpCalculated[2][15 + newColumnCount] = "Сельские районы";
            JvnlpCalculated[2][16 + newColumnCount] = "Районы приравненные к районам Крайнего Севера";

            IncludeCalculated = inDrugs.Select((item, i) =>
            {
                var exItem = new object[17 + newColumnCount];
                item.CopyTo(exItem);
                return exItem.ToList();
            }).ToArray();
            IncludeCalculated[2][11 + newColumnCount] = "Цена производителя с НДС";
            IncludeCalculated[2][12 + newColumnCount] = "Предельная отпускная цена организаций оптовой торговли с НДС";
            IncludeCalculated[2][13 + newColumnCount] =
                "Предельная отпускная цена организаций оптовой торговли без НДС";
            IncludeCalculated[2][14 + newColumnCount] = "г.Улан-Удэ";
            IncludeCalculated[2][15 + newColumnCount] = "Сельские районы";
            IncludeCalculated[2][16 + newColumnCount] = "Районы приравненные к районам Крайнего Севера";
        }
    }
}