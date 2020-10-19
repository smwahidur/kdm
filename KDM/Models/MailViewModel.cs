using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace KDM.Models
{
    public class CustomMailFormat
    {
        [Required]
        [Display(Name = "To")]
        public string To { get; set; }

        public List<string> ToAddresses { get; set; }

        [Required]
        [Display(Name = "From")]
        public string From { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }
    }

    public class SMTPConfig
    {
        public static string UserName { get; set; } = "notification@ndevlab.com";
        public static string Password { get; set; } = "Zjwi#530";
        public static string Host { get; set; } = "mail.ndevlab.com";
        public static int Port { get; set; } = 25;
        public static bool SSL { get; set; } = false;
        public static bool UseDefaultCredentials { get; set; } = true;
    }

    public class MailConfig
    {
        public static string NotificationMailAddress { get; set; } = "notification@ndevlab.com";
        
    }
}