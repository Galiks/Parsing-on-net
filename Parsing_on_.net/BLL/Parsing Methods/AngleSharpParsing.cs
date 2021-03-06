﻿using AngleSharp.Html.Parser;
using NLog;
using Parsing_on_.net.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class AngleSharpParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const int _defaultPage = 30;
        private ConcurrentBag<Shop> shops;
        private const string address = "https://letyshops.com";

        public AngleSharpParsing()
        {
            this.shops = new ConcurrentBag<Shop>();
        }

        public List<Shop> Parsing()
        {
            logger.Info("Начался парсинг " + typeof(AngleSharpParsing).Name);
            WebClient webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            var html = webClient.DownloadString("https://letyshops.com/shops?page=1");
            var maxPage = GetMaxPage(html);
            Parallel.For(1, maxPage + 1, ParseElements);
            return shops.ToList();
        }

        private void ParseElements(int i)
        {
            WebClient webClient = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            string page = "https://letyshops.com/shops?page=" + i;
            string html = webClient.DownloadString(page);
            HtmlParser parser = new HtmlParser();
            var result = parser.ParseDocument(html).QuerySelectorAll("div.b-teaser-list > div.b-teaser > a");
            foreach (var item in result)
            {
                var name = GetName(item);
                var discount = GetDiscount(item);
                string label = GetLabel(item);
                string image = GetImage(item);
                string url = GetUrl(item);
                if (!(String.IsNullOrEmpty(name) || Double.IsNaN(discount) || String.IsNullOrEmpty(label) || String.IsNullOrEmpty(image) || String.IsNullOrEmpty(url)))
                {
                    shops.Add(new Shop(name, discount, label, image, url));
                }
            }
        }

        private string GetUrl(AngleSharp.Dom.IElement item)
        {
            return address + item.GetAttribute("href");
        }

        private string GetImage(AngleSharp.Dom.IElement item)
        {
            return item.QuerySelector("div.b-teaser__top > div.b-teaser__cover > img").GetAttribute("src");
        }

        private string GetLabel(AngleSharp.Dom.IElement item)
        {
            var label = item.QuerySelectorAll("div.b-teaser__caption > div.b-teaser__cashback-rate > div > div > span.b-shop-teaser__label ").Last().TextContent;
            return label;
        }

        private double GetDiscount(AngleSharp.Dom.IElement item)
        {
            double discount;
            var discounts = item.GetElementsByClassName("b-shop-teaser__cash");
            if (discounts.Count() == 0)
            {
                string temp = item.GetElementsByClassName("b-shop-teaser__new-cash").First().TextContent;
                discount = Double.Parse(temp, CultureInfo.InvariantCulture);
            }
            else
            {
                string temp = item.GetElementsByClassName("b-shop-teaser__cash").First().TextContent;
                discount = Double.Parse(temp, CultureInfo.InvariantCulture);
            }

            return discount;
        }

        private string GetName(AngleSharp.Dom.IElement item)
        {
            var name = item.GetElementsByClassName("b-teaser__title").First().InnerHtml.ToString().Substring(17);
            return name.Substring(0, name.Length - 13);
        }

        Int32 GetMaxPage(string html)
        {
            HtmlParser parser = new HtmlParser();
            var result = parser.ParseDocument(html).GetElementsByClassName("b-pagination__link");
            try
            {
                if (Int32.TryParse(result.Last().TextContent, out int page))
                {
                    return page;
                }
            }
            catch (InvalidOperationException e)
            {
                logger.Error("Элемент не найден " + e);
            }
            return _defaultPage;
        }
    }
}