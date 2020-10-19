using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KDM.Models
{
    public class DealerVModel
    {
        public Int64 DealerID { get; set; }
        public string Name { get; set; }
        public int District { get; set; }
        public int Thana { get; set; }
        public int DealerUnion { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public DateTime DateOfOperation { get; set; }
        public decimal PrimaryDeposit { get; set; }
        public Int64 PhotoID { get; set; }
        public string UserID { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public int DealerType { get; set; }
    }
}