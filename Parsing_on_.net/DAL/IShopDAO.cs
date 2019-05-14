using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing_on_.net.DAL
{
    public interface IShopDAO
    {
        void AddShops(List<Shop> shops);
        List<Shop> GetShops();
        void AddTimers(Timer timer);
        List<Timer> GetTimers();
        void DeleteAndCreateDatabase();
    }
}
