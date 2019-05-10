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
            //var list = shops;
            using (ExcelPackage excel = new ExcelPackage())
            {
                //excel.SaveAs(excelFile);
                excel.Workbook.Worksheets.Add("Worksheet1");
                // Target a worksheet
                var worksheet = excel.Workbook.Worksheets["Worksheet1"];
                // Popular header row data
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
            for (int i = 0; i < list.Count; i++)
            {
                var index = i + 1;
                worksheet.Cells["A" + index].Value = list[i].Name;
                worksheet.Cells["B" + index].Value = list[i].Time;
                worksheet.Cells["C" + index].Value = list[i].Date;
            }
        }

        private void Shops(List<Shop> list, ExcelWorksheet worksheet)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var index = i + 1;
                worksheet.Cells["A" + index].Value = list[i].Name;
                worksheet.Cells["B" + index].Value = list[i].Discount + " " + list[i].Label;
                worksheet.Cells["C" + index].Value = list[i].Image;
                worksheet.Cells["D" + index].Value = list[i].URL;
            }
        }
    }
}