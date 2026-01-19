using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class t_tourexpense
    {
        public int? t_expenseid { get; set; }

        public int? t_tourid { get; set; }

        public string? t_expensename { get; set; }

        public string? t_tourdate { get; set; }

        public int? t_tourprice { get; set; }
    }
}