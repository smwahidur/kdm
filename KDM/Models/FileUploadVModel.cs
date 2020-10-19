using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KDM.Models
{
    public class FileUploadVModel
    {
        public string FileNote { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}