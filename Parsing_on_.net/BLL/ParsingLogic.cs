using System;
using System.Collections.Generic;
using NLog;
using Parsing_on_.net.BLL.File_Logic;
using Parsing_on_.net.BLL.Parsing_Methods;
using Parsing_on_.net.DAL;
using Parsing_on_.net.Models;

namespace Parsing_on_.net.BLL
{
    public class ParsingLogic : IParsingLogic
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private ShopDAO shopDAO;

        public ShopDAO ShopDAO { get => shopDAO; set => shopDAO = value; }

        public ParsingLogic()
        {
            ShopDAO = new ShopDAO();
        }

        public void AddShop()
        {
            ShopDAO.DeleteAndCreateDatabase();
            Parsing();
        }

        public bool CreateCSVFileForShops(string filePath, List<Shop> shops)
        {
            CSVFile csvFile = new CSVFile();
            if (filePath != null && shops.Count > 0)
            {
                csvFile.WriteFile(filePath, shops);
                return true;
            }
            return false;
        }

        public bool CreateExcelFileForShops(string filePath, List<Shop> shops)
        {
            ExcelFile excelFile = new ExcelFile();
            if (filePath != null && shops.Count > 0)
            {
                excelFile.WriteFile(filePath, shops);
                return true;
            }
            return false;
        }

        public bool CreateExcelFileForTimers(string filePath)
        {
            ExcelFile excelFile = new ExcelFile();
            var timers = GetTimers();
            if (filePath != null && timers.Count > 0)
            {
                excelFile.WriteFile(filePath, timers);
                return true;
            }
            return false;
        }

        public List<Shop> GetShops()
        {
            return ShopDAO.GetShops();
        }

        public List<Timer> GetTimers()
        {
            return ShopDAO.GetTimers();
        }

        public void Parsing()
        {
            List<IParser> parsers = new List<IParser>()
            {
                //new AngleSharpParsing(),
                new CsQueryParsing(),
                //new FizzlerParsing(),
                //new HtmlAgilityPackParsing(),
                //new RestSharpForLetyShopsParsing(),
                //new WebDriverAndFizzlerMegaBonusParsing(),
                //new WebDriverParsing(),
            };
            List<Shop> shops = new List<Shop>();
            foreach (var item in parsers)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                shops = item.Parsing();
                watch.Stop();
                ShopDAO.AddTimers(new Timer(item.GetType().Name, watch.ElapsedMilliseconds, DateTime.Now.ToString()));
                if (shops.Count > 0)
                {
                    ShopDAO.AddShops(shops); 
                }
            }
        }
    }
}