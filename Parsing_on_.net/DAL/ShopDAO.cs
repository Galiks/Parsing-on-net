using NLog;
using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Parsing_on_.net.DAL
{
    public class ShopDAO : IShopDAO
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly ShopContext shopContext = new ShopContext();

        private static ShopContext ShopContext => shopContext;

        static ShopDAO()
        {
            
        }

        public void AddShops(List<Shop> shops)
        {
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

        public void AddTimers(Timer timer)
        {
            ShopContext.Timers.Add(timer);
            ShopContext.SaveChanges();
        }

        public List<Shop> GetShops()
        {
            return ShopContext.Shops.ToList();
        }

        public List<Timer> GetTimers()
        {
            return ShopContext.Timers.ToList();
        }

        public void DeleteAndCreateDatabase()
        {
            ShopContext.Database.EnsureDeleted();
            ShopContext.Database.EnsureCreated();
        }
    }
}