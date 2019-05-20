using CsQuery;
using NLog;
using Parsing_on_.net.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class CsQueryParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const string address = "https://letyshops.com/shops?page=";
        private readonly int defaultMaxpage = 20;
        private ConcurrentBag<Shop> Shops { get; set; }
        public CsQueryParsing()
        {
            Shops = new ConcurrentBag<Shop>();
        }

        public List<Shop> Parsing()
        {
            logger.Info("Начался парсинг " + typeof(CsQueryParsing).Name);
            var maxPage = GetMaxPage();
            for (int i = 1; i <= GetMaxPage(); i++)
            {
                var document = CQ.CreateFromUrl(address + i);
                var domObjects = document["a.b-teaser__inner"];
                Parallel.ForEach(domObjects, domObject =>
                {
                    Shop shop;
                    if ((shop = ParseElements(domObject)) != null)
                    {
                        Shops.Add(shop);
                    }
                });
            }

            return Shops.ToList();
        }

        private Shop ParseElements(IDomObject domObject)
        {
            var document = CQ.CreateDocument(domObject.InnerHTML);
            string name = GetName(document);
            double discount = GetDiscount(document);
            string label = GetLabel(document);
            string image = GetImage(document);
            string url = GetUrl(domObject);
            if (!(String.IsNullOrEmpty(name) || Double.IsNaN(discount) || String.IsNullOrEmpty(label) || String.IsNullOrEmpty(image) || String.IsNullOrEmpty(url)))
            {
                return new Shop(name, discount, label, image, url);
            }
            else
            {
                return null;
            }
        }

        private string GetUrl(IDomObject domObject)
        {
            return "https://letyshops.com" + domObject.Attributes.GetAttribute("href");
        }

        private string GetImage(CQ document)
        {
            return document["img"].First().Attr<string>("src");
        }

        private string GetLabel(CQ document)
        {
            string label = document["span.b-shop-teaser__label "].Last().Text().Trim();
            return label;
        }

        private double GetDiscount(CQ document)
        {
            string discount = document["span.b-shop-teaser__cash"].First().Text().Trim();
            if (String.IsNullOrWhiteSpace(discount))
            {
                discount = document["span.b-shop-teaser__new-cash"].First().Text().Trim();
            }
            if (double.TryParse(discount, NumberStyles.Number, CultureInfo.InvariantCulture, out double trueDiscount))
            {
                return trueDiscount;
            }
            return double.NaN;
        }

        private string GetName(CQ document)
        {
            return document["div.b-teaser__title"].First().Text().Trim();
        }

        private Int32 GetMaxPage()
        {
            var document = CQ.CreateFromUrl("https://letyshops.com/shops?page=1");
            var numbers = document["a.b-pagination__link"];
            if (Int32.TryParse(numbers[numbers.Length - 2].InnerText.Trim(), out Int32 maxPage))
            {
                return maxPage;
            }
            return defaultMaxpage;
        }
    }
}