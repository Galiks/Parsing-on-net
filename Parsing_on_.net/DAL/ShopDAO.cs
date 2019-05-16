using Microsoft.EntityFrameworkCore;
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

        private ShopContext ShopContext { get; set; }

        public ShopDAO()
        {
            CreateShopContext();
        }

        public void AddShops(List<Shop> shops)
        {
            foreach (var item in shops)
            {
                try
                {
                    ShopContext.Shops.Add(item);
                    ShopContext.SaveChanges();
                }
                catch (Exception e)
                {
                    logger.Error("Произошла ошибка : " + e);
                }
            }
        }

        public void AddTimers(Timer timer)
        {
            try
            {
                ShopContext.Timers.Add(timer);
                ShopContext.SaveChanges();
            }
            catch (Exception e )
            {
                logger.Error("Произошла ошибка : " + e);
            }
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

        public void CreateShopContext()
        {
            ShopContext = new ShopContext();
        }
    }
}