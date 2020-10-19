using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf.qrcode;
using KDM.Models;
using Spire.License;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class ProductController : Controller
    {
        // GET: Product
        KDMDB db = new KDMDB();
        public ActionResult AddProduct()
        {
            ViewBag.PageTitle = "PRODUCT MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddProduct"] = "Add Product";
            ViewBag.PanelTitles = PanelTitles;

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductVModel model)
        {
            ViewBag.PageTitle = "PRODUCT MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddProduct"] = "Add Product";
            ViewBag.PanelTitles = PanelTitles;

            var codeExist = db.tbl_products_master.FirstOrDefault(x => x.Code.Replace(" ", "") == model.Code.Replace(" ", ""));

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Product Added Failed.";
                return View(model);
            }
            else if (codeExist != null)
            {
                ViewBag.ErrorMessage = "Product Code Already Exist.";
                return View(model);
            }
            else
            {
                using (KDMDB db = new KDMDB())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var lastProduct = db.tbl_products_master.ToList();

                            tbl_products_master product = new tbl_products_master();
                            product.ProductID= lastProduct.Max(x => x.ProductID) + 1;
                            product.ProductName = model.ProductName;
                            product.Category = model.Category;
                            product.Type = model.Type;
                            product.Code = model.Code;
                            product.MRP = model.MRP;
                            product.Unit = model.Unit;
                            product.DP = model.DistributorPrice;
                            product.SP = model.StokishPrice;
                            product.Vat = model.Vat;
                            product.WP = model.WalletPrice;
                            product.PP = model.PurchasePrice;
                            product.RB = model.RB;
                            product.BP = model.BP;
                            product.Quantity = model.Quantity;
                            product.ProductDetails = model.ProductDetails;
                            product.CreateDate = DateTime.Now;
                            product.CreateBy = User.Identity.Name;
                            product.ModificationDate = DateTime.Now; //Need to Confirm From niaz vai
                            product.ModifiedBy = User.Identity.Name; //Need to Confirm from niaz vai
                            db.tbl_products_master.Add(product);

                            tbl_products_history pHistory = new tbl_products_history();
                          
                            pHistory.ProductID = lastProduct.Max(x => x.ProductID) + 1;
                            pHistory.ProductName = model.ProductName;
                            pHistory.Category = model.Category;
                            pHistory.Type = model.Type;
                            pHistory.Code = model.Code;
                            pHistory.MRP = model.MRP;
                            pHistory.Unit = model.Unit;
                            pHistory.DistributorPrice = model.DistributorPrice;
                            pHistory.StokishPrice = model.StokishPrice;
                            pHistory.Vat = model.Vat;
                            pHistory.WalletPrice = model.WalletPrice;
                            pHistory.PurchasePrice = model.PurchasePrice;
                            pHistory.RB = model.RB;
                            pHistory.BP = model.BP;
                            pHistory.Quantity = model.Quantity;
                            pHistory.ProductDetails = model.ProductDetails;
                            pHistory.CreateBy = User.Identity.Name;
                            pHistory.CreateDate = DateTime.Now;
                            pHistory.Action = "Insert";
                            db.tbl_products_history.Add(pHistory);
                            db.SaveChanges();
                            transaction.Commit();
                            ModelState.Clear();
                            ViewBag.SuccessMessage = "Product Added Successfully.";
                            return View();
                        }
                        catch
                        {
                            transaction.Rollback();
                            ViewBag.ErrorMessage = "Product Added Failed.";
                            return View(model);
                        }
                    }
                }

            }
        }
        [Authorize]
        public ActionResult ProductList(string pName, string pCode, string pCategory, string pType)     
        {
            ViewBag.pName = (pName == null) ? "" : pName;
            ViewBag.pCode = (pCode == null) ? "" : pCode;
            ViewBag.pCategory = (pCategory == "0" || pCategory == null) ? "" : pCategory.ToString();
            ViewBag.pType = (pType == "0" || pType == null) ? "" : pType.ToString();

            ViewBag.PageTitle = "Product MANAGEMENT";
            List<ProductVModel> pmodel1 = TempData["productList"] as List<ProductVModel>;

            if (pmodel1 == null)
            {
                var productList = db.tbl_products_master.ToList();

                List<ProductVModel> model = new List<ProductVModel>();
                productList.ForEach(x =>
                {
                    ProductVModel product = new ProductVModel();
                    product.ProductID = x.ProductID;
                    product.ProductName = x.ProductName;
                    product.Category = (int)x.Category;
                    product.CategoryName = db.tbl_product_category.FirstOrDefault(y => y.Code == product.Category.ToString()).Name;
                    product.Type = (int)x.Type;
                    product.TypeName = db.tbl_product_type.FirstOrDefault(y => y.Id == product.Type).Name;
                    product.Code = x.Code;
                    product.MRP = (decimal)x.MRP;
                    product.Unit = x.Unit;
                    product.DistributorPrice = (decimal)x.DP;
                    product.StokishPrice = (decimal)x.SP;
                    product.Vat = (decimal)x.Vat;
                    product.WalletPrice = (decimal)x.WP;
                    product.PurchasePrice = (decimal)x.PP;
                    product.RB = (int)x.RB;
                    product.BP = (int)x.BP;
                    product.Quantity = (int)x.Quantity;
                    product.ProductDetails = x.ProductDetails;
                    product.CreateDate = (DateTime)x.CreateDate;
                    product.CreateBy = x.CreateBy;
                    product.ModificationDate = (DateTime)x.ModificationDate;
                    product.ModifiedBy = x.ModifiedBy;

                    model.Add(product);
                });

                return View(model);
            }
            else
            {
                return View(pmodel1);
            }
        }

        [HttpPost]
        public ActionResult SearchProduct(ProductVModel pModel)
        {
            var productList = db.tbl_products_master.ToList();
            if (pModel.ProductName != null)
            {
                productList = productList.Where(x => x.ProductName.ToLower().Contains(pModel.ProductName.ToLower())).ToList();
            }
            if (pModel.Code != null)
            {
                productList = productList.Where(x => x.Code.ToLower().Contains(pModel.Code.ToLower())).ToList();
            }
            if (pModel.Category != 0)
            {
                productList = productList.Where(x => x.Category == pModel.Category).ToList();
            }
            if (pModel.Type != 0)
            {
                productList = productList.Where(x => x.Type == pModel.Type).ToList();
            }
            if (pModel.ProductName == null && pModel.Code == null && pModel.Category == 0 && pModel.Type == 0)
            {
                productList = new List<tbl_products_master>();
            }

            List<ProductVModel> model = new List<ProductVModel>();
            productList.ForEach(x =>
            {
                ProductVModel product = new ProductVModel();
                product.ProductID = x.ProductID;
                product.ProductName = x.ProductName;
                product.Category = (int)x.Category;
                product.CategoryName = db.tbl_product_category.FirstOrDefault(y => y.Code == product.Category.ToString()).Name;
                product.Type = (int)x.Type;
                product.TypeName = db.tbl_product_type.FirstOrDefault(y => y.Id == product.Type).Name;
                product.Code = x.Code;
                product.MRP = (decimal)x.MRP;
                product.Unit = x.Unit;
                product.DistributorPrice = (decimal)x.DP;
                product.StokishPrice = (decimal)x.SP;
                product.Vat = (decimal)x.Vat;
                product.WalletPrice = (decimal)x.WP;
                product.PurchasePrice = (decimal)x.PP;
                product.RB = (int)x.RB;
                product.BP = (int)x.BP;
                product.Quantity = (int)x.Quantity;
                product.ProductDetails = x.ProductDetails;
                product.CreateDate = (DateTime)x.CreateDate;
                product.CreateBy = x.CreateBy;
                product.ModificationDate = (DateTime)x.ModificationDate;
                product.ModifiedBy = x.ModifiedBy;

                model.Add(product);
            });

            TempData["productList"] = model;

            return RedirectToAction("ProductList", new
            {
                pName = pModel.ProductName,
                pCode = pModel.Code,
                pCategory = pModel.Category,
                pType = pModel.Type
            });
        }

        [HttpGet]
        public ActionResult ProductEdit(int productId)
        {
            ViewBag.PageTitle = "Product MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["ProductEdit"] = "Product Edit";
            ViewBag.PanelTitles = PanelTitles;
            var productList = db.tbl_products_master.FirstOrDefault(x => x.ProductID == productId);

            ProductVModel product = new ProductVModel();
            product.ProductID = productList.ProductID;
            product.ProductName = productList.ProductName;
            product.Category = (int)productList.Category;
            product.CategoryName = db.tbl_product_category.FirstOrDefault(y => y.Code == product.Category.ToString()).Name;
            product.Type = (int)productList.Type;
            product.TypeName = db.tbl_product_type.FirstOrDefault(y => y.Id == product.Type).Name;
            product.Code = productList.Code;
            product.MRP = (decimal)productList.MRP;
            product.Unit = productList.Unit;
            product.DistributorPrice = (decimal)productList.DP;
            product.StokishPrice = (decimal)productList.SP;
            product.Vat = (decimal)productList.Vat;
            product.WalletPrice = (decimal)productList.WP;
            product.PurchasePrice = (decimal)productList.PP;
            product.RB = (int)productList.RB;
            product.BP = (int)productList.BP;
            product.Quantity = (int)productList.Quantity;
            product.ProductDetails = productList.ProductDetails;
            product.CreateDate = (DateTime)productList.CreateDate;
            product.CreateBy = productList.CreateBy;
            product.ModificationDate = (DateTime)productList.ModificationDate;
            product.ModifiedBy = productList.ModifiedBy;

            return View(product);
        }

        [HttpPost]
        public ActionResult ProductEdit(ProductVModel model)
        {
            ViewBag.PageTitle = "Product MANAGEMENT";

            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["ProductEdit"] = "Product Edit";
            ViewBag.PanelTitles = PanelTitles;

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Product Updated Failed.";
                return View(model);
            }
            else
            {
                using (KDMDB db = new KDMDB())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            tbl_products_master product = new tbl_products_master();
                            product = db.tbl_products_master.FirstOrDefault(x => x.ProductID == model.ProductID);
                            product.ProductName = model.ProductName;
                            product.Category = model.Category;
                            product.Type = model.Type;
                            product.Code = model.Code;
                            product.MRP = model.MRP;
                            product.Unit = model.Unit;
                            product.DP = model.DistributorPrice;
                            product.SP = model.StokishPrice;
                            product.Vat = model.Vat;
                            product.WP = model.WalletPrice;
                            product.PP = model.PurchasePrice;
                            product.RB = model.RB;
                            product.BP = model.BP;
                            product.Quantity = model.Quantity;
                            product.ProductDetails = model.ProductDetails;
                            product.CreateDate = model.CreateDate;
                            product.CreateBy = model.CreateBy;
                            product.ModificationDate = DateTime.Now; 
                            product.ModifiedBy = User.Identity.Name; 
                            db.Entry(product).State = EntityState.Modified;

                            tbl_products_history pHistory = new tbl_products_history();
                            pHistory.ProductID = product.ProductID;
                            pHistory.ProductName = model.ProductName;
                            pHistory.Category = model.Category;
                            pHistory.Type = model.Type;
                            pHistory.Code = model.Code;
                            pHistory.MRP = model.MRP;
                            pHistory.Unit = model.Unit;
                            pHistory.DistributorPrice = model.DistributorPrice;
                            pHistory.StokishPrice = model.StokishPrice;
                            pHistory.Vat = model.Vat;
                            pHistory.WalletPrice = model.WalletPrice;
                            pHistory.PurchasePrice = model.PurchasePrice;
                            pHistory.RB = model.RB;
                            pHistory.BP = model.BP;
                            pHistory.Quantity = model.Quantity;
                            pHistory.ProductDetails = model.ProductDetails;
                            pHistory.CreateBy = User.Identity.Name;
                            pHistory.CreateDate = DateTime.Now;
                            pHistory.Action = "Update";
                            db.tbl_products_history.Add(pHistory);
                            db.SaveChanges();
                            transaction.Commit();
                            ModelState.Clear();
                            ViewBag.SuccessMessage = "Product Updated Successfully.";
                            return View(model);
                        }
                        catch
                        {
                            transaction.Rollback();
                            ViewBag.ErrorMessage = "Product Updated Failed.";
                            return View(model);
                        }
                    }
                }

            }
        }

        [HttpGet]
        public ActionResult ProductDelete(int productId)
        {
            using (KDMDB db = new KDMDB())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var productExist = db.tbl_products_master.FirstOrDefault(x => x.ProductID == productId);
                        db.tbl_products_master.Remove(productExist);

                        tbl_products_history pHistory = new tbl_products_history();
                        pHistory.ProductID = productExist.ProductID;
                        pHistory.ProductName = productExist.ProductName;
                        pHistory.Category = productExist.Category;
                        pHistory.Type = productExist.Type;
                        pHistory.Code = productExist.Code;
                        pHistory.MRP = productExist.MRP;
                        pHistory.Unit = productExist.Unit;
                        pHistory.DistributorPrice = productExist.DP;
                        pHistory.StokishPrice = productExist.SP;
                        pHistory.Vat = productExist.Vat;
                        pHistory.WalletPrice = productExist.WP;
                        pHistory.PurchasePrice = productExist.PP;
                        pHistory.RB = productExist.RB;
                        pHistory.BP = productExist.BP;
                        pHistory.Quantity = productExist.Quantity;
                        pHistory.ProductDetails = productExist.ProductDetails;
                        pHistory.CreateBy = productExist.CreateBy;
                        pHistory.CreateDate = productExist.CreateDate;
                        pHistory.Action = "Delete";
                        db.tbl_products_history.Add(pHistory);
                        db.SaveChanges();
                        transaction.Commit();
                        ModelState.Clear();
                        ViewBag.SuccessMessage = "Product Removed Successfully.";
                        return RedirectToAction("ProductList");
                    }
                    catch
                    {
                        transaction.Rollback();
                        ViewBag.ErrorMessage = "Product Updated Failed.";
                        return RedirectToAction("ProductList");
                    }
                }
            }
        }
        public ActionResult ProductDetails(int productId)
        {
            ViewBag.PageTitle = "Product MANAGEMENT";
            var productList = db.tbl_products_master.FirstOrDefault(x => x.ProductID == productId);

            ProductVModel product = new ProductVModel();
            product.ProductID = productList.ProductID;
            product.ProductName = productList.ProductName;
            product.Category = (int)productList.Category;
            product.CategoryName = db.tbl_product_category.FirstOrDefault(y => y.Code == product.Category.ToString()).Name;
            product.Type = (int)productList.Type;
            product.TypeName = db.tbl_product_type.FirstOrDefault(y => y.Id == product.Type).Name;
            product.Code = productList.Code;
            product.MRP = (decimal)productList.MRP;
            product.Unit = productList.Unit;
            product.DistributorPrice = (decimal)productList.DP;
            product.StokishPrice = (decimal)productList.SP;
            product.Vat = (decimal)productList.Vat;
            product.WalletPrice = (decimal)productList.WP;
            product.PurchasePrice = (decimal)productList.PP;
            product.RB = (int)productList.RB;
            product.BP = (int)productList.BP;
            product.Quantity = (int)productList.Quantity;
            product.ProductDetails = productList.ProductDetails;
            product.CreateDate = (DateTime)productList.CreateDate;
            product.CreateBy = productList.CreateBy;
            product.ModificationDate = (DateTime)productList.ModificationDate;
            product.ModifiedBy = productList.ModifiedBy;

            return View(product);
        }

        [HttpGet]

        public ActionResult AddProductQuantity()
        {
            ViewBag.PageTitle = "Product MANAGEMENT";
            //db.tbl_products_quantity_update_history 
            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddProductQuantity"] = "Add Product Quantity";
            ViewBag.PanelTitles = PanelTitles;
            return View();
        }

        [HttpGet]
        public ActionResult GetStockQty(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                var productExist = db.tbl_products_master.FirstOrDefault(x => x.Code == code.Trim());
                if (productExist != null)
                {
                    return Json(productExist.Quantity, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(2, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddProductQuantity(ProductVModel model)
        {
            ViewBag.PageTitle = "Product MANAGEMENT";
            //db.tbl_products_quantity_update_history 
            Dictionary<string, string> PanelTitles = new Dictionary<string, string>();
            PanelTitles["AddProductQuantity"] = "Add Product Quantity";
            ViewBag.PanelTitles = PanelTitles;
            
            using (KDMDB db = new KDMDB())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        tbl_products_master productExist = new tbl_products_master();
                        productExist = db.tbl_products_master.FirstOrDefault(x => x.Code == model.Code);
                        productExist.Quantity = model.InStock + model.Quantity;
                        db.Entry(productExist).State = EntityState.Modified;

                        tbl_products_history pHistory = new tbl_products_history();
                        pHistory.ProductID = productExist.ProductID;
                        pHistory.ProductName = productExist.ProductName;
                        pHistory.Category = productExist.Category;
                        pHistory.Type = productExist.Type;
                        pHistory.Code = productExist.Code;
                        pHistory.MRP = productExist.MRP;
                        pHistory.Unit = productExist.Unit;
                        pHistory.DistributorPrice = (decimal)productExist.DP;
                        pHistory.StokishPrice = (decimal)productExist.SP;
                        pHistory.Vat = (decimal)productExist.Vat;
                        pHistory.WalletPrice = productExist.WP;
                        pHistory.PurchasePrice = productExist.PP;
                        pHistory.RB = productExist.RB;
                        pHistory.BP = productExist.BP;
                        pHistory.Quantity = productExist.Quantity;
                        pHistory.ProductDetails = productExist.ProductDetails;
                        pHistory.CreateBy = productExist.CreateBy;
                        pHistory.CreateDate = productExist.CreateDate;
                        pHistory.Action = "Qty Update";
                        db.tbl_products_history.Add(pHistory);
                        db.SaveChanges();
                        transaction.Commit();
                        ModelState.Clear();
                        ViewBag.SuccessMessage = "Product Quantity Added Successfully.";
                        return View();
                    }
                    catch
                    {
                        transaction.Rollback();
                        ViewBag.ErrorMessage = "Product Quantity Added Failed.";
                        return View(model);
                    }
                }
            }
        }

        public ActionResult CategoryList()
        {
            var model = db.tbl_product_category.ToList();            
            return View(model);
        }

        public ActionResult CategoryEdit(string Code)
        {
            var category = db.tbl_product_category.Where(x => x.Code == Code).FirstOrDefault();
            return View(category);
        }
        [HttpPost]
        public ActionResult CategoryEdit(tbl_product_category model)
        {
            var category= db.tbl_product_category.Where(x => x.Code == model.Code).FirstOrDefault();
            category.Name = model.Name;
            db.SaveChanges();
            return RedirectToAction("CategoryList");
        }

        public ActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCategory(tbl_product_category model)
        {
            // var item = db.tbl_product_category.Max(Convert.ToInt32(y=>y.Code));
            //    var itemsMax = items.Where(x => x.Height == items.Max(y => y.Height));
            // int lastCode=db.tbl_product_category.
           //int item = 13;
           // tbl_product_category tbl = new tbl_product_category();
           // tbl.Code = (int)(item);
           // tbl.Name = model.Name;
           // db.tbl_product_category.Add(tbl);
            return RedirectToAction("CategoryList");
        }
    }
}