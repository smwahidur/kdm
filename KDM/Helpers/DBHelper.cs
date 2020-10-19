using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KDM.Models;
using Newtonsoft.Json;

namespace KDM.Helpers
{
    public class DBHelper
    {
        
        public bool HasRoutePermission(string role, string area, string controller, string action, List<string> parameters = null)
        {
            bool IsPermitted = false;

            using (KDMDB db = new KDMDB())
            {
                var routeDetails = db.tbl_RoleActionMappings
                    .Where(x => x.Area == area && x.Controller == controller && x.Action == action && x.IsActive == true)
                    .Select(x => x).FirstOrDefault();

                if (routeDetails != null)
                {
                    Int64 routeId = routeDetails.ID;

                    var roleRouteMapDetails = db.tbl_RoleActions
                        .Where(x => x.Role == role)
                        .Select(x => x).FirstOrDefault();
                    List<Int64> actionIds = JsonConvert.DeserializeObject<List<Int64>>(roleRouteMapDetails.Actions);
                    if (actionIds.Contains(routeId))
                    {
                        IsPermitted = true;
                    }
                }
            }

            return IsPermitted;
        }

        public void CreateRoute(string role, UserRoute newRoute)
        {
            using (KDMDB db = new KDMDB())
            {
                using (var tr = db.Database.BeginTransaction())
                {
                    try
                    {
                        tbl_RoleActionMappings tbl = new tbl_RoleActionMappings();
                        tbl.Area = newRoute.Area;
                        tbl.Controller = newRoute.Controller;
                        tbl.Action = newRoute.Action;
                        tbl.IsActive = newRoute.IsActive;

                        db.tbl_RoleActionMappings.Add(tbl);
                        db.SaveChanges();

                        Int64 newRouteId = tbl.ID;

                        tbl_RoleActions roleAction = db.tbl_RoleActions
                            .Where(p =>p.Role == role)
                            .Select(t => t).FirstOrDefault();
                        List<Int64> actionIds = JsonConvert.DeserializeObject<List<Int64>>(roleAction.Actions);
                        actionIds.Add(newRouteId);


                    }
                    catch
                    {
                        tr.Rollback();
                    }
                    finally
                    {
                        tr.Commit();

                    }
                }
            }
        }

        public UserRoute GetDefaultRoute(string role)
        {
            UserRoute ur = null;
            using (KDMDB db = new KDMDB())
            {
                var defaultRouteId = db.tbl_RoleActions.Where(x => x.Role == role).Select(x => x.DefaultAction).FirstOrDefault();

                if (defaultRouteId != null)
                {
                    var routeDetails = db.tbl_RoleActionMappings.Where(
                                          x => 
                                          x.ID == defaultRouteId
                                          && x.IsActive == true).Select(x => x).FirstOrDefault();

                    if (routeDetails != null)
                    {
                        ur = new UserRoute(){
                            Area = routeDetails.Area
                            ,
                            Controller = routeDetails.Controller
                            ,
                            Action = routeDetails.Action
                            ,
                            Parameters = JsonConvert.DeserializeObject<List<string>>(routeDetails.Parameters)
                        };
                    }
                }

            }

            return ur;
        }

        //public static string GetNewEmployeeId()
        //{
        //    string id = "";
        //    using (ERPSolution.Areas.HRPayroll.Models.HRPayrollDB db = new ERPSolution.Areas.HRPayroll.Models.HRPayrollDB())
        //    {
        //        id = db.tbl_employees.Where(x => x.ID == db.tbl_employees.Max(m => m.ID)).Select(e => e.EmployeeId).FirstOrDefault();
        //        id = (Convert.ToInt64(id) + 1).ToString();
        //    }
        //    return id;
        //}
    }
}