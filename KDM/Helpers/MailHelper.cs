using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Web.Routing;

using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using KDM.Models;

namespace KDM.Helpers
{
    public class MailHelper
    {
        IdentityManager im = new IdentityManager();

        public List<SelectListItem> GetRoleList()
        {
            List<SelectListItem> dropDownListItem = new List<SelectListItem>();

            List<IdentityRole> roles = im.GetAllRoles();
            foreach (IdentityRole role in roles)
            {
                SelectListItem item = new SelectListItem();
                item.Value = role.Id;
                item.Text = role.Name;
                dropDownListItem.Add(item);
            }
            return dropDownListItem;
        }

        public List<KeyValuePair<string, string>> GetRoles()
        {
            List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
            List<IdentityRole> roles = im.GetAllRoles();
            foreach (IdentityRole role in roles)
            {
                items.Add(new KeyValuePair<string, string>(role.Id, role.Name));
            }
            return items;
        }



        //public static List<SelectListItem> GetDropDownItems(Dictionary<string, string> options)
        //{
        //    List<SelectListItem> items = new List<SelectListItem>();
        //    foreach (var )
        //    {

        //        employeeDropDownList = (from emp in db.tbl_employee
        //                                select new SelectListItem
        //                                {
        //                                    Value = emp.emp_id.ToString(),
        //                                    Text = emp.emp_name
        //                                }).ToList();
        //    }
        //    return employeeDropDownList;
        //}



        public static bool SendMessage(NotificationViewModel model)
        {
            bool send = false;

            using (KDMDB db = new KDMDB())
            {
                tbl_notification tbl = new tbl_notification();
                tbl.notf_title = model.Title;
                tbl.notf_from = model.From;
                tbl.notf_to = model.To;
                tbl.notf_message = model.Message;
                tbl.notf_time = DateTime.Now;
                tbl.notf_status = "send";
                db.tbl_notification.Add(tbl);
                db.SaveChanges();
                send = true;
            }
            return send;
        }

        public static List<NotificationViewModel> ReadMessage(string userId, string status)
        {
            //string userId=
            List<NotificationViewModel> msgs = new List<NotificationViewModel>();
            using (KDMDB db = new KDMDB())
            {
                msgs = (from item in db.tbl_notification
                        where item.notf_to == userId && item.notf_to == "send"
                        select new NotificationViewModel
                        {
                            Id = item.notf_id,
                            Type = item.notf_type,
                            Title = item.notf_title,
                            From = item.notf_from,
                            To = item.notf_to,
                            Message = item.notf_message,
                            SendTime = item.notf_time,
                            ReadTime = item.notf_read_time,
                            Status = item.notf_status

                        }).ToList();

            }
            return msgs;
        }

        public static bool DeleteMessage(Int64 msgId)
        {
            bool ret = false;
            using (KDMDB db = new KDMDB())
            {
                var msg = db.tbl_notification.Find(msgId);
                if (msg != null)
                {
                    db.tbl_notification.Remove(msg);
                    db.SaveChanges();
                    ret = true;
                }
            }
            return ret;
        }



        public static List<SelectListItem> GetYesNo()
        {
            List<SelectListItem> yesNo = new List<SelectListItem>();

            yesNo.Add(new SelectListItem { Value = "yes", Text = "Yes" });
            yesNo.Add(new SelectListItem { Value = "no", Text = "No" });

            return yesNo;
        }

        //------------------- email ------------------

        public static bool SendMail(CustomMailFormat mf)
        {
            bool ret = false;

            var message = new MailMessage();
            if (mf.ToAddresses.Count() > 0)
                foreach (var to in mf.ToAddresses)
                {
                    message.To.Add(new MailAddress(to));
                }
            else
                message.To.Add(new MailAddress(mf.To));

            // replace with valid value 
            message.From = new MailAddress(mf.From);  // replace with valid value
            message.Subject = mf.Subject;
            message.Body = mf.Message;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = SMTPConfig.UserName,  // replace with valid value
                    Password = SMTPConfig.Password  // replace with valid value
                };
                smtp.Credentials = credential;
                smtp.Host = SMTPConfig.Host;
                smtp.Port = SMTPConfig.Port;
                smtp.EnableSsl = false;
                try
                {
                    smtp.Send(message);
                }
                catch
                {
                    return false;
                }

                ret = true;
            }
            return ret;
        }

        public static bool SendMailByRole(List<string> roles, string subjectTag, string groupMessage)
        {
            bool ret = false;
            foreach (var role in roles)
            {
                CustomMailFormat mf = new CustomMailFormat();
                List<string> toAddresses = GetMailAddressesByRole(role);
                mf.Subject = "FAV [" + subjectTag + "]";
                mf.To = toAddresses.First();
                mf.ToAddresses = toAddresses;
                mf.Message = groupMessage;
                mf.From = MailConfig.NotificationMailAddress;
                if (toAddresses.Count > 0)
                    SendMail(mf);
            }

            return ret;
        }

        private static List<string> GetMailAddressesByRole(string roleName)
        {
            var roleManager =
             new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var users = roleManager.FindByName(roleName).Users;
            List<string> mailAddresses = new List<string>();
            foreach (var user in users)
            {
                ApplicationUserManager userManager = HttpContext.Current.GetOwinContext().Get<ApplicationUserManager>();
                var u = userManager.FindById(user.UserId);
                mailAddresses.Add(u.Email);
            }
            return mailAddresses;
        }
        //------------------- email end --------------

        public static string RenderPartialViewToString(ControllerContext context, string viewName, object model = null)
        {
            context.Controller.ViewData.Model = model;

            try
            {
                using (StringWriter sw = new StringWriter())
                {
                    ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                    ViewContext viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                    viewResult.View.Render(viewContext, sw);

                    return sw.GetStringBuilder().ToString();
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string RenderViewToString(ControllerContext context,
                                    string viewPath,
                                    object model = null,
                                    bool partial = false)
        {
            // first find the ViewEngine for this view
            ViewEngineResult viewEngineResult = null;
            if (partial)
                viewEngineResult = ViewEngines.Engines.FindPartialView(context, viewPath);
            else
                viewEngineResult = ViewEngines.Engines.FindView(context, viewPath, null);

            if (viewEngineResult == null)
                throw new FileNotFoundException("View cannot be found.");

            // get the view and attach the model to view data
            var view = viewEngineResult.View;
            context.Controller.ViewData.Model = model;

            string result = null;

            using (var sw = new StringWriter())
            {
                var ctx = new ViewContext(context, view,
                                            context.Controller.ViewData,
                                            context.Controller.TempData,
                                            sw);
                view.Render(ctx, sw);
                result = sw.ToString();
            }

            return result;
        }

        public static T CreateController<T>(RouteData routeData = null)
                where T : Controller, new()
        {
            T controller = new T();

            // Create an MVC Controller Context
            HttpContextBase wrapper = null;
            if (HttpContext.Current != null)
                wrapper = new HttpContextWrapper(System.Web.HttpContext.Current);
            //else
            //    wrapper = CreateHttpContextBase(writer);


            if (routeData == null)
                routeData = new RouteData();

            if (!routeData.Values.ContainsKey("controller") && !routeData.Values.ContainsKey("Controller"))
                routeData.Values.Add("controller", controller.GetType().Name
                                                            .ToLower()
                                                            .Replace("controller", ""));

            controller.ControllerContext = new ControllerContext(wrapper, routeData, controller);
            return controller;
        }

        public static DateTime GetBDTime()
        {
            TimeZoneInfo timeInfo = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, timeInfo).ToUniversalTime();
        }
    }

    

}