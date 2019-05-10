using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parsing_on_.net.Models
{
    public class Timer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Time { get; set; }
        public string Date { get; set; }

        public Timer(string name, double time, string date)
        {
            Name = name;
            Time = time;
            Date = date;
        }
    }
}