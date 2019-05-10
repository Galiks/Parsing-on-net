using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Parsing_on_.net.BLL.File_Logic
{
    public class CSVFile
    {
        public void WriteFile(string filePath, List<Shop> shops)
        {
            using (StreamWriter file = new StreamWriter(filePath, false, System.Text.Encoding.Default))
            {
                foreach (var item in shops)
                {
                    file.WriteLine($"{item.Name},{item.Discount}{item.Label},{item.Image},{item.URL}");
                }
            }
        }
    }
}