using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Web.Profile;
using Membership = System.Web.Security.Membership;

namespace StarterSite.Web.Models
{
    public class UserProfile : ProfileBase
    {
        public string FirstName
        {
            get { return this["FirstName"].ToString(); }
            set { this["FirstName"] = value; }
        }

        public string LastName
        {
            get { return this["LastName"].ToString(); }
            set { this["LastName"] = value; }
        }

        public static UserProfile GetUserProfile(string username)
        {
            return Create(username) as UserProfile;
        }

        public static UserProfile GetUserProfile()
        {
            return Create(Membership.GetUser().UserName) as UserProfile;
        }
    }

    public class UserModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [ReadOnly(true)]
        public DateTime CreationDate { get; set; }

        [ReadOnly(true)]
        public DateTime LastLoginDate { get; set; }

        public bool IsApproved { get; set; }

        public string RoleList
        {
            get
            {
                var roles = string.Empty;
                for (var index = 0; index < Roles.Count; index++)
                {
                    roles += Roles[index];
                    if (index != Roles.Count - 1)
                        roles += ",";
                }
                return roles;
            }
        }

        public List<string> Roles { get; set; }
    }

    public class AssignedRole
    {
        public string RoleName { get; set; }
        public bool Assigned { get; set; }
    }

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName{ get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName{get;set;}

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
