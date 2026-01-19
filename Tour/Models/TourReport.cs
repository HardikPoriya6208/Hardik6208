using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class TourReport
    {
        public int t_tourid { get; set; }
        public string t_expensename { get; set; } = "";
        public int t_tourprice { get; set; }
        public int totalExpense { get; set; }
        public int totalBooking { get; set; }
        public int totalIncome { get; set; }
        public int profit { get; set; }
    }
}