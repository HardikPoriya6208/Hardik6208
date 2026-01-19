using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models
{
    public class t_book
    {
        public int? c_bookingid { get; set; }

        public string? c_bookname { get; set; }

        public string? c_author { get; set; }

        public string? c_category { get; set; }

        public int? c_totalqty { get; set; }
        public string? c_image { get; set; }

        public int? c_availableqty { get; set; }
    }
}