using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KDM.Models
{
    public enum ERPStatus
    {
        ACTIVE,
        INACTIVE,
        PENDING,
        CLOSED,
        SALARY,
        SALARY_PAYMENT_COMPLETED
    }
    public class UserRoutePermissions
    {
        public Int64 ID { get; set; }
        public List<string> Areas { get; set; }
        public string DefaultArea { get; set; }
        public List<string> Controllers { get; set; }
        public string DefaultController { get; set; }
        public List<string> Actions { get; set; }
        public string DefaultAction { get; set; }
    }

    public class UserRoute
    {
        public string Area { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public bool IsActive { get; set; } = true;
        public List<string> Parameters { get; set; }
    }

    public class ReturnMessage
    {
        public string Type { get; set; }
        public string Message { get; set; }
    }

    public class Error
    {
        public Error(string key, string message)
        {
            Key = key;
            Message = message;
        }

        public string Key { get; set; }
        public string Message { get; set; }
    }

    public class MenuItem
    {
        public Int64 Id { get; set; }
        public Int64? Category { get; set; }
        public Int64? Parent { get; set; }
        public string Module { get; set; }
        public string Area { get; set; }
        public string AreaText { get; set; }

        public string Controller { get; set; }
        public string ControllerText { get; set; }

        public string Action { get; set; }
        public string ActionText { get; set; }
        public int? ItemOrder { get; set; }

        public List<MenuItem> ChildItems { get; set; }
    }

    public class Test
    {
        
        public DateTime time { get; set; }
    }

    public class ActionTableModel
    {   [Required]
        public string Controller { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Text { get; set; }
    }
    public class ControllerTableModel
    {
        [Required]
        public string Module { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Text { get; set; }
    }
}