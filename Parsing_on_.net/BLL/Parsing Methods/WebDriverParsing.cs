using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Parsing_on_.net.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;


namespace Parsing_on_.net.BLL.Parsing_Methods
{
    public class WebDriverParsing : IParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger(); 

        public List<Shop> Parsing()
        {
            logger.Info("Начался парсинг " + typeof(WebDriverParsing).Name);
            ConcurrentBag<Shop> shops = new ConcurrentBag<Shop>();
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://megabonus.com/feed");
            var button = driver.FindElement(By.ClassName("see-more"));
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
            var ul = driver.FindElement(By.ClassName("cacheback-block-list"));
            var webElements = ul.FindElements(By.TagName("li"));
            Parallel.ForEach(webElements, WebElement =>
            {
                var shop = ParseElements(WebElement);
                if (shop != null)
                {
                    shops.Add(shop);
                }
            });
            driver.Close();
            return shops.ToList();
        }

        private Shop ParseElements(IWebElement item)
        {
            var name = GetName(item);
            var fullDiscount = GetFullDiscount(item);
            var discount = GetDiscount(fullDiscount);
            var label = GetLabel(fullDiscount);
            var image = GetImage(item);
            var url = GetPage(item);
            if (!(String.IsNullOrEmpty(name) || Double.IsNaN(discount) || String.IsNullOrEmpty(label) || String.IsNullOrEmpty(image) || String.IsNullOrEmpty(url)))
            {
                return new Shop(name, discount, label, image, url);
            }
            return null;
        }

        private String GetPage(IWebElement element)
        {
            String page = "";
            try
            {
                page = element.FindElement(By.CssSelector("div.holder-img > a")).GetAttribute("href");
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return page;
        }

        private String GetLabel(String fullDiscount)
        {
            if (fullDiscount == null)
            {
                logger.Error("Пустой fullDiscount");
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
                logger.Error("Пустой fullDiscount");
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

        private String GetFullDiscount(IWebElement element)
        {
            String fullDiscount = "";
            try
            {
                fullDiscount = element.FindElement(By.CssSelector("div.your-percentage > strong")).Text;
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return fullDiscount;
        }

        private String GetImage(IWebElement element)
        {
            String image = "";
            try
            {
                image = element.FindElement(By.TagName("img")).GetAttribute("src");
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            return image;
        }

        private String GetName(IWebElement element)
        {
            Regex regex = new Regex("Подробнее про кэшбэк в ([\\w\\s\\d\\W]+)");
            String name = "";
            try
            {
                name = element.FindElement(By.ClassName("holder-more")).FindElement(By.TagName("a")).GetAttribute("innerHTML");
            }
            catch (Exception e)
            {
                if (e is NullReferenceException || e is NoSuchElementException)
                {
                    logger.Error("Произошла ошибка: " + e);
                    return null;
                }
            }
            Match matcher = regex.Match(name);
            if (matcher.Success)
            {
                return matcher.Groups[1].Value;
            }
            return null;
        }
    }
}