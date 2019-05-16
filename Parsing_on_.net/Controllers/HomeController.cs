using Parsing_on_.net.BLL;
using Parsing_on_.net.Models;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace Parsing_on_.net.Controllers
{
    public class HomeController : Controller
    {
        private ParsingLogic parsingLogic;
        public static List<Shop> Shops { get; set; }
        public ParsingLogic ParsingLogic { get => parsingLogic; set => parsingLogic = value; }

        public ActionResult Index(string action)
        {
            ParsingLogic = new ParsingLogic();
            Shops = ParsingLogic.GetShops();
            if (action.Equals("update"))
            {
                ParsingLogic.AddShop();
                Shops = new List<Shop>();
                Shops.AddRange(ParsingLogic.GetShops());
                return View(Shops); 
            }
            if (action.Equals("excel"))
            {
                GetExcelFile();
            }
            if (action.Equals("csv"))
            {
                GetCSVFile();
            }
            if (action.Equals("timer"))
            {
                GetTimerFile();
            }
            return View(Shops);
        }

        private void GetTimerFile()
        {
            string filePath = Server.MapPath("~/Files/timer.xlsx");
            ParsingLogic.CreateExcelFileForTimers(filePath);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "text/plain";
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }

        private void GetCSVFile()
        {
            string filePath = Server.MapPath(@"~/Files/shops.csv");
            ParsingLogic.CreateCSVFileForShops(filePath, Shops);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "text/plain";
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }

        private void GetExcelFile()
        {
            string filePath = Server.MapPath(@"~/Files/shops.xlsx");
            ParsingLogic.CreateExcelFileForShops(filePath, Shops);
            FileInfo file = new FileInfo(filePath);
            if (file.Exists)
            {
                Response.ClearContent();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
                Response.AddHeader("Content-Length", file.Length.ToString());
                Response.ContentType = "text/plain";
                Response.TransmitFile(file.FullName);
                Response.End();
            }
        }
    }
}