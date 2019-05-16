using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Parsing_on_.net.Models;

namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class WebDriverAndFizzlerMegaBonusParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //private IReadOnlyCollection<IWebElement> WebElements { get; set; }

        private const string Url = "https://megabonus.com/feed";

        public List<Shop> Parsing()
        {
            ConcurrentBag<Shop> shops = new ConcurrentBag<Shop>();
            IWebDriver driver = new ChromeDriver();
            try
            {
                driver.Navigate().GoToUrl(Url);
                var button = driver.FindElement(By.ClassName("see-more"));
                button.Click();
                while (button.Displayed)
                {
                    try
                    {
                        button.Click();
                        button = driver.FindElement(By.ClassName("see-more"));
                    }
                    catch (Exception e)
                    {
                        logger.Info("Кнопка 'Показать ещё' не была найдена. Поиск продолжится " + e);
                    }
                }
                var ul = driver.FindElement(By.ClassName("cacheback-block-list")).GetAttribute("outerHTML");
                var html = new HtmlDocument();
                html.LoadHtml(ul);
                var document = html.DocumentNode.ChildNodes;
                var webElements = document[0].SelectNodes("li");
                Parallel.ForEach(webElements, webElement =>
                {
                    var shop = ParseElements(webElement);
                    if (shop != null)
                    {
                        shops.Add(shop);
                    }
                });
                return shops.ToList();
            }
            finally
            {
                driver.Close();
            }
        }

        private Shop ParseElements(HtmlNode element)
        {
            String name = GetName(element);
            String fullDiscount = GetFullDiscount(element);
            Double discount = GetDiscount(fullDiscount);
            String label = GetLabel(fullDiscount);
            String url = GetUrl(element);
            String image = GetImage(element);
            if (name != null && !Double.IsNaN(discount) && label != null && url != null && image != null)
            {
                return new Shop(name, discount, label, image, url);
            }
            return null;
        }

        private String GetName(HtmlNode html)
        {
            try
            {
                return html.QuerySelector("div.holder-more > a").InnerText.Trim();
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return null;
        }

        private String GetFullDiscount(HtmlNode html)
        {
            try
            {
                return html.QuerySelector("div.percent_cashback").InnerText.Trim();
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return null;
        }

        private String GetUrl(HtmlNode html)
        {
            try
            {
                return Url + html.QuerySelector("div.activate-hover-block > div.holder-more > a").GetAttributeValue("href", "");
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return null;
        }

        private String GetImage(HtmlNode html)
        {
            try
            {
                return html.QuerySelector("div.holder-img > a > img").GetAttributeValue("src", "");
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return null;
        }

        private String GetLabel(String fullDiscount)
        {
            if (fullDiscount == null)
            {
                return null;
            }
            Regex regex = new Regex("[$%€]|руб|(р.)|cent|р|Р|RUB|USD|EUR|SEK|UAH|INR|BRL|GBP|CHF|PLN");
            Match matcher = regex.Match(fullDiscount);
            if (matcher.Success)
            {
                return matcher.Value;
            }
            return null;
        }

        private Double GetDiscount(String fullDiscount)
        {
            if (fullDiscount == null)
            {
                return Double.NaN;
            }
            Regex regex = new Regex("\\d+[.|,]*\\d*");
            String discount = "";
            Match matcher = regex.Match(fullDiscount);
            if (matcher.Success)
            {
                discount = matcher.Value;
            }
            if (Double.TryParse(discount.Replace('.', ','), out double result))
            {
                return result;
            }
            return Double.NaN;
        }
    }
}