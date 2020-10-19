using KDM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class MiscController:Controller
    {

        KDMDB Kdb = new KDMDB();
        public JsonResult LoadControllers(string id)
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_controllers.Where(x=>x.Module==id).Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return Json(selectItems, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult LoadActions(string id)
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_actions.Where(x => x.Controller == id).Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return Json(selectItems, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ListAction()
        {
            return View(Kdb.tbl_actions.OrderBy(o => o.Controller).ToList());
        }

        public ActionResult CreateAction()
        {   
            return View();
        }
        [HttpPost]
        public ActionResult CreateAction(ActionTableModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                tbl_actions tbl = new tbl_actions();
                tbl.Controller = model.Controller;
                tbl.Value = model.Value;
                tbl.Text = model.Text;
                Kdb.tbl_actions.Add(tbl);
                Kdb.SaveChanges();

            }
             
            return RedirectToAction("ListAction", "Misc");
        }

        public ActionResult ListofController()
        {
            return View(Kdb.tbl_controllers.OrderBy(o => o.Value).ToList());
        }

        public ActionResult CreateController()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateController(ControllerTableModel model)
        {
           
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                tbl_controllers tbl = new tbl_controllers();
                tbl.Module = model.Module;
                tbl.Value = model.Value;
                tbl.Text = model.Text;
                Kdb.tbl_controllers.Add(tbl);
                Kdb.SaveChanges();

            }
            return View();
        }
        [HttpPost]
        public ActionResult DeleteAction(IEnumerable<int> IDs)
        {
           
           
            foreach (var item in IDs)
            {
                var delete = Kdb.tbl_actions.FirstOrDefault(s => s.Id == item);
                if (delete != null)
                {
                    Kdb.tbl_actions.Remove(delete);
                }
            }
            Kdb.SaveChanges();
            return RedirectToAction("ListAction", "Misc");
        }

    }
}