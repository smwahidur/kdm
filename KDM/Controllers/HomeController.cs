using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using Microsoft.Reporting.WebForms;
using KDM.Helpers;
using Microsoft.AspNet.Identity;
using KDM.Models;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using KDM.Filters;

namespace KDM.Controllers
{
    //[KDMActionFilter]
    public class HomeController : Controller
    {
        KDMDB db = new KDMDB();

        // GET: Home
        //public ActionResult Index()
        //{
        //    DataSet ds = new DataSet();
        //    DataTable table = new DataTable("DataTable");
        //    DataColumn col1 = new DataColumn("EmployeeId");
        //    DataColumn col2 = new DataColumn("FirstName");
        //    table.Columns.Add(col1);
        //    table.Columns.Add(col2);

        //    var data = db.tbl_employees.Select(x => x).ToList();
        //    foreach(var rw in data)
        //    {
        //        table.Rows.Add(rw.EmployeeId, rw.FirstName);
        //    }

        //    ds.Tables.Add(table);
        //    Session["ReportDataSet"] = ds;
        //    Session["Report"] = "~/Reports/Employee/Report1.rdlc";
        //    //return View();

        //    return Redirect("~/Reports/ReportViewer.aspx");
        //}

        [AllowAnonymous]
        public ActionResult Index()
        {
            ControllerHelper cHelper = new ControllerHelper();
            IdentityManager im = new IdentityManager();
            string userId = User.Identity.GetUserId();
            if (!im.IsChangePassword(userId))
            {
                if (userId != null)
                {
                    List<string> roles = im.GetUsersRole(userId);
                    if (roles.Contains(KDMEnvironmentConstants.DefaultUserRole))
                    {
                        return RedirectToAction("Dashboard", "KDMMember", new { Area = "" });
                    }

                    UserRoute userRoute = cHelper.GetDefaultRoute(userId);
                    if (userRoute == null)
                    {
                        return RedirectToAction("Index", "Default", new { Area = "" });
                    }
                    if (!String.IsNullOrWhiteSpace(userRoute.Controller) && !String.IsNullOrWhiteSpace(userRoute.Action))
                        return RedirectToAction(userRoute.Action, userRoute.Controller);
                    else
                        return RedirectToAction("Index", "Default", new { Area = "" });
                }
                else
                {
                    return View();
                }
            }
            else
            {
                return View();
            }
           
        }

        [NonAction]
        public ActionResult Test()
        {
            IdentityManager im = new IdentityManager();
            string userId = im.GetUserIdByName("admin");
            im.DirectResetPassword(userId, "Admin#1234");
            return View();
        }

        [NonAction]
        [HttpPost]
        public ActionResult Test(DateTime time)
        {

            string t = String.Format("{0:hh:mm}", time);


            // or save time variable 
            return View();
        }

        [NonAction]
        public FileResult PdfReport()
        {
            //ControllerContext context = ReportHelper.CreateController<HomeController>().ControllerContext;
            
            return ReportHelper.ResponseAsPDF(this.ControllerContext, "Test");
        }

        //public ActionResult GetRawResponse(string id)
        //{
        //    List<tbl_employees> model = new List<tbl_employees>();
        //    if (!String.IsNullOrWhiteSpace(id))
        //    {
        //        model = db.tbl_employees.Where(x => x.EmployeeId == id).Select(x => x).ToList();

        //    }
        //    ControllerContext context = ReportHelper.CreateController<EmployeeController>().ControllerContext;
        //    string html = ReportHelper.RenderViewToString(context, "ReportEmployeeDetails", model);
            
        //    return Content(html);
        //}

        [NonAction]
        public ActionResult KDMHelpTest()
        {
            KDMTRHelper kdmHelper = new KDMTRHelper();
            //kdmHelper.WalletBonus(10004);
            //kdmHelper.UpdateFundMonthlyRoyalityBonusByMemberSource(10004);
            kdmHelper.UpdateFundPerformanceBonusByMemberSource("10004");
            return View();
        }

    }
}