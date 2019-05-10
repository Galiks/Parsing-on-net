using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parsing_on_.net.Models
{
    public class Shop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Discount { get; set; }
        public string Label { get; set; }
        public string Image { get; set; }
        public string URL { get; set; }

        public Shop()
        {
        }

        public Shop(string name, double discount, string label, string image, string uRL)
        {
            Name = name;
            Discount = discount;
            Label = label;
            Image = image;
            URL = uRL;
        }

        public override string ToString()
        {
            return $"Name: {Name}\nDiscount: {Discount} {Label}\nImage: {Image}\nURL: {URL}\n";
        }
    }
}