using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Event.Models
{
    public class t_event
    {
        public int? c_eventId { get; set; }

        public string? c_eventname { get; set; }

        public string? c_eventdate { get; set; }

        public int? c_eventprice { get; set; }

        public int? c_eventseats { get; set; }

        public string? c_address { get; set; }
    }
}