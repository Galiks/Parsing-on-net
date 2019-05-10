using Parsing_on_.net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parsing_on_.net.BLL
{
    public interface IParsingLogic
    {
        List<Shop> Parsing();
        bool AddShop();
        List<Shop> GetShops();
        List<Timer> GetTimers();
        bool CreateExcelFileForShops(string filePath, List<Shop> shops);
        bool CreateCSVFileForShops(string filePath, List<Shop> shops);
        bool CreateExcelFileForTimers(string filePath);
    }
}
