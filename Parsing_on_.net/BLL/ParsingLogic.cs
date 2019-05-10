using System.Collections.Generic;
using NLog;
using Parsing_on_.net.BLL.Parsing_Methods;
using Parsing_on_.net.DAL;
using Parsing_on_.net.Models;

namespace Parsing_on_.net.BLL
{
    public class ParsingLogic : IParsingLogic
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly ShopDAO _shopDAO = new ShopDAO();

        public static ShopDAO ShopDAO => _shopDAO;

        public bool AddShop()
        {
            var shops = Parsing();
            if (shops.Count > 0)
            {
                ShopDAO.AddShops(shops);
                return true;
            }
            return false;
        }

        public List<Shop> GetShops()
        {
            return ShopDAO.GetShops();
        }

        public List<Shop> Parsing()
        {
            List<IParser> parsers = new List<IParser>()
            {
                //new AngleSharpParsing(),
                //new CsQueryParsing(),
                //new FizzlerParsing(),
                //new HtmlAgilityPackParsing(),
                new RestSharpParsing(),
                //new WebDriverParsing(),
            };
            List<Shop> shops = new List<Shop>();
            foreach (var item in parsers)
            {
                var watch = System.Diagnostics.Stopwatch.StartNew();
                shops.AddRange(item.Parsing());
                watch.Stop();
                logger.Info($"Парсинг {item.GetType().Name} завершился за {watch.ElapsedMilliseconds}");
            }
            return shops;
        }
    }
}