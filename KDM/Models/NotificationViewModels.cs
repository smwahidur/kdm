using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class NotificationViewModel
    {
        public Int64 Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string ToEmail { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? SendTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ReadTime { get; set; }
    }
}