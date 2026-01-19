using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Event.Models
{
    public class AdminBookingView
    {
        public int? c_bookingid { get; set; }
        public int? c_userid { get; set; }
        public int? c_eventid { get; set; }
        public int? c_eventprice { get; set; }

        public int? c_eventseats { get; set; }
        public string? c_status { get; set; }

        public string? c_eventdate { get; set; }
    }
}