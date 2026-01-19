using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tax.Models
{
    public class t_tran
    {
        public int? t_tranid { get; set; }

        public int? c_userid { get; set; }

        public string? t_tranname { get; set; }

        public string? t_trantype { get; set; }

        public string? t_taxable { get; set; }

        public int? t_taxamount { get; set; }
    }
}