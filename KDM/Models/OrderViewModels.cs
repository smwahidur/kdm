using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KDM.Models
{
    public class OrderViewModel
    {
        public Int64 OrderID { get; set; }
        [Required]
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Int64 ProductTypeID { get; set; }
        public double MRP { get; set; }
        public double PP { get; set; }
        public double DP { get; set; }
        public string MemberID { get; set; }
        public int BP { get; set; }
        public int RB { get; set; }
        public double TotalAmount { get; set; }
        public int TotalPV { get; set; }
        public int TotalBV { get; set; }
        public string DealerID { get; set; }
        public string PaymentAddress { get; set; }
        public string ShipmentAddress { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string OrderBy { get; set; }
        public string OrderApprovedBy { get; set; }
        public string OrderCanceledBy { get; set; }
        public double Quantity { get; set; }
        public decimal vat { get; set; }
        public decimal VatAmount { get; set; }
        public decimal Shiping { get; set; }
        public string MembeName { get; set; }
    }

    public class OrderInvoiceDetailsViewModel
    {
        public Int64 OrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public double TotalPrice { get; set; }
        public double TotalVat { get; set; }
        public double TotalPriceIncludingVat { get; set; }
        public double TotalShippingCharge { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string OrderStatus { get; set; }

        public List<OrderInvoicedProduct> InvoicedProducts { get; set; } = new List<OrderInvoicedProduct>();
    }

    public class OrderInvoicedProduct
    {
        public Int64 ProductID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Int16 Quantity { get; set; }
        public double MRP { get; set; }
        public double DP { get; set; }

        public Int16 PV { get; set; }
        public Int16 BV { get; set; }

        public double Vat { get; set; }
        public double TotalVat { get; set; }
        public double TotalDP { get; set; }

    }

    public class SalesHistory
    {
        public String MemberID { get; set; }
        public Int64 OrderID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public double? MRP { get; set; }
        public double? DP { get; set; }
        public int Qty { get; set; }
        public double TotalAmount { get; set; }
        public double TotalVat { get; set; }
        public int PV { get; set; }
        public int BV { get; set; }
        public string ShipmentAddress { get; set; }
        public string PaymentAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderBy { get; set; }
        public string ApprovedBy { get; set; }

        public DateTime? StatusUpdateDate { get; set; }
    }

    public class SalesHistoryByInvoice
    {
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string UserID { get; set; }
        public string MemberID { get; set; }
        public string InvoicedBillAmount { get; set; }
        public string InvoicedPV { get; set; }
        public string InvoicedBV { get; set; }
        public string NotProcessedPV { get; set; }
        public string NotProcessedBV { get; set; }
    }
}