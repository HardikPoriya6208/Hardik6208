using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flight.Models
{
    public class t_flight
    {
        public int? t_flightId { get; set; }

        public string? t_flightno { get; set; }

        public string? t_departure { get; set; }

        public string? t_destination { get; set; }

        public DateOnly? t_date { get; set; }

        public int? t_totalSeats { get; set; }

        public int? t_price { get; set; }

    }
}