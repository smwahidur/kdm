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
    
    public partial class tbl_member_tree_history
    {
        public System.Guid ID { get; set; }
        public string MemberID { get; set; }
        public Nullable<int> BLeftPoint { get; set; }
        public Nullable<int> BRightPoint { get; set; }
        public Nullable<int> BOwnPoint { get; set; }
        public Nullable<int> ALeftPoint { get; set; }
        public Nullable<int> ARightPoint { get; set; }
        public Nullable<int> AOwnPoint { get; set; }
        public Nullable<int> BBVPoint { get; set; }
        public Nullable<int> ABVPoint { get; set; }
        public Nullable<System.DateTime> ProcessDate { get; set; }
    }
}