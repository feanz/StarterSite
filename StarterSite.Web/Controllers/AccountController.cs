using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using StarterSite.Web.Helpers;
using StarterSite.Web.Models;

namespace StarterSite.Web.Controllers
{
    [CustomAuthorize(Roles = "SuperUser")]
    public class AccountController : Controller
    {
        public ActionResult Index()
        {
            var members = Membership.GetAllUsers();

            var viewModel = new List<UserModel>();

            //Could really use automapper for this
            foreach (var m in members)
            {
                var member = m as MembershipUser;
                var user = new UserModel
                               {
                                   Email = member.Email,
                                   CreationDate = member.CreationDate,
                                   LastLoginDate = member.LastLoginDate,
                                   IsApproved = member.IsApproved
                               };

                viewModel.Add(user);
            }

            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult LogOn()
        {
            return ContextDependentView();
        }

        [HttpPost,AllowAnonymous]
        public JsonResult JsonLogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    return Json(new { success = true, redirect = returnUrl });
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        [HttpPost,AllowAnonymous]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return ContextDependentView();
        }

        [HttpPost, AllowAnonymous]
        public ActionResult JsonRegister(RegisterModel model)
        {
            //Use email as username
            model.UserName = model.Email;

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);

                    //Create profile
                    var profile = UserProfile.GetUserProfile(model.UserName);

                    profile.FirstName = model.FirstName;
                    profile.LastName = model.LastName;
                    profile.Save();

                    //Add to default role 
                    Roles.AddUserToRole(model.UserName, "User");

                    return Json(new { success = true });
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed
            return Json(new { errors = GetErrorsFromModelState() });
        }

        [HttpPost, AllowAnonymous]
        public ActionResult Register(RegisterModel model)
        {
            //Use email as username
            model.UserName = model.Email;

            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, null, null, true, null, out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);

                    //Create profile
                    var profile = UserProfile.GetUserProfile(model.UserName);

                    profile.FirstName = model.FirstName;
                    profile.LastName = model.LastName;
                    profile.Save();

                    //Add to default role 
                    Roles.AddUserToRole(model.UserName, "User");

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", ErrorCodeToString(createStatus));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded = false;
                try
                {
                    var currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    if (currentUser != null)
                        changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        private ActionResult ContextDependentView()
        {
            var actionName = ControllerContext.RouteData.GetRequiredString("action");
            if (Request.QueryString["content"] != null)
            {
                ViewBag.FormAction = "Json" + actionName;
                return PartialView();
            }
            ViewBag.FormAction = actionName;
            return View();
        }

        private IEnumerable<string> GetErrorsFromModelState()
        {
            return ModelState.SelectMany(x => x.Value.Errors
                .Select(error => error.ErrorMessage));
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion

        public ActionResult Edit(string email)
        {
            var member = Membership.GetUser(email);

            if (member != null)
            {
                var user = new UserModel
                               {
                                   Email = member.Email,
                                   CreationDate = member.CreationDate,
                                   LastLoginDate = member.LastLoginDate,
                               };

                var profile = UserProfile.GetUserProfile(user.Email) ?? new UserProfile();

                user.FirstName = profile.FirstName;
                user.LastName = profile.LastName;

                PopulateAssignedRolesData(user);

                return View(user);
            }
            ViewBag.PossibleRoles = Roles.GetAllRoles();
            return View();
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Edit(UserModel user,string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var member = Membership.GetUser(user.Email);
                
                //Update User
                if (member != null)
                {
                    member.Email = user.Email;                
                    Membership.UpdateUser(member);
                }

                UpdateUserRoles(selectedRoles, user);

                //Save profile
                var profile = UserProfile.GetUserProfile(user.Email);
                
                profile.FirstName = user.FirstName;
                profile.LastName = user.LastName;

                profile.Save();

                this.FlashInfo("Updated user");
                return RedirectToAction("Index", "Account");
            }
            // If we got this far, something failed, redisplay form
            PopulateAssignedRolesData(user);
            return View(user);
        }

        public ActionResult UserRoles()
        {
            ViewBag.Roles = Roles.GetAllRoles();

            return View();
        }

        private void PopulateAssignedRolesData(UserModel user)
        {
            var allRoles = Roles.GetAllRoles();
            var userRoles = new HashSet<string>(Roles.GetRolesForUser(user.Email));
            var viewModel = allRoles.Select(role => new AssignedRole
            {
                RoleName = role,
                Assigned = userRoles.Contains(role)
            }).ToList();

            ViewBag.PossibleRoles = viewModel;
        }

        private static void UpdateUserRoles(IEnumerable<string> selectedRoles, UserModel user)
        {
            var allRoles = Roles.GetAllRoles();

            if (selectedRoles == null)
            {
                Roles.RemoveUserFromRoles(user.Email, allRoles);
                return;
            }
            var selectedRolesHs = new HashSet<string>(selectedRoles);
            var userRoles = new HashSet<string>(Roles.GetRolesForUser(user.Email));
            
            foreach (var role in allRoles)
            {
                if (selectedRolesHs.Contains(role))
                {
                    if (!userRoles.Contains(role))
                    {
                        Roles.AddUserToRole(user.Email,role);
                    }
                }
                else
                {
                    if (userRoles.Contains(role))
                    {
                        Roles.RemoveUserFromRole(user.Email, role);
                    }
                }
            }
        }

        public ActionResult DeleteRole(string role)
        {
            Roles.DeleteRole(role);
            this.FlashWarning("Role Created");
            return RedirectToAction("UserRoles");
        }

        public ActionResult CreateRole(string role)
        {
            Roles.CreateRole(role);
            this.FlashInfo("Role Created");
            return RedirectToAction("UserRoles");
        }

        public ActionResult Lock(string email)
        {
            var user = Membership.GetUser(email);

            if (user != null)
            {
                user.IsApproved = false;

                Membership.UpdateUser(user);
            }

            this.FlashInfo("User Locked");

            return RedirectToAction("Index");
        }

        public ActionResult Delete(string email)
        {
            Membership.DeleteUser(email);

            this.FlashWarning("User deleted");

            return RedirectToAction("Index");
        }
    }
}
