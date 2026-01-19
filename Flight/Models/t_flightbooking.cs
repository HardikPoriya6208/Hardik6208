using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Models
{
    public class t_flightbooking
    {
        public int? t_bookingId { get; set; }

        public int? c_userid { get; set; }

        public int? t_flightId { get; set; }
        public string? t_departure { get; set; }

        public DateOnly? t_date { get; set; }

        public int? t_totalSeats { get; set; }

    }
}