using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using KDM.Models;
using Serilog;
using KDM.Helpers;
using KDM.Filters;
using System.Globalization;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class OrderController : Controller
    {
        KDMDB db = new KDMDB();
        // GET: Order
        [HttpGet]
        public ActionResult CreateOrder()
        {
            ViewBag.PageTitle = "Order MANAGEMENT";
            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddNewOrder"] = "ADD NEW Order";
            ViewBag.PanelTitles = PanelTitles;
            ViewBag.LoadProducts = ViewHelper.LoadProductFromDB();
            return View();
        }

        public ActionResult GetOrderListByUser()
        {
            string getLogInUser = User.Identity.Name;
            var addTocartListByUser = db.tbl_add_to_cart.Where(x => x.UserName == getLogInUser).ToList();
            addTocartListByUser.ForEach(x =>
            {
                x.MemberName = db.tbl_members.FirstOrDefault(y => y.MemberID == x.MemberID).DistributorName;
            });

            return Json(addTocartListByUser, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult InvoicedOrderDetails(Int64 OrderID)
        {
            var order = db.tbl_orders.Where(x => x.OrderID == OrderID)
                .Select(x => new OrderInvoiceDetailsViewModel()
                {
                    OrderID = x.OrderID,
                    OrderDate = x.OrderDateTime,
                    PaymentMethod = x.PaymentType,
                    PaymentAddress = x.PaymentAddress,
                    ShippingAddress = x.ShipmentAddress,
                    OrderStatus = x.OrderStatus

                }).FirstOrDefault();


            switch (order.PaymentMethod)
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

            foreach (var item in items)
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
            order.TotalShippingCharge = (double)db.tbl_shipping_charge.FirstOrDefault().ShipCharge;

            return View(order);
        }

        //[HttpPost]
        //public ActionResult CreateOrder(OrderViewModel orderViewModel)
        //{
        //    ViewBag.PageTitle = "Order MANAGEMENT";
        //    Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
        //    PanelTitles["AddNewOrder"] = "ADD NEW Order";
        //    ViewBag.PanelTitles = PanelTitles;
        //    if (!ModelState.IsValid)
        //    {
        //        ViewBag.ErrorMessage = "Can't Add the Order.";
        //        return View(orderViewModel);
        //    }
        //    else
        //    {
        //        //ViewBag.SuccessMessage = "Order successfully Added";
        //        //var lastOrder = db.tbl_orders.ToList().LastOrDefault();
        //        //if (lastOrder != null)
        //        //{
        //        //    orderViewModel.OrderID = lastOrder.OrderID + 1;
        //        //}
        //        //else
        //        //{
        //        //    orderViewModel.OrderID = 1;
        //        //}
        //        orderViewModel.OrderBy = User.Identity.Name;
        //        orderViewModel.OrderDateTime = DateTime.Now;
        //        ModelState.Clear();
        //        return RedirectToAction("OrderDetails", orderViewModel);
        //    }
        //}

        //created by jobaed 18-09-2020
        [HttpPost]
        public ActionResult PostOrder(OrderViewModel orderViewModel)
        {
            ViewBag.PageTitle = "ORDER MANAGEMENT";
            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddNewOrder"] = "ADD NEW ORDER";
            ViewBag.PanelTitles = PanelTitles;
            var isAlreadyAdded = db.tbl_add_to_cart.FirstOrDefault(x => x.Code == orderViewModel.ProductCode && x.MemberID == orderViewModel.MemberID);
            if (isAlreadyAdded == null)
            {
                var getProduct = db.tbl_products_master.FirstOrDefault(x => x.Code == orderViewModel.ProductCode);
                if (getProduct != null)
                {
                    tbl_add_to_cart adc = new tbl_add_to_cart();
                    adc.MemberID = orderViewModel.MemberID;
                    adc.ProductID = getProduct.ProductID;
                    adc.ProductName = getProduct.ProductName;
                    adc.Category = getProduct.Category;
                    adc.Type = getProduct.Type;
                    adc.Code = getProduct.Code;
                    adc.DP = getProduct.DP; //* (int)orderViewModel.Quantity;
                    adc.MRP = getProduct.MRP; // * (int)orderViewModel.Quantity;
                    adc.WP = getProduct.WP; // *(int)orderViewModel.Quantity;
                    adc.PP = getProduct.PP; // * (int)orderViewModel.Quantity;
                    adc.RB = orderViewModel.RB; // * (int)orderViewModel.Quantity;
                    adc.BP = orderViewModel.BP; // * (int)orderViewModel.Quantity;
                    adc.Quantity = (int)orderViewModel.Quantity;
                    adc.ProductDetails = getProduct.ProductDetails;
                    adc.UserName = User.Identity.Name;
                    adc.Vat = getProduct.Vat; // * (decimal)orderViewModel.Quantity;
                    db.tbl_add_to_cart.Add(adc);
                    db.SaveChanges();
                    return Json(adc, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var getProduct = db.tbl_products_master.FirstOrDefault(x => x.Code == orderViewModel.ProductCode);
                if (getProduct != null)
                {
                    isAlreadyAdded.MemberID = orderViewModel.MemberID;
                    isAlreadyAdded.ProductID = getProduct.ProductID;
                    isAlreadyAdded.ProductName = getProduct.ProductName;
                    isAlreadyAdded.Category = getProduct.Category;
                    isAlreadyAdded.Type = getProduct.Type;
                    isAlreadyAdded.Code = getProduct.Code;
                    isAlreadyAdded.MRP = getProduct.MRP;
                    isAlreadyAdded.WP = getProduct.WP;
                    isAlreadyAdded.PP = getProduct.PP;
                    isAlreadyAdded.RB = orderViewModel.RB;
                    isAlreadyAdded.BP = orderViewModel.BP;
                    isAlreadyAdded.Quantity = (int)orderViewModel.Quantity;
                    isAlreadyAdded.ProductDetails = getProduct.ProductDetails;
                    isAlreadyAdded.UserName = User.Identity.Name;
                    isAlreadyAdded.Vat = getProduct.Vat * (decimal)orderViewModel.Quantity;
                    db.Entry(isAlreadyAdded).State = EntityState.Modified;
                    db.SaveChanges();
                    return Json(isAlreadyAdded, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
        }

        public ActionResult DeleteFromAddToCart(string productCode)
        {
            string getLogInUser = User.Identity.Name;
            var addToCartList = db.tbl_add_to_cart.FirstOrDefault(x => x.UserName == getLogInUser && x.Code == productCode);
            if (addToCartList != null)
            {
                db.tbl_add_to_cart.Remove(addToCartList);
                db.SaveChanges();
            }
            return Json(addToCartList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult OrderDetailsForPostOrder()
        {
            string getLogInUser = User.Identity.Name;
            var addToCartList = db.tbl_add_to_cart.Where(x => x.UserName == getLogInUser).ToList();
            decimal totalVat = 0;
            double totalAmount = 0;
            List<OrderViewModel> cartList = new List<OrderViewModel>();
            addToCartList.ForEach(x =>
            {
                OrderViewModel cart = new OrderViewModel();
                cart.ProductName = db.tbl_products_master.FirstOrDefault(y => y.ProductID == x.ProductID).ProductName;
                cart.ProductTypeID = (long)x.Type;
                cart.DP = (double)x.DP;
                cart.MemberID = x.MemberID;
                cart.BP = (int)x.BP;
                cart.RB = (int)x.RB;
                cart.Quantity = (int)x.Quantity;
                cart.TotalAmount = cart.DP * cart.Quantity;
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

        //[HttpGet]
        //public ActionResult OrderDetails(OrderViewModel orderViewModel)
        //{
        //    ViewBag.OrderId = orderViewModel.OrderID;
        //    ViewBag.PageTitle = "Order MANAGEMENT 2";
        //    Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
        //    PanelTitles["OrderDetails"] = "Order Details";
        //    ViewBag.PanelTitles = PanelTitles;

        //    return View(orderViewModel);
        //}

        [HttpGet]
        public ActionResult OrderSave(string PaymentAddress, string ShipmentAddress, string PaymentMode)
        {
            string getLogInUser = User.Identity.Name;
            //string memberId = db.tbl_members.FirstOrDefault(x => x.Phone1 == getLogInUser).MemberID;
            var addToCartList = db.tbl_add_to_cart.Where(x => x.UserName == getLogInUser).ToList();

            if (addToCartList.Count == 0)
            {
                return Json(new { Success = false, Message = "No order to place" }, JsonRequestBehavior.AllowGet);
            }

            if (PaymentMode != "MP" && PaymentMode != "COD" && PaymentMode != "WP")
                return Json(new { Success = false, Message = "Select the payment type" }, JsonRequestBehavior.AllowGet);

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
                line.DP = x.DP;
                line.SP = x.SP;
                line.RB = x.RB;
                line.BP = x.BP;
                line.Quantity = x.Quantity;
                line.ProductDetails = x.ProductDetails;
                line.Vat = x.Vat;
                db.tbl_order_Line_item.Add(line);
                order.TotalAmount += (x.DP);
                order.TotalBV += x.RB;
                order.TotalPV += x.BP;
            });
            db.tbl_orders.Add(order);
            db.tbl_add_to_cart.RemoveRange(addToCartList);
            db.SaveChanges();
            ViewBag.SuccessMessage = "Order successfully Placed";
            return Json(order, JsonRequestBehavior.AllowGet);

            //ViewBag.PageTitle = "Order MANAGEMENT 2";

            //Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            //PanelTitles["OrderDetails"] = "Order Details";
            //ViewBag.PanelTitles = PanelTitles;

            //tbl_orders order = new tbl_orders();
            //order.ProductID = db.tbl_products_master.FirstOrDefault(x => x.Code == orderViewModel.ProductCode).ProductID;
            //order.Quantity = (decimal)orderViewModel.Quantity;
            //order.ProductName = "";
            //order.ProductTypeID = orderViewModel.ProductTypeID;
            //order.ProductTypeName = "";
            //order.MRP = (decimal)orderViewModel.MRP;
            //order.MemberID = orderViewModel.MemberID;
            //order.BP = orderViewModel.BP;
            //order.RB = orderViewModel.RB;
            //order.TotalAmount = (decimal)orderViewModel.TotalAmount;
            //order.TotalPV = orderViewModel.TotalPV;
            //order.TotalBV = orderViewModel.TotalBV;
            //order.DealerID = orderViewModel.DealerID;
            //order.PaymentAddress = orderViewModel.PaymentAddress;
            //order.ShipmentAddress = orderViewModel.ShipmentAddress;
            //order.OrderStatus = "Pending";
            //order.OrderDateTime = DateTime.Now;
            //order.OrderBy = orderViewModel.OrderBy;
            //order.OrderApprovedBy = "";
            //order.OrderCanceledBy = "";

            //db.tbl_orders.Add(order);
            //db.SaveChanges();

            //order.OrderID = db.tbl_orders.ToList().LastOrDefault().OrderID;
            //ViewBag.SuccessMessage = "Order successfully Placed";

            //return Json(order, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderList()
        {
            if (TempData["SMsg"] != null)
                ViewBag.SMsg = TempData["SMsg"];
            if (TempData["WMsg"] != null)
                ViewBag.WMsg = TempData["WMsg"];
            if (TempData["EMsg"] != null)
                ViewBag.EMsg = TempData["EMsg"];

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["OrderDetails"] = "Order List";
            ViewBag.PanelTitles = PanelTitles;
            var orders = db.tbl_orders.Where(x => x.OrderStatus == "Pending").ToList();
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


        // Method ID: M_0000013
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderApprove(int OrderId)
        {
            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    int totalPVForTemp = 0;
                    #region Update Order Status
                    var order = db.tbl_orders.FirstOrDefault(x => x.OrderID == OrderId);

                    if (order != null && order.MemberID != "")
                    {
                        var orderLine = db.tbl_order_Line_item.Where(x => x.OrderID == order.OrderID).ToList();
                        

                        orderLine.ForEach(x =>
                        {

                            #region Update Member Tree

                            var memberExist = db.tbl_members.FirstOrDefault(y => y.MemberID == order.MemberID);

                            if(memberExist==null)
                            {
                                throw new Exception("Member not found in members");
                            }
                            var member = db.tbl_member_tree.Where(m => m.PlacementID == memberExist.PlacementID).FirstOrDefault();

                            if (member == null)
                            {
                                throw new Exception("Member not found in member tree");
                            }

                            member.PV = (member.PV??0) + (x.BP*x.Quantity);
                            member.BV = (member.BV??0) + (x.RB*x.Quantity);

                            totalPVForTemp += (int)x.BP * (int)x.Quantity;

                            db.Entry(member).State = EntityState.Modified;

                            #endregion

                            // db.SaveChanges();

                            #region Purchase Entry

                            tbl_purchase tblpur = new tbl_purchase();
                            tblpur.OrderID = OrderId;
                            tblpur.ProductID = x.ProductID;
                            tblpur.ProductName = x.ProductName;
                            tblpur.MRP = x.MRP;
                            tblpur.DP = x.DP;
                            tblpur.SP = x.SP;
                            tblpur.MemberID = order.MemberID;
                            tblpur.BP = x.BP;
                            tblpur.RB = x.RB;
                            //tblpur.DealerID
                            tblpur.PaymentAddress = order.PaymentAddress;
                            tblpur.ShipmentAddress = order.ShipmentAddress;
                            tblpur.OrderStatus = KDMOrderStatus.Approved;
                            tblpur.OrderDate = order.OrderDateTime;
                            tblpur.OrderBy = order.OrderBy;
                            tblpur.OrderApprovedBy = User.Identity.Name;
                            tblpur.OrderCanceledBy = "";
                            tblpur.Quantity = x.Quantity;
                            tblpur.PaymentType = order.PaymentType;
                            tblpur.TotalAmount = x.DP;
                            tblpur.TotalPV = 0;
                            tblpur.TotalBV = 0;
                            tblpur.StatusUpdateDate = DateTime.Now;
                            db.tbl_purchase.Add(tblpur);

                            #endregion

                            #region Vat Entry

                            tbl_vat_account_data vad = new tbl_vat_account_data();

                            var rowCount = db.tbl_vat_account_data.ToList();
                            int lastRow = 0;
                            if (rowCount.Count > 0)
                            {
                                lastRow = (int)db.tbl_vat_account_data.Max(y => y.trSerialNo);
                                vad.trSerialNo = lastRow + 1;
                            }
                            else
                            {
                                vad.trSerialNo = 1;
                            }
                            decimal balance = (decimal)db.tbl_vat_account_data.Where(z => z.trSerialNo == lastRow).Select(s => s.Balance).FirstOrDefault();
                            vad.ForAccount = order.OrderID.ToString();
                            vad.PurposeCode = 1;
                            vad.DebitAmount = 0;
                            vad.CreditAmount = x.Vat;
                            vad.Balance = db.tbl_vat_account_data.FirstOrDefault(y => y.trSerialNo == lastRow).Balance ?? 0 + vad.CreditAmount;
                            vad.PostingDate = DateTime.Now.Date;
                            vad.PostingTime = DateTime.Now.TimeOfDay;
                            vad.PostedBy = User.Identity.Name;
                            vad.ApprovedBy = User.Identity.Name;
                            db.tbl_vat_account_data.Add(vad);

                            #endregion
                        });

                        db.tbl_orders.Remove(order);
                        db.tbl_order_Line_item.RemoveRange(orderLine);

                    }
                    #endregion

                    #region Bonus Processing
                    KDMTRHelper trHelper = new KDMTRHelper();

                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.Wallet, (double)order.TotalPV);
                    trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.Sponsor, (double)totalPVForTemp);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.BinaryMatching, (double)order.TotalPV);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.Generation, (double)order.TotalBV);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.MonthlyRoyalty, (double)order.TotalPV);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.Performance, (double)order.TotalAmount);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.Leadership, (double)order.TotalAmount);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.Rank, (double)order.TotalAmount);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.RoyalClub, (double)order.TotalAmount);
                    //trHelper.UpdateMemberBonusTemp(db, order.MemberID, KDMBonusConstants.ECommerce, (double)order.TotalAmount);

                    //trHelper.GiveWalletBonus(order.MemberID);
                    trHelper.GiveSponsorBonus(db,order.MemberID);
                    //trHelper.UpdateFundMonthlyRoyalityBonusByMemberSource(order.MemberID);
                    //trHelper.UpdateFundPerformanceBonusByMemberSource(order.MemberID);
                    //trHelper.UpdateFundLadershipBonusByMemberSource(order.MemberID);
                    //trHelper.UpdateFundRoyalClubBonusByMemberSource(order.MemberID);
                    //trHelper.UpdateFundECommerceBonusByMemberSource(order.MemberID);


                    #endregion

                    db.SaveChanges();
                    tr.Commit();
                    TempData["SMsg"] = "Order# " + OrderId + " approved";
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Log.Error(ex, "[M_0000013] [ERROR]");
                    TempData["EMsg"] = "Can't approve the order# " + OrderId;
                }
            }

            return RedirectToAction("OrderList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderCencel(int OrderId)
        {
            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    #region Update Order Status
                    var order = db.tbl_orders.FirstOrDefault(x => x.OrderID == OrderId);

                    if (order != null)
                    {
                        var orderLine = db.tbl_order_Line_item.Where(x => x.OrderID == order.OrderID).ToList();
                        orderLine.ForEach(x =>
                        {
                            tbl_purchase tblpur = new tbl_purchase();
                            tblpur.OrderID = OrderId;
                            tblpur.ProductID = x.ProductID;
                            tblpur.ProductName = x.ProductName;
                            //tblpur.ProductTypeID = "";
                            //tblpur.ProductTypeName = "";
                            tblpur.MRP = x.MRP;
                            tblpur.MemberID = order.MemberID;
                            tblpur.BP = x.BP;
                            tblpur.RB = x.RB;
                            //tblpur.DealerID
                            tblpur.PaymentAddress = order.PaymentAddress;
                            tblpur.ShipmentAddress = order.ShipmentAddress;
                            tblpur.OrderStatus = KDMOrderStatus.Cancled;
                            // tblpur.OrderDateTime = order.OrderDateTime;
                            tblpur.OrderBy = order.OrderBy;
                            //tblpur.OrderApprovedBy = "";
                            tblpur.OrderCanceledBy = User.Identity.Name;
                            tblpur.Quantity = x.Quantity;
                            tblpur.PaymentType = order.PaymentType;
                            tblpur.TotalAmount = x.Quantity * x.DP;
                            tblpur.TotalPV = 0;
                            tblpur.TotalBV = 0;
                            tblpur.StatusUpdateDate = DateTime.Now;
                            db.tbl_purchase.Add(tblpur);
                        });

                        db.tbl_orders.Remove(order);
                        db.tbl_order_Line_item.RemoveRange(orderLine);

                    }
                    #endregion

                    db.SaveChanges();
                    tr.Commit();
                    TempData["SMsg"] = "Order# " + OrderId + " cancled";
                }
                catch (Exception ex)
                {
                    tr.Rollback();
                    Log.Error(ex, "[M_0000013] [ERROR]");
                    TempData["EMsg"] = "Can't cancel the order# " + OrderId;
                }
            }

            return RedirectToAction("OrderList");
        }

        [HttpGet]
        public ActionResult OrderDetailsFromList(int OrderId)
        {
            var getOrder = db.tbl_orders.FirstOrDefault(x => x.OrderID == OrderId);
            var getOrderList = db.tbl_order_Line_item.Where(x => x.OrderID == OrderId).ToList();
            decimal totalVat = 0;
            double totalAmount = 0;
            List<OrderViewModel> cartList = new List<OrderViewModel>();
            getOrderList.ForEach(x =>
            {
                OrderViewModel cart = new OrderViewModel();
                cart.ProductName = db.tbl_products_master.FirstOrDefault(y => y.ProductID == x.ProductID).ProductName;
                cart.ProductTypeID = (long)x.Type;
                cart.MRP = (double)x.MRP;
                cart.DP = (double)x.DP;
                cart.MemberID = getOrder.MemberID;
                cart.BP = (int)x.BP;
                cart.RB = (int)x.RB;
                cart.Quantity = (int)x.Quantity;
                cart.TotalAmount = cart.DP * (int)x.Quantity;
                cart.vat = (decimal)x.Vat;

                totalVat += cart.vat * (int)x.Quantity;
                totalAmount += cart.TotalAmount;
                cartList.Add(cart);
            });

            ViewBag.totalshiping = db.tbl_shipping_charge.FirstOrDefault().ShipCharge;
            ViewBag.totalVat = totalVat;
            ViewBag.totalAmount = totalAmount;

            return View(cartList);

        }

        public ActionResult GetProductDetails(string ProductCode)
        {
            tbl_products_master product = new tbl_products_master();
            product.Code = ProductCode;

            product = db.tbl_products_master.Where(x => x.Code == ProductCode).FirstOrDefault();

            return Json(product, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Saleshistory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Saleshistory(string FromDate, string ToDate, string status)
        {
            CultureInfo culture = new CultureInfo("bn-BD");
            DateTime startDate = Convert.ToDateTime(FromDate, culture);
            DateTime endDate = Convert.ToDateTime(ToDate, culture);

            List<SalesHistory> data = new List<SalesHistory>();
            data = db.tbl_purchase.Where(x => x.OrderDate >= startDate && x.OrderDate <= endDate && x.OrderStatus == status)
                .Select(x => new SalesHistory
                {
                    MemberID = x.MemberID,
                    OrderID = x.OrderID,
                    ProductCode = db.tbl_products_master.Where(y => y.ProductID == x.ProductID).Select(y => y.Code).FirstOrDefault(),
                    ProductName = x.ProductName,
                    MRP = (double)(x.MRP ?? 0),
                    DP = (double)(x.DP ?? 0),
                    Qty = (int)(x.Quantity ?? 0),
                    TotalAmount = (double)((x.DP ?? 0) * (x.Quantity ?? 0)),
                    TotalVat = (double)((x.MRP ?? 0) * (x.Quantity ?? 0)) * 0.15,
                    PV = (int)(x.BP ?? 0),
                    BV = (int)(x.RB ?? 0),
                    ShipmentAddress = x.ShipmentAddress,
                    PaymentMethod = x.PaymentType,
                    OrderStatus = x.OrderStatus,
                    OrderDate = x.OrderDate,
                    OrderBy = x.OrderBy,
                    ApprovedBy = x.OrderApprovedBy,
                    StatusUpdateDate = x.StatusUpdateDate
                })
                .OrderByDescending(o => o.OrderDate).ToList();

            if (data.Count == 0)
                ViewBag.EMsg = "No data found";

            return View(data);
        }


        [NonAction]
        public ActionResult SalesHistoryData(string FromDate, string ToDate)
        {
            DateTime fromdate = Convert.ToDateTime(FromDate);
            DateTime todate = Convert.ToDateTime(ToDate);

            var model = db.tbl_purchase.Where(x => x.OrderDate >= fromdate && x.OrderDate <= todate).OrderByDescending(o => o.OrderDate).ToList();
            return View(model);
        }

        public ActionResult PVBVHistory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PVBVHistory(string FromDate, string ToDate)
        {
            CultureInfo culture = new CultureInfo("bn-BD");
            DateTime startDate = Convert.ToDateTime(FromDate, culture);
            DateTime endDate = Convert.ToDateTime(ToDate, culture);

            List<SalesHistoryByInvoice> model = new List<SalesHistoryByInvoice>();

            model = db.tbl_orders.Where(o => o.OrderDateTime >= startDate && o.OrderDateTime <= endDate)
                .Select(o => new Models.SalesHistoryByInvoice()
                {
                    InvoiceNo=o.OrderID.ToString(),
                    InvoiceDate=o.OrderDateTime.ToString(),
                    MemberID=o.MemberID,
                    InvoicedBillAmount=o.TotalAmount.ToString(),
                    InvoicedPV=o.TotalPV.ToString(),
                    InvoicedBV=o.TotalBV.ToString(),
                    NotProcessedPV=db.tbl_member_tree.Where(x=>x.PlacementID==o.MemberID).Select(x=>x.PV).FirstOrDefault().ToString(),
                    NotProcessedBV = db.tbl_member_tree.Where(x => x.PlacementID == o.MemberID).Select(x => x.BV).FirstOrDefault().ToString(),

                }).ToList();

            return View(model);
        }
    }
}