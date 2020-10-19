using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace KDM.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        
    }

    public class CreateNewUserViewModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name ="Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name ="Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage = "Confirm password doesn't match, Type again !")]
        [Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; }
        
    }

    public class ChangeMyPasswordViewModel
    {
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression("^((?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])|(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[^a-zA-Z0-9])|(?=.*?[A-Z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])|(?=.*?[a-z])(?=.*?[0-9])(?=.*?[^a-zA-Z0-9])).{8,}$", ErrorMessage = "Passwords must be at least 8 characters and contain at 3 of 4 of the following: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; }

    }

    public class UsersListViewModel
    {
        public string UserName { get; set; }
        
        public string Email { get; set; }
        
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }

    public class ProfileViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        
        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }
    }

    public class UpdateUserViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }
        
        public string Role { get; set; }

        public List<string> Roles { get; set; }
    }

    public class AssignRolesToUserViewModel
    {
        [Required]
        [Display(Name ="User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name ="Roles")]
        public List<System.Web.Mvc.SelectListItem> Roles { get; set; }

    }

    public class MapRoleModuleActionsViewModel
    {
        public Int64 Id { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string Module { get; set; }
        [Required]
        public string Controller { get; set; }
        [Required]
        public string Action { get; set; }

        public string Status { get; set; }
    }

}