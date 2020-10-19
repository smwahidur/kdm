using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class MemberRegistrationViewModel
    {
        public bool WithPassword { get; set; }
        public Int64 ID { get; set; }
        public string MemberID { get; set; }
        public string SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int Position { get; set; }
        [Required]
        public string DistributorName { get; set; }

        public string FathersName { get; set; }
      
        public string MothersName { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
       
        public string NID { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone1 { get; set; }

        [Required]
        public string UserPassword { get; set; } = "User@123456";
        public string Phone2 { get; set; }
        public string NomineeName { get; set; }
        public string RelationWithNominee { get; set; }
        public string NomineeNIDOrBirthCertificate { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Int64 PhotoID { get; set; }
        public HttpPostedFileBase Photo { get; set; }

        [Required]
        public string PlacementID { get; set; }
        
        public string LeftID { get; set; }
        public string RightID { get; set; }

        public string PlacementPosition { get; set; } = "Left";
        
    }
    public class MemberToUserViewModel
    {
        [Required]
        public string MemberID { get; set; }
        
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string Phone1 { get; set; }

        public string UserPassword { get; set; }
    }



}