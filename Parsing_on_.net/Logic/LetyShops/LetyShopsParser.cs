using AngleSharp.Html.Parser;
using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Parsing_on_.net.Logic.LetyShops
{
    public class LetyShopsParser
    {
        private const int _defaultPage = 30;
        private static List<Shop> shops = new List<Shop>();

        public List<Shop> Parsing()
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            var html = webClient.DownloadString("https://letyshops.com/shops?page=1");
            Parallel.For(1, MaxPage(html) + 1, ParseElements);
            return shops;
        }

        private void ParseElements(int i)
        {
            WebClient webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
            string html = webClient.DownloadString($"https://letyshops.com/shops?page={i}");
            HtmlParser parser = new HtmlParser();
            var result = parser.ParseDocument(html).GetElementsByClassName("b-teaser");

            foreach (var item in result)
            {
                var name = item.GetElementsByClassName("b-teaser__title").First().InnerHtml.ToString().Substring(17);
                name = name.Substring(0, name.Length - 13);
                decimal discount = 0;
                var discounts = item.GetElementsByClassName("b-shop-teaser__cash");
                if (discounts.Count() == 0)
                {
                    string temp = item.GetElementsByClassName("b-shop-teaser__new-cash").First().InnerHtml;
                    discount = Decimal.Parse(temp, CultureInfo.InvariantCulture);
                }
                else
                {
                    string temp = item.GetElementsByClassName("b-shop-teaser__cash").First().InnerHtml;
                    discount = Decimal.Parse(temp, CultureInfo.InvariantCulture);
                }
                var label = item.GetElementsByClassName("b-shop-teaser__label ").Last().InnerHtml;
                var image = item.GetElementsByClassName("b-teaser__cover").First().GetElementsByTagName("img").First().GetAttribute("src");
                var url = item.GetElementsByClassName("b-teaser__inner").First().GetAttribute("href");
                shops.Add(new Shop(name, discount, label, image, url));
            }
        }

        private Int32 MaxPage(string html)
        {
            HtmlParser parser = new HtmlParser();
            var result = parser.ParseDocument(html).GetElementsByClassName("b-pagination__link");
            try
            {
                if (Int32.TryParse(result.Last().InnerHtml, out int page))
                {
                    return page;
                }
            }
            catch (InvalidOperationException)
            {
                if (result.Length == 0)
                {
                    Console.WriteLine("Elements not found");
                }
            }
            return _defaultPage;
        }
    }
}