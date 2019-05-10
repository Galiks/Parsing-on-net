using ClosedXML.Excel;
using Ninject;
using Parsing_on_.net.BLL;
using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Parsing_on_.net.Controllers
{
    public class HomeController : Controller
    {
        private static readonly ParsingLogic parsingLogic = new ParsingLogic();
        public static ParsingLogic ParsingLogic => parsingLogic;

        public static List<Shop> Shops { get; set; }

        public ActionResult Index(string action)
        {
            if (action.Equals("update"))
            {
                ParsingLogic.AddShop();
                Shops = new List<Shop>();
                Shops.AddRange(ParsingLogic.GetShops());
                return View(Shops); 
            }
            if (action.Equals("excel"))
            {
                GetFile();
            }
            if (action.Equals("csv"))
            {
                return View(Shops);
            }
            return View(Shops);
        }

        public FileResult GetFile()
        {
            WriteFile();
            // Путь к файлу
            string file_path = Server.MapPath("~/Files/shops.xls");
            // Тип файла - content-type
            string file_type = "application/pdf/txt";
            // Имя файла - необязательно
            string file_name = "shops.xls";
            return File(file_path, file_type, file_name);
        }

        private void WriteFile()
        {
            string file_path = Server.MapPath("~/Files/shops.xls");

            using (StreamWriter file = new StreamWriter(file_path, false, System.Text.Encoding.Default))
            {
                var list = Shops;
                foreach (var item in list)
                {
                    file.WriteLine($"{item.Name} {item.Discount}{item.Label} {item.Image} {item.URL}");
                }
            }
        }
    }
}