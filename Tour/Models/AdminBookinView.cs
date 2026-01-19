using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class AdminBookinView
    {
        public int? t_bookingid { get; set; }

        public string? t_tourname { get; set; }

        public int? c_userid { get; set; }

        public int? t_tourid { get; set; }

        public string? t_tourdate { get; set; }
    }
}