using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KDM.Models
{
    public class UserMemberLinkVModel
    {
        public Int64 ID { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string MemberId { get; set; }
        public DateTime ModificationDate { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime TransferDate { get; set; }
        public string TransferBy { get; set; }  
    }
}