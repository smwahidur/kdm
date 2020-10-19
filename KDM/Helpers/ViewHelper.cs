using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KDM.Helpers;
using KDM.Models;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace KDM.Helpers
{
    public class ViewHelper
    {
        public List<MenuItem> menuItems = new List<MenuItem>();

        public List<SelectListItem> AllRolesToSelectListItem()
        {
            List<SelectListItem> roleItems = new List<SelectListItem>();
            IdentityManager im = new IdentityManager();
            List<IdentityRole> roles = im.GetAllRoles();

            foreach (var role in roles)
            {
                SelectListItem newItem = new SelectListItem();
                newItem.Value = role.Id;
                newItem.Text = role.Name;
                newItem.Selected = false;

                roleItems.Add(newItem);
            }

            return roleItems;

        }

        public static List<SelectListItem> LoadPhoneTypes()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_phone_types.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }

        }

        public static string GetPhoneType(string value)
        {
            using (KDMDB db = new KDMDB())
            {
                var item = db.tbl_phone_types.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();
                
                return item;
            }
        }

        public static List<SelectListItem> LoadAddressTypes()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_address_types.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }


        }

        public static string GetAddressType(string value)
        {
            using (KDMDB db = new KDMDB())
            {
                var item = db.tbl_address_types.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

                return item;
            }
        }

        public static List<SelectListItem> LoadDivisions()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_division.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }


        }

        public static string GetDivision(string value)
        {
            using (KDMDB db = new KDMDB())
            {
                var item = db.tbl_division.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

                return item;
            }
        }

        public static List<SelectListItem> LoadDistricts()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_district.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }
        }

        public static List<SelectListItem> LoadThana()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_district.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }
        }


        public static List<SelectListItem> LoadUnion()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_district.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }
        }

        

        public static string L(string value)
        {
            using (KDMDB db = new KDMDB())
            {
                var item = db.tbl_district.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

                return item;
            }
        }
        
        //public static List<SelectListItem> LoadPoliceStations()
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var items = db.tbl_police_station.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }
        //}

        //public static string GetPoliceStation(string value)
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var item = db.tbl_police_station.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

        //        return item;
        //    }
        //}

        public static List<SelectListItem> LoadCountry()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_country.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }
        }

        public static string GetCountry(string value)
        {
            using (KDMDB db = new KDMDB())
            {
                var item = db.tbl_country.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

                return item;
            }
        }

        public static List<SelectListItem> LoadRoles()
        {
            IdentityManager im = new IdentityManager();
            var items = im.GetAllRoles();

            List<SelectListItem> selectItems = new List<SelectListItem>();

            foreach(var item in items)
            {
                selectItems.Add(new SelectListItem()
                {
                    Value = item.Name,
                    Text = item.Name
                });
            }
            return selectItems;

        }
        
        public static List<SelectListItem> LoadMenuCategories()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_menu_category.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Id.ToString(),
                        Text = item.CategoryName
                    });
                }
                return selectItems;
            }
        }

        public static List<SelectListItem> LoadModules()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_modules.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Text,
                        Text = item.Value
                    });
                }
                return selectItems;
            }
        }

        public static List<SelectListItem> LoadControllers()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_controllers.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }
        }

        public static List<SelectListItem> LoadActions()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_actions.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Value,
                        Text = item.Text
                    });
                }
                return selectItems;
            }
        }

        //public static List<SelectListItem> LoadCity()
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var items = db.tbl_city.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }
        //}

        //public static string GetCity(string value)
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var item = db.tbl_city.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

        //        return item;
        //    }
        //}

        //public static List<SelectListItem> LoadExams()
        //{
        //    using (ERPSolution.Areas.HRPayroll.Models.HRPayrollDB db = new ERPSolution.Areas.HRPayroll.Models.HRPayrollDB())
        //    {
        //        var items = db.tbl_exams.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }
        //}

        //public static string GetExam(string value)
        //{
        //    using (ERPSolution.Areas.HRPayroll.Models.HRPayrollDB db = new ERPSolution.Areas.HRPayroll.Models.HRPayrollDB())
        //    {
        //        var item = db.tbl_exams.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

        //        return item;
        //    }
        //}

        //public static List<SelectListItem> LoadDesignations()
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var items = db.tbl_designation.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }

        //}

        //public static string GetDesignation(string value)
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var item = db.tbl_designation.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

        //        return item;
        //    }
        //}

        //public static List<SelectListItem> LoadDepartments()
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var items = db.tbl_departments.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }

        //}

        //public static string GetDepartment(string value)
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var item = db.tbl_departments.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();
        //        return item;
        //    }
        //}

        //public static List<SelectListItem> LoadSections()
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var items = db.tbl_sections.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }

        //}

        //public static string GetSection(string value)
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var item = db.tbl_sections.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();
        //        return item;
        //    }
        //}

        public static List<SelectListItem> LoadBoards()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem() {
                Value="DHAKA",
                Text="DHAKA"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "RAJSHAHI",
                Text = "RAJSHAHI"
            });
            selectItems.Add(new SelectListItem()
            {
                Value = "KHULNA",
                Text = "KHULNA"
            });

            return selectItems;
        }
        
        public static List<SelectListItem> LoadTrainingStatus()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem()
            {
                Value = "ACTIVE",
                Text = "ACTIVE"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "PASSED",
                Text = "PASSED"
            });
            selectItems.Add(new SelectListItem()
            {
                Value = "FAILED",
                Text = "FAILED"
            });

            return selectItems;
        }
        
        public static List<SelectListItem> LoadJobStatus()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem()
            {
                Value = "ACTIVE",
                Text = "ACTIVE"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "INACTIVE",
                Text = "INACTIVE"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "PENDING",
                Text = "PENDING"

            });
            return selectItems;
        }

        public static List<SelectListItem> LoadGroupNames()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem()
            {
                Value = "SCIENCE",
                Text = "SCIENCE"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "ARTS",
                Text = "ARTS"
            });
            selectItems.Add(new SelectListItem()
            {
                Value = "COMMERCE",
                Text = "COMMERCE"
            });

            return selectItems;
        }

        public static List<SelectListItem> LoadProduct()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem()
            {
                Value = "1",
                Text = "Product 1"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "2",
                Text = "Product 2"
            });
            selectItems.Add(new SelectListItem()
            {
                Value = "3",
                Text = "Product 3"
            });

            return selectItems;
        }

        public static List<SelectListItem> LoadGenders()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem()
            {
                Value = "Male",
                Text = "Male"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "Female",
                Text = "Female"
            });
            selectItems.Add(new SelectListItem()
            {
                Value = "Third",
                Text = "Third"
            });

            return selectItems;
        }

        public static List<SelectListItem> LoadMaritialStatus()
        {
            List<SelectListItem> selectItems = new List<SelectListItem>();
            selectItems.Add(new SelectListItem()
            {
                Value = "Married",
                Text = "Married"
            });

            selectItems.Add(new SelectListItem()
            {
                Value = "UnMarried",
                Text = "UnMarried"
            });

            return selectItems;
        }


        //public static List<SelectListItem> LoadProductType()
        //{
        //    List<SelectListItem> selectItems = new List<SelectListItem>();
        //    selectItems.Add(new SelectListItem()
        //    {
        //        Value = "1",
        //        Text = "Package"
        //    });

        //    selectItems.Add(new SelectListItem()
        //    {
        //        Value = "2",
        //        Text = "Product"
        //    });
        //    selectItems.Add(new SelectListItem()
        //    {
        //        Value = "3",
        //        Text = "Wallet"
        //    });

        //    return selectItems;
        //}

        public static List<SelectListItem> LoadDealer()
        {
          
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_dealers.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.DealerID.ToString(),
                        Text = item.Name
                    }) ;
                }
                return selectItems;
            }
        }


        public static List<SelectListItem> LoadDealerType()
        {
            using (KDMDB db = new KDMDB())
            {
                var items = db.tbl_dealer_type.Select(x => x).ToList();
                List<SelectListItem> selectItems = new List<SelectListItem>();

                foreach (var item in items)
                {
                    selectItems.Add(new SelectListItem()
                    {
                        Value = item.Id.ToString(),
                        Text = item.Name
                    });
                }
                return selectItems;
            }
        }
        //public static List<SelectListItem> LoadReligions()
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var items = db.tbl_religions.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.Value,
        //                Text = item.Text
        //            });
        //        }
        //        return selectItems;
        //    }

        //}

        //public static string GetReligion(string value)
        //{
        //    using (KDMDB db = new KDMDB())
        //    {
        //        var item = db.tbl_religions.Where(x => x.Value == value).Select(x => x.Text).FirstOrDefault();

        //        return item;
        //    }
        //}

        //public static List<SelectListItem> LoadShifts()
        //{
        //    using (ERPSolution.Areas.HRPayroll.Models.HRPayrollDB db = new ERPSolution.Areas.HRPayroll.Models.HRPayrollDB())
        //    {
        //        var items = db.hr_liv_sft_conf_shift_time.Select(x => x).ToList();
        //        List<SelectListItem> selectItems = new List<SelectListItem>();

        //        foreach (var item in items)
        //        {
        //            selectItems.Add(new SelectListItem()
        //            {
        //                Value = item.ID.ToString(),
        //                Text = item.ShiftCode
        //            });
        //        }
        //        return selectItems;
        //    }

        //}

        //public static string GetShift(int value)
        //{
        //    using (ERPSolution.Areas.HRPayroll.Models.HRPayrollDB db = new ERPSolution.Areas.HRPayroll.Models.HRPayrollDB())
        //    {
        //        var item = db.hr_liv_sft_conf_shift_time.Where(x => x.ID == value).Select(x => x.ShiftCode).FirstOrDefault();

        //        return item;
        //    }
        //}

        public static List<MenuCategory> LoadMenu()
        {
            List<MenuItem> items = new List<MenuItem>();
            string userId = HttpContext.Current.User.Identity.GetUserId();
            IdentityManager im = new IdentityManager();
            List<string> roles = im.GetUsersRole(userId);
            List<MenuItem> selectedMenuItems = new List<MenuItem>();
            List<MenuCategory> categories = new List<MenuCategory>();

            using (KDMDB db = new KDMDB())
            {
                 items = (from rw in db.tbl_RoleActionMappings
                          where roles.Contains(rw.Role) && rw.IsActive==true
                             select new MenuItem {
                                 Id=rw.ID,
                                 Category=rw.Category,
                                 Parent=rw.Parent,
                                 Area=rw.Area,
                                 AreaText=rw.AreaDisplayText,
                                 Controller=rw.Controller,
                                 ControllerText=rw.ControllerDisplayText,
                                 Action=rw.Action,
                                 ActionText=rw.ActionDisplayText,
                                 ItemOrder=rw.ItemOrder
                             }).ToList();

                categories = db.tbl_menu_category.OrderBy(x => x.MenuOrder).Select(x => new MenuCategory()
                {

                    Id=x.Id,
                    CategoryName=x.CategoryName

                }).ToList();


                if (categories.Count > 0)
                {
                    foreach(var category in categories)
                    {
                        var categoryChildItems = items.Where(x => x.Category == category.Id).Select(x => x).ToList();

                        if (categoryChildItems.Count > 0)
                        {
                            category.ChildItems = categoryChildItems;
                        }
                    }
                }
                
            }
            
            return categories;
        }

        public static string GetMonthName(int month)
        {
            if (month > 0)
                return System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(month);
            else
                return "";
        }


        public static List<SelectListItem> LoadProductCategory()
        {
            KDMDB db = new KDMDB();
            var categoryList = db.tbl_product_category.ToList();
            List<SelectListItem> selectItems = new List<SelectListItem>();
            categoryList.ForEach(x =>
            {
                selectItems.Add(new SelectListItem()
                {
                    Value = x.Code.ToString(),
                    Text = x.Name
                });
            });
            return selectItems;
        }

        public static List<SelectListItem> LoadProductType()
        {
            KDMDB db = new KDMDB();
            var typeList = db.tbl_product_type.ToList();
            List<SelectListItem> selectItems = new List<SelectListItem>();
            typeList.ForEach(x =>
            {
                selectItems.Add(new SelectListItem()
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
            });
            return selectItems;
        }

        public static List<SelectListItem> LoadProductFromDB()
        {
            KDMDB db = new KDMDB();
            var typeList = db.tbl_products_master.ToList();
            List<SelectListItem> selectItems = new List<SelectListItem>();
            typeList.ForEach(x =>
            {
                selectItems.Add(new SelectListItem()
                {
                    Value = x.Code.ToString(),
                    Text = x.ProductName
                });
            });
            return selectItems;
        }
    }

    public static class Messages
    {
        public static string CheckInput { get; } = "Check your input";
        public static string UpdateSuccessfull { get; } = "Update successfull";
    }
    
}