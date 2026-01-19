using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace library.Models
{
    public class t_BookIssue
    {
        public int? c_issueid { get; set; }

        public string? c_bookname { get; set; }
        public int c_userid { get; set; }

        public int c_bookingid { get; set; }

        public string? c_issueDate { get; set; }

        public string? c_dueDate { get; set; }

        public string? c_status { get; set; }
    }
}