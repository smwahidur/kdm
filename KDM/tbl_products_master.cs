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
    
    public partial class tbl_products_master
    {
        public long ID { get; set; }
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public Nullable<int> Category { get; set; }
        public Nullable<int> Type { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<decimal> PP { get; set; }
        public Nullable<decimal> MRP { get; set; }
        public Nullable<decimal> DP { get; set; }
        public Nullable<decimal> WP { get; set; }
        public Nullable<decimal> SP { get; set; }
        public Nullable<decimal> Vat { get; set; }
        public Nullable<int> RB { get; set; }
        public Nullable<int> BP { get; set; }
        public string ProductDetails { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreateBy { get; set; }
        public Nullable<System.DateTime> ModificationDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
