using KDM.Helpers;
using KDM.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;
using System.Drawing.Printing;

namespace KDM.Controllers
{
   // [KDMActionFilter]

    public class KDMMemberController : Controller
    {
        KDMDB db = new KDMDB();
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult Profile()
        {
            string userName = User.Identity.Name;
            var member = db.tbl_members.FirstOrDefault(x => x.Phone1 == userName);

            if (member != null)
            {
                MemberRegistrationViewModel model = new MemberRegistrationViewModel();
                model.ID = member.ID;
                model.MemberID = member.MemberID;
                model.PlacementID = member.PlacementID;
                model.SponsorID = member.SponsorID;
                model.SponsorName = member.SponsorName;
                //model.Position = (int)member.Position;
                model.DistributorName = member.DistributorName;
                model.FathersName = member.FathersName;
                model.MothersName = member.MothersName;
                model.PresentAddress = member.PresentAddress;
                model.PermanentAddress = member.PermanentAddress;
                model.NID = member.NID;
                model.Phone1 = member.Phone1;
                model.Phone2 = member.Phone2;
                model.NomineeName = member.NomineeName;
                model.RelationWithNominee = member.RelationWithNominee;
                model.NomineeNIDOrBirthCertificate = member.NomineeNIDOrBirthCertificate;
                return View(model);
            }
            else
            {
                //ViewBag.Error = "Member Not Found";
                return View();
            }
        }

        public ActionResult AddMember()
        {
            //ViewBag.PageTitle = "Member Registration";

            if (TempData["SMsg"] != null)
                ViewBag.SMsg = TempData["SMsg"];
            if (TempData["EMsg"] != null)
                ViewBag.EMsg = TempData["EMsg"];
            if (TempData["WMsg"] != null)
                ViewBag.WMsg = TempData["WMsg"];

            return View();
        }

        public ActionResult Position()
        {
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;
            var model = db.tbl_member_leadeship_position.Where(x => x.MemberID == memberId).OrderBy(o=>o.ID).ToList();

            return View(model);
        }

        public ActionResult PurchaseHistory()
        {
            return View();
        }

        public ActionResult ShowPurchaseHistory(string FromDate,string ToDate)
        {
            DateTime fromdate = Convert.ToDateTime(FromDate);
            DateTime todate = Convert.ToDateTime(ToDate);
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;
            var model = db.tbl_purchase.Where(x => x.MemberID == memberId && x.OrderDate >= fromdate && x.OrderDate <= todate).ToList();
            return View(model);
        }

        public ActionResult PaymentHistory()
        {
            return View();
        }

        public ActionResult ProductPurchase(string sms = "")   
        {
            if (sms != "")
            {
                ViewBag.SMS = sms;
            }

            if (TempData["SMsg"] != null)
                ViewBag.SMsg = TempData["SMsg"];
            if (TempData["EMsg"] != null)
                ViewBag.EMsg = TempData["EMsg"];

            var productList = db.tbl_products_master.ToList();
            return View(productList);
        }

        public ActionResult ShowPaymentHistory()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult addedtocart(string MemberID)
        {
            var carts = db.tbl_add_to_cart.Where(x => x.MemberID == MemberID).ToList();
            return PartialView(carts);
        }
        public ActionResult ProductAddToCart(int ProductID)
        {
            string sms = "";
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;
            var isAlreadyAdded = db.tbl_add_to_cart.FirstOrDefault(x => x.ProductID == ProductID && x.MemberID == memberId);
            if (isAlreadyAdded == null)
            {
                var getProduct = db.tbl_products_master.FirstOrDefault(x => x.ProductID == ProductID);
                if (getProduct != null)
                {
                    tbl_add_to_cart adc = new tbl_add_to_cart();
                    adc.MemberID = memberId;
                    adc.ProductID = getProduct.ProductID;
                    adc.ProductName = getProduct.ProductName;
                    adc.Category = getProduct.Category;
                    adc.Type = getProduct.Type;
                    adc.Code = getProduct.Code;
                    adc.MRP = getProduct.MRP;
                    adc.DP = getProduct.DP;
                    adc.MRP = getProduct.MRP;
                    adc.WP = getProduct.WP;
                    adc.PP = getProduct.PP;
                    adc.RB = getProduct.RB;
                    adc.BP = getProduct.BP;
                    adc.Quantity = 1;
                    adc.ProductDetails = getProduct.ProductDetails;
                    adc.UserName = User.Identity.Name;
                    adc.Vat = getProduct.Vat * (decimal)adc.Quantity;
                    db.tbl_add_to_cart.Add(adc);
                    db.SaveChanges();

                    sms = "Successfully Added To Cart.";
                    return RedirectToAction("ProductPurchase", new { sms = sms });
                }
                else
                {
                    sms = "Add To Cart Failed.";
                    return RedirectToAction("ProductPurchase", new { sms = sms });
                }
            }
            else
            {
                isAlreadyAdded.Quantity = isAlreadyAdded.Quantity + 1;
                db.SaveChanges();
                return RedirectToAction("ProductPurchase", new { sms = sms });
            }
            //else
            //{
            //    sms = "Already Added To Cart.";
            //    return RedirectToAction("ProductPurchase", new { sms = sms });
            //}

        }

        public ActionResult CartDetails()
        {
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;

            decimal totalVat = 0;
            double totalAmount = 0;
            var addToCartList = db.tbl_add_to_cart.Where(x => x.MemberID == memberId).ToList();

            if (addToCartList.Count == 0)
                return RedirectToAction("ProductPurchase");

            List<OrderViewModel> cartList = new List<OrderViewModel>();
            addToCartList.ForEach(x =>
            {
                OrderViewModel cart = new OrderViewModel();
                cart.ProductName = db.tbl_products_master.FirstOrDefault(y => y.ProductID == x.ProductID).ProductName;
                cart.ProductTypeID = (long)x.Type;
                cart.MRP = (double)x.MRP;
                cart.DP = (double)x.DP;
                cart.MemberID = x.MemberID;
                cart.BP = (int)x.BP;
                cart.RB = (int)x.RB;
                cart.Quantity = (int)x.Quantity;
                cart.TotalAmount = cart.Quantity * cart.DP;
                cart.vat = (decimal)x.Vat;
                totalVat += cart.vat * (int)cart.Quantity;
                totalAmount += cart.TotalAmount;
                cartList.Add(cart);
            });
            ViewBag.totalshiping = db.tbl_shipping_charge.FirstOrDefault().ShipCharge;
            ViewBag.totalVat = totalVat;
            ViewBag.totalAmount = totalAmount;
            return View(cartList);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoicedOrderDetails(Int64 OrderID)
        {
            var order = db.tbl_orders.Where(x => x.OrderID == OrderID)
                .Select(x=>new OrderInvoiceDetailsViewModel()
                {
                    OrderID=x.OrderID,
                    OrderDate=x.OrderDateTime,
                    PaymentMethod=x.PaymentType,
                    PaymentAddress=x.PaymentAddress,
                    ShippingAddress=x.ShipmentAddress,
                    OrderStatus=x.OrderStatus

                }).FirstOrDefault();


            switch(order.PaymentMethod)
            {
                case "MP":
                    order.PaymentMethod = "bKash/Rocket";
                    break;

                case "COD":
                    order.PaymentMethod = "Cash on Delivery";
                    break;

                case "WP":
                    order.PaymentMethod = "Wallet Payment";
                    break;
            }

            var items = db.tbl_order_Line_item.Where(x => x.OrderID == OrderID).ToList();

            foreach(var item in items)
            {
                OrderInvoicedProduct product = new OrderInvoicedProduct();
                product.ProductID = item.ProductID;
                product.ProductCode = item.Code;
                product.ProductName = item.ProductName;
                product.Quantity = Convert.ToInt16(item.Quantity);
                product.MRP = (double)item.MRP;
                product.DP = (double)item.DP;
                product.PV = (Int16)item.BP;
                product.BV = (Int16)item.RB;
                product.Vat = (double)item.Vat;
                product.TotalVat = product.Vat * product.Quantity;
                product.TotalDP = product.DP * product.Quantity;

                order.TotalPrice += product.TotalDP;
                order.TotalVat += product.TotalVat;

                order.InvoicedProducts.Add(product);
            }

            order.TotalPriceIncludingVat = order.TotalPrice;
            order.TotalShippingCharge=(double)db.tbl_shipping_charge.FirstOrDefault().ShipCharge;

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFromCart(string ProductID, string MemberID)
        {
            try
            {
                Int64 productID = -1;
                if (!String.IsNullOrWhiteSpace(ProductID) && !String.IsNullOrWhiteSpace(MemberID))
                {
                    productID = Convert.ToInt64(ProductID);
                    var item = db.tbl_add_to_cart.Where(x => x.MemberID == MemberID && x.ProductID == productID).FirstOrDefault();

                    if(item!=null)
                    {
                        db.tbl_add_to_cart.Remove(item);
                        db.SaveChanges();
                        TempData["SMsg"] = "Successfully product deleted";
                    }
                }
                
            }
            catch
            {
                TempData["EMsg"] = "Can't delete that product";
            }

            return RedirectToAction("ProductPurchase");
        }

        public ActionResult PostOrder(string PaymentAddress, string ShipmentAddress, string PaymentMode)    
        {
            try
            {
                string getLogInUser = User.Identity.Name;
                string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;

                var addToCartList = db.tbl_add_to_cart.Where(x => x.MemberID == memberId).ToList();

                if(addToCartList.Count==0)
                {
                    return Json(new { Success = false, Message = "Please add items in to cart" }, JsonRequestBehavior.AllowGet);
                }

                tbl_orders order = new tbl_orders();
                var settingMaster = db.tbl_kdm_settings_master.FirstOrDefault();
                order.OrderID = (Int64)settingMaster.NextOrderID;
                settingMaster.NextOrderID = order.OrderID + 1;
                db.Entry(settingMaster).State = EntityState.Modified;

                order.MemberID = addToCartList.FirstOrDefault().MemberID;
                order.TotalAmount = 0;
                order.TotalPV = 0;
                order.TotalBV = 0;
                order.PaymentAddress = PaymentAddress;
                order.ShipmentAddress = ShipmentAddress;
                order.OrderStatus = "Pending";
                order.OrderDateTime = DateTime.Now;
                order.OrderBy = User.Identity.Name;
                order.OrderApprovedBy = "";
                order.OrderCanceledBy = "";
                order.PaymentType = PaymentMode;

                addToCartList.ForEach(x =>
                {
                    order.TotalAmount += x.MRP;
                    order.TotalBV += x.RB;
                    order.TotalPV += x.BP;

                    tbl_order_Line_item line = new tbl_order_Line_item();
                    line.OrderID = order.OrderID;
                    line.ProductID = x.ProductID;
                    line.ProductName = x.ProductName;
                    line.Category = x.Category;
                    line.Type = x.Type;
                    line.Code = x.Code;
                    line.MRP = x.MRP;
                    line.WP = x.WP;
                    line.PP = x.PP;
                    line.RB = x.RB;
                    line.BP = x.BP;
                    line.DP = x.DP;
                    line.Quantity = x.Quantity;
                    line.ProductDetails = x.ProductDetails;
                    line.Vat = x.Vat;
                    db.tbl_order_Line_item.Add(line);
                });
                db.tbl_orders.Add(order);
                db.tbl_add_to_cart.RemoveRange(addToCartList);
                db.SaveChanges();
                ViewBag.SuccessMessage = "Order successfully Placed";
                return Json(new { Success = true, Message = "Successfully Order Placed", OrderId = order.OrderID }, JsonRequestBehavior.AllowGet); ;
            }
            catch
            {
                return Json(new { Success = false, Message = "Can't place the order" }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult BinaryIncome()
        {
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;
            var model = db.tbl_binary_matching_data.Where(x => x.PlacementID == memberId).ToList();
            return View(model);
           
        }
        public ActionResult GenerationIncome()
        {
            return View();
        }
        public ActionResult Tree()
        {
            string userName = User.Identity.Name;
            var getMember = db.tbl_members.FirstOrDefault(x => x.Phone1 == userName);
            if (!String.IsNullOrWhiteSpace(getMember.PlacementID))
            {
                BTreeHelpers bHelper = new BTreeHelpers();

                NodeView root = new NodeView(getMember.PlacementID);
                var jsonData = bHelper.BuildMemberTreeBFS(root, 5);
                ViewBag.JsonData = jsonData;
            }
            else
            {
                ViewBag.JsonData = "{}";
            }

            return View();
        }
        public ActionResult Withdrawl()
        {          
            
            return View();
        }
        [HttpPost]
        public ActionResult WithdrawlAction()
        {

            return View();
        }

        public ActionResult Accounts()
        {
            return View();
        }
        
        [HttpGet]
        public ActionResult ChangePassword(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "CHANGE MY PASSWORD";
            ViewBag.PanelTitles = PanelTitles;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangeMyPasswordViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.PageTitle = "USER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["PanelTitle1"] = "CHANGE MY PASSWORD";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please check the inputs";
                return View(model);
            }

            IdentityManager im = new IdentityManager();

            string userId = User.Identity.GetUserId();
            //string userId = "398574f6-c390-4f76-b6a8-84109e330f46";

            if (im.ChangePassword(userId, model.OldPassword, model.NewPassword))
            {
                im.DisablePasswordChangeFlag(userId);
                ViewBag.SuccessMessage = "Password Successfully Changed";
            }
            else
                ViewBag.ErrorMessage = "Can't Change the Password";

            return View();
        }

        [HttpGet]
        public string GETMemberID(string UserName)
        {
            var SPName = db.tbl_members.FirstOrDefault(x => x.Phone1 == UserName);
            string memberId = "";

            if (SPName != null)
            {
                memberId = SPName.MemberID;
            }
            return memberId;
        }

        public ActionResult PendingOrder()
        {
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;

            var Orders = db.tbl_orders.FirstOrDefault(x => x.MemberID == memberId);
            if(Orders == null)
            {
                return View("~/Views/KDMMember/no_pending_order.cshtml");
            }
            var getOrder = db.tbl_orders.FirstOrDefault(x => x.OrderID == Orders.OrderID);
            ViewBag.PageTitle = "ORDER MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["OrderDetails"] = "Order List";
            ViewBag.PanelTitles = PanelTitles;
            var orders = db.tbl_orders.Where(x => x.OrderStatus == "Pending" && x.MemberID== memberId).ToList();
            List<OrderViewModel> model = new List<OrderViewModel>();
            if (model != null)
            {
                orders.ForEach(x =>
                {
                    OrderViewModel order = new OrderViewModel();
                    order.OrderID = x.OrderID;
                    order.MemberID = x.MemberID;
                    order.MembeName = db.tbl_members.FirstOrDefault(y => y.MemberID == x.MemberID).DistributorName;
                    order.TotalAmount = (double)x.TotalAmount;
                    //order.DealerID = x.DealerID;
                    order.PaymentAddress = x.PaymentAddress;
                    order.ShipmentAddress = x.ShipmentAddress;
                    order.OrderStatus = x.OrderStatus;
                    order.OrderDateTime = (DateTime)x.OrderDateTime;
                    order.OrderBy = x.OrderBy;
                    order.OrderApprovedBy = "";
                    order.OrderCanceledBy = "";

                    model.Add(order);
                });

                return View(model);
            }
            else
            {
                return View();
            }

        }

        public ActionResult PVBVhistory()
        {
            return View();
        }

        public ActionResult ShowMyPvBvHistory(string FromDate, string ToDate)
        {
            DateTime fromdate = Convert.ToDateTime(FromDate);
            DateTime todate = Convert.ToDateTime(ToDate);
            string getLogInUser = User.Identity.Name;
            string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;
            var model = db.tbl_PvBv_history.Where(x => x.MemberID == memberId && x.PostingDate >= fromdate && x.PostingDate <= todate).ToList();
            return View(model);
        }


    }
}