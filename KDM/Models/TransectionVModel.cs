using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class TransectionVModel
    {
        [Required]
        public string MemberID { get; set; }
        public string MemberName { get; set; }
        [Required]
        public string DebitAccount { get; set; }
        [Required]
        public string CreditACcount { get; set; }
        public decimal Amount { get; set; }
        [Required]
        public string Particulars { get; set; }
        public DateTime PostingDates { get; set; }
        public string TFNumber { get; set; }

    }
}