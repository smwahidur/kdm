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
    
    public partial class tbl_binary_matching_data
    {
        public int ID { get; set; }
        public string PlacementID { get; set; }
        public string BPCode { get; set; }
        public Nullable<System.DateTime> ProcessDate { get; set; }
        public string InputerID { get; set; }
        public string AuthorizerID { get; set; }
        public Nullable<double> BLeftPoint { get; set; }
        public Nullable<double> BRightPoint { get; set; }
        public Nullable<int> MatchingCount { get; set; }
        public Nullable<double> ALeftPoint { get; set; }
        public Nullable<double> ARightPoint { get; set; }
        public Nullable<System.DateTime> PostingDate { get; set; }
        public Nullable<int> SentToAccount { get; set; }
        public Nullable<System.DateTime> SentDate { get; set; }
        public string SentByID { get; set; }
        public string AuthorizeSentID { get; set; }
    }
}