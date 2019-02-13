using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parsing_on_.net.Models
{
    public class Shop
    {

        public Shop(string name, decimal discount, string label, string image, string url)
        {
            Name = name;
            Discount = discount;
            Label = label;
            Image = image;
            URL = url;
        }

        public string Name { get; set; }
        public decimal Discount { get; set; }
        public string Label { get; set; }
        public string Image { get; set; }
        public string URL { get; set; }
    }
}