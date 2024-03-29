﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExcelDataReader;
using Newtonsoft.Json;

namespace ServiceJob.Models
{
    public class UploadFile
    {
        public DataTable Drugs { get; protected set; }
        public DataTable RemovedDrugs { get; protected set; }

        public string NewDateUpdate { get; protected set; }
        public int NewColumnCount { get; protected set; }

        public List<List<object>[]> ReadFileJvnlp(StreamReader fileMemoryStream, string fileName)
        {
            IExcelDataReader bookExcel = null;
            // register provide encoding 1252 to Excel
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            try
            {
                //Reading from a binary Excel file ('97-2003 format; *.xls)
                if (fileName.Split(".").LastOrDefault().Equals("xls"))
                    bookExcel = ExcelReaderFactory.CreateBinaryReader(fileMemoryStream.BaseStream);

                //Reading from a OpenXml Excel file (2007 format; *.xlsx)
                if (fileName.Split(".").LastOrDefault().Equals("xlsx"))
                    bookExcel = ExcelReaderFactory.CreateOpenXmlReader(fileMemoryStream.BaseStream);

                //DataSet - The result of each spreadsheet will be created in the result.Tables
                var tableJvnlp = bookExcel?.AsDataSet();
                CreateTablesJvnlpAsync(tableJvnlp);

                if (Drugs == null || RemovedDrugs == null)
                    return new List<List<object>[]>();

                var drugs = Drugs.Rows.Cast<DataRow>().Select(x => x.ItemArray.ToList()).ToArray();
                var rmDrugs = RemovedDrugs.Rows.Cast<DataRow>().Select(x => x.ItemArray.ToList()).ToArray();
                RemoveNullColumns(drugs, rmDrugs);
                bookExcel?.Close();
                fileMemoryStream.Close();
                LoadDateUpdate();
                return new List<List<object>[]>
                {
                    drugs,
                    rmDrugs
                };
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private void LoadDateUpdate()
        {
            var date = Drugs.Rows[0].ItemArray[0].ToString();
            var match = Regex.Match(date, @"(0[1-9]|[12][0-9]|3[01])[- .](0[1-9]|1[012])[- .](19|20)\d\d");
            NewDateUpdate = match.Value;
        }

        private void CreateTablesJvnlpAsync(DataSet dataJvnlp)
        {
            var json = File.ReadAllText("settingFileJvnlp.json");
            var nameList = JsonConvert.DeserializeObject<StructJvnlp>(json);
            NewColumnCount = nameList.AddColumnCount;

            Drugs = dataJvnlp.Tables[nameList.MainList];
            RemovedDrugs = dataJvnlp.Tables[nameList.ExList];
        }

        private void RemoveNullColumns(List<object>[] drugs, List<object>[] rmDrugs)
        {
            try
            {
                for (var i = drugs[2].Count - 1; i != 0; i--)
                {
                    var obj = drugs[2][i].ToString();
                    if (!obj.Equals("")) continue;
                    foreach (var drug in drugs) drug.RemoveAt(i);
                }

                for (var i = rmDrugs[2].Count - 1; i != 0; i--)
                {
                    var obj = rmDrugs[2][i].ToString();
                    if (!obj.Equals("")) continue;
                    foreach (var rmDrug in rmDrugs) rmDrug.RemoveAt(i);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    public class StructJvnlp
    {
        public string MainList { get; set; }
        public string ExList { get; set; }
        public int AddColumnCount { get; set; }
    }
}