//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace KDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_dealers
    {
        public long DealerID { get; set; }
        public string MemberID { get; set; }
        public string Name { get; set; }
        public Nullable<int> District { get; set; }
        public Nullable<int> Thana { get; set; }
        public Nullable<int> DealerUnion { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public Nullable<System.DateTime> DateOfOperation { get; set; }
        public Nullable<decimal> PrimaryDeposit { get; set; }
        public Nullable<long> PhotoID { get; set; }
        public string UserID { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public Nullable<int> DealerType { get; set; }
    }
}
