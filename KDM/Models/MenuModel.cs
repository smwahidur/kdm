using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class MenuCategory
    {
        public Int64 Id { get; set; }

        [Required]
        [Display(Name="Category Name")]
        public string CategoryName { get; set; }

        public List<MenuItem> ChildItems { get; set; } = new List<MenuItem>();
    }
    public class MenuCreate
    {
        public int MenuOrder { get; set; }
        public string CategoryName { get; set; }
    }

    
}