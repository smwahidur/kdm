using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class ProductVModel
    {
        public Int64 ProductID { get; set; }
        [Required]
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int Category { get; set; }
        public string CategoryName { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        [Required]
        public string Code { get; set; }
        public decimal MRP { get; set; }
        public decimal WalletPrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public int RB { get; set; }
        public int BP { get; set; }
        public int Quantity { get; set; }
        public string ProductDetails { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime ModificationDate { get; set; }
        public string ModifiedBy { get; set; }
        public int InStock { get; set; }

        public string Unit { get; set; }
        public decimal DistributorPrice { get; set; }
        public decimal StokishPrice { get; set; }
        public decimal Vat { get; set; }

    }
}