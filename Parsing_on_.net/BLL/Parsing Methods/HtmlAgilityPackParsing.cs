using HtmlAgilityPack;
using NLog;
using Parsing_on_.net.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class HtmlAgilityPackParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private const int defaultPageCount = 30;
        private const string addressOfSiteForMaxPage = "https://letyshops.com/shops?page=1";
        private const string addressOfSiteForParsing = "https://letyshops.com/shops?page=";
        private const string addressOfSite = "https://letyshops.com";
        private HtmlWeb htmlWeb;

        public HtmlAgilityPackParsing()
        {
            htmlWeb = new HtmlWeb
            {
                OverrideEncoding = Encoding.UTF8
            };
        }

        public List<Shop> Parsing()
        {
            ConcurrentBag<Shop> shops = new ConcurrentBag<Shop>();
            logger.Info("Начался парсинг " + typeof(HtmlAgilityPackParsing).Name);
            for (int i = 1; i <= GetMaxPage(); i++)
            {
                var document = htmlWeb.Load(addressOfSiteForParsing + i);
                var nodes = document.DocumentNode.SelectNodes("*//a[@class='b-teaser__inner']");
                Parallel.ForEach(nodes, node =>
                {
                    var shop = ParseElements(node);
                    if (shop != null)
                    {
                        shops.Add(shop);
                    }
                });
            }

            return shops.ToList();
        }

        private Shop ParseElements(HtmlNode item)
        {
            String name = GetName(item);
            double discount = GetDiscount(item);
            String label = GetLabel(item);
            String url = GetURL(item);
            String image = GetImage(item);
            if (!(String.IsNullOrEmpty(name) || Double.IsNaN(discount) || String.IsNullOrEmpty(label) || String.IsNullOrEmpty(image) || String.IsNullOrEmpty(url)))
            {
                return new Shop(ConvertString(name), discount, label, image, url);
            }
            return null;
        }

        private string GetName(HtmlNode node)
        {
            var name = node.SelectSingleNode("*//div[@class='b-teaser__title']").InnerText;
            if (!String.IsNullOrWhiteSpace(name))
            {
                return name.Trim();
            }
            return null;
        }

        private double GetDiscount(HtmlNode node)
        {
            string discount = "";
            try
            {
                discount = node.SelectSingleNode("*//span[@class='b-shop-teaser__cash']").InnerText;
                if (!String.IsNullOrWhiteSpace(discount))
                {
                    return Double.Parse(discount, CultureInfo.InvariantCulture);
                }
            }
            catch (NullReferenceException e)
            {
                logger.Info("Вторая попытка взять discount " + e);
                discount = node.SelectSingleNode("*//span[@class='b-shop-teaser__new-cash']").InnerText;
                if (!String.IsNullOrWhiteSpace(discount))
                {
                    return Double.Parse(discount, CultureInfo.InvariantCulture);
                }
            }
            return Double.NaN;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetLabel(HtmlNode node)
        {
            var label = "";

            //Почему сначала 4, а потом 3? Потому что если наоборот, то можно вытащить размер кэш-бэка, вместо лэйбла
            try
            {
                label = node.SelectSingleNode("//span[4]").InnerText;
                if (!String.IsNullOrWhiteSpace(label))
                {
                    return label;
                }
            }
            catch (NullReferenceException e)
            {
                logger.Info("Вторая попытка взять label " + e);
                label = node.SelectSingleNode("//span[3]").InnerText;
                if (!String.IsNullOrWhiteSpace(label))
                {
                    return label;
                }
            }
            return null;
        }

        /// <summary>
        /// Возвращает url магазина на сайте
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private string GetURL(HtmlNode node)
        {
            var url = node.GetAttributeValue("href", "");
            if (!String.IsNullOrWhiteSpace(url))
            {
                return addressOfSite + url;
            }
            return null;
        }

        private string GetImage(HtmlNode node)
        {
            var image = node.SelectSingleNode("*//div[@class='b-teaser__cover']/img").GetAttributeValue("src", "");
            if (!String.IsNullOrWhiteSpace(image))
            {
                return image;
            }
            return null;
        }

        private int GetMaxPage()
        {
            var document = htmlWeb.Load(addressOfSiteForMaxPage);
            var maxPageString = document.DocumentNode.SelectSingleNode("//ul[@class='b-pagination js-pagination']/li[5]");
            if (maxPageString == null)
            {
                return defaultPageCount;
            }
            if (int.TryParse(maxPageString.InnerText, out int maxPageInt))
            {
                return maxPageInt;
            }
            else
            {
                return defaultPageCount;
            }
        }

        /// <summary>
        /// Возвращает html сайта
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetHTML(string url)
        {
            string pageUrl = "";
            var request = (HttpWebRequest)WebRequest.Create(url);

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var receiveStream = response.GetResponseStream();
                    if (receiveStream != null)
                    {
                        StreamReader streamReader;
                        if (response.CharacterSet == null)
                        {
                            streamReader = new StreamReader(receiveStream);
                        }
                        else
                        {
                            streamReader = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                        }
                        pageUrl = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
            }

            return pageUrl;
        }

        private string ConvertString(string text)
        {
            return WebUtility.HtmlDecode(text);
        }
    }
}