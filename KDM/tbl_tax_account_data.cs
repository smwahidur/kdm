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
    
    public partial class tbl_tax_account_data
    {
        public int ID { get; set; }
        public long trSerialNo { get; set; }
        public string ForAccount { get; set; }
        public int PurposeCode { get; set; }
        public Nullable<decimal> DebitAmount { get; set; }
        public Nullable<decimal> CreditAmount { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public Nullable<System.DateTime> PostingDate { get; set; }
        public Nullable<System.TimeSpan> PostingTime { get; set; }
        public string PostedBy { get; set; }
        public string ApprovedBy { get; set; }
    }
}
