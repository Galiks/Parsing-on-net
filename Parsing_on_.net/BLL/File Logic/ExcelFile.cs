using OfficeOpenXml;
using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Parsing_on_.net.BLL.File_Logic
{
    public class ExcelFile
    {
        public void WriteFile <T>(string filePath, List<T> list)
        {
            using (ExcelPackage excel = new ExcelPackage())
            {
                excel.Workbook.Worksheets.Add("Worksheet1");
                var worksheet = excel.Workbook.Worksheets["Worksheet1"];
                if (list is List<Shop>)
                {
                    Shops(list as List<Shop>, worksheet);
                }
                if (list is List<Timer>)
                {
                    Timers(list as List<Timer>, worksheet);
                }
                FileInfo excelFile = new FileInfo(filePath);
                excel.SaveAs(excelFile);
            }
        }

        private void Timers(List<Timer> list, ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].Value = "ИМЯ МЕТОДА";
            worksheet.Cells["B1"].Value = "ВРЕМЯ ВЫПОЛНЕНИЯ";
            worksheet.Cells["C1"].Value = "ДАТА ЗАМЕРА";
            for (int i = 0; i < list.Count; i++)
            {
                var index = i + 2;
                worksheet.Cells["A" + index].Value = list[i].Name;
                worksheet.Cells["B" + index].Value = list[i].Time;
                worksheet.Cells["C" + index].Value = list[i].Date;
            }
        }

        private void Shops(List<Shop> list, ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1"].Value = "НАЗВАНИЕ";
            worksheet.Cells["B1"].Value = "ЗНАЧЕНИЕ КЭШБЭКА";
            worksheet.Cells["C1"].Value = "ВАЛЮТА";
            worksheet.Cells["D1"].Value = "КАРТИНКА МАГАЗИНА";
            worksheet.Cells["E1"].Value = "АДРЕС МАГАЗИНА";
            for (int i = 0; i < list.Count; i++)
            {
                var index = i + 2;
                worksheet.Cells["A" + index].Value = list[i].Name;
                worksheet.Cells["B" + index].Value = list[i].Discount;
                worksheet.Cells["C" + index].Value = list[i].Label;
                worksheet.Cells["D" + index].Value = list[i].Image;
                worksheet.Cells["E" + index].Value = list[i].URL;
            }
        }
    }
}