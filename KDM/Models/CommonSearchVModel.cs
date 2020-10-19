using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class CommonSearchVModel
    {
        [Required]
        public DateTime FDate { get; set; }
        [Required]
        public DateTime TDate { get; set; }

    }
}