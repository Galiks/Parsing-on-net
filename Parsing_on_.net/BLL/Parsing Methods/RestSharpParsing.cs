using Newtonsoft.Json.Linq;
using NLog;
using Parsing_on_.net.Models;
using RestSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class RestSharpParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public ConcurrentBag<Shop> Shops { get; set; }

        public RestSharpParsing()
        {
            Shops = new ConcurrentBag<Shop>();
        }

        public List<Shop> Parsing()
        {
            logger.Info("Начался парсинг " + typeof(RestSharpParsing).Name);
            int[] pages = new int[]
            {
                0,100,200,300,400,500,600,700,800,900,1000,1100,1200,1300
            };
            List<JToken> listOfItems = new List<JToken>();
            Parallel.ForEach(pages, page =>
            {
                try
                {
                    listOfItems.Add(GetJSon(page));
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            });
            Parallel.ForEach(listOfItems, item =>
            {
                try
                {
                    Shops.Add(ParseElements(item));
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            });

            return Shops.ToList();
        }

        private JToken GetJSon(int i)
        {
            string url = $"https://d289b99uqa0t82.cloudfront.net/sites/5/campaigns_limit_100_offset_{i}_order_popularity.json";
            var client = new RestClient(url);
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", "d289b99uqa0t82.cloudfront.net");
            request.AddHeader("Postman-Token", "048aef15-143b-4f61-8c44-60467f64a33d,e85413f5-28a6-4878-b792-942c640071cc");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("User-Agent", "PostmanRuntime/7.11.0");
            IRestResponse response = client.Execute(request);
            JObject jsonParse = JObject.Parse(response.Content);
            var listOfItems = jsonParse["items"];
            return listOfItems;
        }

        private Shop ParseElements(JToken jToken)
        {
            var name = GetName(jToken);
            var image = GetImage(jToken);
            var shopUrl = GetUrl(jToken);
            var discount = GetDiscount(jToken);
            var label = GetLabel(jToken);
            if (!(String.IsNullOrEmpty(name) || Double.IsNaN(discount) || String.IsNullOrEmpty(label) || String.IsNullOrEmpty(image) || String.IsNullOrEmpty(shopUrl)))
            {
                return new Shop(name, discount, label, image, shopUrl);
            }
            else
            {
                return null;
            }
        }

        private String GetName(JToken token)
        {
            return token["title"].ToString();
        }

        private String GetImage(JToken token)
        {
            return token["image"]["url"].ToString();
        }

        private String GetUrl(JToken token)
        {
            return "https://www.kopikot.ru/stores/" + token["url"].ToString() + "/" + token["id"].ToString();
        }

        private Double GetDiscount(JToken token)
        {
            string discount = token["commission"]["max"]["original_amount"].ToString();
            if (double.TryParse(discount.Replace('.', ','), out double result))
            {
                return result;
            }
            return Double.NaN;
        }

        private String GetLabel(JToken token)
        {
            return token["commission"]["max"]["unit"].ToString();
        }
    }
}