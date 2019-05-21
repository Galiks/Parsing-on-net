using HtmlAgilityPack;
using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler.Systems.HtmlAgilityPack;
using NLog;
using System.Collections.Concurrent;

namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class FizzlerParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const int defaultPageCount = 30;
        private const string addressOfSiteForMaxPage = "https://letyshops.com/shops?page=1";
        private const string addressOfSiteForParsing = "https://letyshops.com/shops?page=";
        private const string addressOfSite = "https://letyshops.com";

        private ConcurrentBag<Shop> Shops { get; set; }

        public FizzlerParsing()
        {
            Shops = new ConcurrentBag<Shop>();
        }

        public List<Shop> Parsing()
        {
            logger.Info("Начался парсинг " + typeof(FizzlerParsing).Name);
            var maxPage = GetMaxPage();
            Parallel.For(1, maxPage + 1, ParseElements);
            return Shops.ToList();
        }

        private void ParseElements(int i)
        {
            var htmlWeb = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8
            };
            var document = htmlWeb.Load(addressOfSiteForParsing + i);
            var html = document.DocumentNode;
            var listOfShops = html.QuerySelectorAll("div.b-teaser > a.b-teaser__inner");
            foreach (var item in listOfShops)
            {
                String name = GetName(item);
                Double discount = GetDiscount(item);
                String label = GetLabel(item);
                String url = GetUrl(item);
                String image = GetImage(item);
                if (!(String.IsNullOrEmpty(name) || Double.IsNaN(discount) || String.IsNullOrEmpty(label) || String.IsNullOrEmpty(image) || String.IsNullOrEmpty(url)))
                {
                    Shops.Add(new Shop(name, discount, label, image, url));
                }
            }
        }

        private String GetName(HtmlNode html)
        {
            return html.QuerySelector("div.b-teaser__title").InnerText.Trim();
        }

        private String GetLabel(HtmlNode html)
        {
            var labelList = html.QuerySelectorAll("div.b-teaser__caption > div.b-teaser__cashback-rate > div > div > span.b-shop-teaser__label");
            return labelList.Last().InnerText.Trim();
        }

        private String GetUrl(HtmlNode html)
        {
            return addressOfSite + html.GetAttributeValue("href", "");
        }

        private String GetImage(HtmlNode html)
        {
            return html.QuerySelector("div.b-teaser__top > div.b-teaser__cover > img").GetAttributeValue("src", "");
        }

        private Double GetDiscount(HtmlNode html)
        {
            string discount = "";
            try
            {
                discount = html.QuerySelector("div.b-teaser__caption > div.b-teaser__cashback-rate > div > div > span.b-shop-teaser__cash").InnerText.Trim();
            }
            catch (NullReferenceException e)
            {
                logger.Info("Вторая попытка взять discount " + e);
                discount = html.QuerySelector("div.b-teaser__caption > div.b-teaser__cashback-rate > div > div > span.b-shop-teaser__new-cash").InnerText.Trim(); ;
            }
            if (Double.TryParse(discount.Replace('.', ','), out double result))
            {
                return result;
            }
            return Double.NaN;
        }

        private Int32 GetMaxPage()
        {
            var htmlWeb = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8
            };
            var document = htmlWeb.Load("https://letyshops.com/shops?page=1");
            var html = document.DocumentNode;
            string maxPage = html.QuerySelector("div.b-content.b-content--shops > ul > li:nth-child(5) > a").InnerText.Trim();
            if (Int32.TryParse(maxPage, out int result))
            {
                return result;
            }
            return 30;
        }
    }
}