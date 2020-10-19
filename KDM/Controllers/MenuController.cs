using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Models;
using Org.BouncyCastle.Crypto.Generators;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class MenuController : Controller
    {
        KDMDB db = new KDMDB();
        
        #region Create Category
        public ActionResult CreateCategory()
        {
            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (TempData["WarningMessage"] != null)
                ViewBag.WarningMessage = TempData["WarningMessage"];

            ViewBag.CategoryList = db.tbl_menu_category.Select(x => new MenuCategory()
            {
                Id = x.Id,
                CategoryName = x.CategoryName
            }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCategory(MenuCategory model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Please check your inputs";
                ViewBag.CategoryList = db.tbl_menu_category.Select(x => new MenuCategory()
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName
                }).ToList();
                return View(model);
            }

            var menuExists = db.tbl_menu_category.Where(x => x.CategoryName.ToLower() == model.CategoryName.ToLower()).Select(x => x).FirstOrDefault();
            if (menuExists != null)
            {
                ViewBag.ErrorMessage = "Menu category name already exists";
                ViewBag.CategoryList = db.tbl_menu_category.Select(x => new MenuCategory()
                {
                    Id = x.Id,
                    CategoryName = x.CategoryName
                }).ToList();
                return View(model);
            }

            db.tbl_menu_category.Add(new tbl_menu_category()
            {
                CategoryName = model.CategoryName
            });
            db.SaveChanges();

            TempData["SuccessMessage"] = "Menu category create successfull";

            ModelState.Clear();
            return RedirectToAction("CreateCategory", "Menu");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteMenuCategory(Int64 id)
        {
            var rw = db.tbl_menu_category.Find(id);
            if (rw != null)
            {
                db.tbl_menu_category.Remove(rw);
                db.SaveChanges();
            }

            TempData["SuccessMessage"] = "Successfully deleted";
            return RedirectToAction("CreateCategory", "Menu");

        }
        #endregion

        #region Category Action Mappings

        public ActionResult ListActionMappings()
        {
            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];
            if (TempData["ErrorMessage"] != null)
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            if (TempData["WarningMessage"] != null)
                ViewBag.WarningMessage = TempData["WarningMessage"];

        
            var model = db.tbl_RoleActionMappings.Select(x => x).ToList();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCategoryActionMappings(Int64 id, Int64 category, bool isActive)
        {
            if(id>-1 && category>-1)
            {
                var data = db.tbl_RoleActionMappings.Where(x => x.ID == id).Select(x => x).First();
                data.Category = category;
                data.IsActive = isActive;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Category updated";
            }
            else
            {
                TempData["ErrorMessage"] = "Can't update category";
            }

            return RedirectToAction("ListActionMappings");
        }
        #endregion

        #region Create Menu item
        public ActionResult ListMenuItem()
        {
            
            return View(db.tbl_menu_category.ToList());
        }
        public ActionResult CreateMenuCategory()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateMenuCategory(MenuCreate model)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                tbl_menu_category tbl = new tbl_menu_category();
                tbl.MenuOrder = model.MenuOrder;
                tbl.CategoryName = model.CategoryName;
                db.tbl_menu_category.Add(tbl);
                db.SaveChanges();
            }
            return RedirectToAction("ListMenuItem", "Menu");
        }
        #endregion
    }
}