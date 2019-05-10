using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;
using Parsing_on_.net.Models;

namespace Parsing_on_.net.DAL
{
    public class ShopDAO : IShopDAO
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public void AddShops(List<Shop> shops)
        {
            using (ShopContext shopContext = new ShopContext())
            {
                shopContext.Database.Delete();
                shopContext.Database.CreateIfNotExists();
                foreach (var item in shops)
                {
                    try
                    {
                        shopContext.Shops.Add(item);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Произошёл троллинг: " + e);
                    }
                }
                shopContext.SaveChanges();
            }
        }

        public List<Shop> GetShops()
        {
            using (ShopContext shopContext = new ShopContext())
            {
                return shopContext.Shops.ToList();
            }
        }
    }
}