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
    
    public partial class tbl_training
    {
        public int ID { get; set; }
        public string EmployeeId { get; set; }
        public string TrainingTitle { get; set; }
        public string InstituteName { get; set; }
        public string TrainingCountry { get; set; }
        public string TrainingDuration { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
}