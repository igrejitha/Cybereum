using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security;
using Cybereum.Models;
using System.Web.Security;
using Microsoft.IdentityModel.Protocols;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Net;
using System.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Cybereum.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            // Send an OpenID Connect sign-in request.
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Home/Index" },
                    OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            HttpContext.GetOwinContext().Authentication.SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult SignOutCallback()
        {
            if (Request.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Login");
            //return View();
        }


        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            if (Request.IsAuthenticated)
            {
                LoginViewModel mod = new LoginViewModel();
                mod.Email = User.Identity.Name;
                return View(mod);
            }
            return View();
        }

        public ActionResult UserLogin()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            if (Convert.ToInt32(Session["RoleId"]) == 3)
                return RedirectToAction("UserLogin");
            else
                return RedirectToAction("Login");

            //string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

            //HttpContext.GetOwinContext().Authentication.SignOut(
            //    new AuthenticationProperties { RedirectUri = callbackUrl },
            //    OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            //return RedirectToAction("Login");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel user)
        {
            try
            {                
                //if (!Request.IsAuthenticated)
                //{
                //    HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/Home/Index" },
                //        OpenIdConnectAuthenticationDefaults.AuthenticationType);
                //}

                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    using (cybereumEntities objdmsEntities = new cybereumEntities())
                    {

                        int? userId = 0; // objdmsEntities.ValidateUsers(user.email, user.password).FirstOrDefault();

                        EncryptDecrypt encrypt = new EncryptDecrypt();
                        string password = encrypt.Encrypt(user.password);
                        //var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.password == password).FirstOrDefault();                        
                        var objList = objdmsEntities.sp_FetchLoginDetails(user.Email, password).FirstOrDefault();
                        if (objList != null)
                        {
                            switch (objList.userid)
                            {
                                case -1:
                                    message = "Username and/or password is incorrect.";
                                    break;
                                case -2:
                                    message = "Account has not been activated.";
                                    break;
                                default:
                                    FormsAuthentication.SetAuthCookie(user.Email, true);
                                    //Session.Timeout = 90;                                    
                                    Session["LoggedInUserId"] = objList.userid;
                                    Session["RoleId"] = objList.roleid;
                                    Session["Username"] = objList.username;
                                    AuthorizeAttribute objAuth = new AuthorizeAttribute();
                                    if (objList.roleid == (int)Role.Admin)
                                    {
                                        objAuth.Roles = Role.Admin.ToString();
                                        Session["RoleName"] = Role.Admin.ToString();
                                        return RedirectToAction("Index", "Home");
                                    }
                                    else
                                    {
                                        objAuth.Roles = Role.User.ToString();
                                        Session["RoleName"] = (Role)objList.roleid;
                                        return RedirectToAction("List", "Project");
                                    }
                            }
                        }
                        else
                        {
                            message = "Invalid Account";
                        }
                    }
                    ViewBag.Message = message;
                    return View(user);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                IGUtilities.WriteLog(ex.Message);
                IGUtilities.WriteLog(ex.Data.ToString());
                if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
                IGUtilities.WriteLog(ex.TargetSite.ToString());
                throw ex;
            }
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserLogin(LoginViewModel user)
        {
            try
            {
                //if (IGUtilities.AuthenticateUser("LDAP://srjigs.com", user.Email, user.password) == false)
                //{

                //}

                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    using (cybereumEntities objdmsEntities = new cybereumEntities())
                    {

                        int? userId = 0; // objdmsEntities.ValidateUsers(user.email, user.password).FirstOrDefault();

                        EncryptDecrypt encrypt = new EncryptDecrypt();
                        string password = encrypt.Encrypt(user.password);
                        var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.password == password && x.isactive == 1 && x.roleid == 3).FirstOrDefault();
                        if (objList != null)
                        {
                            if (objList.userid > 0)
                            {
                                FormsAuthentication.SetAuthCookie(user.Email, true);
                                //Session.Timeout = 90;                                    
                                Session["LoggedInUserId"] = objList.userid;
                                Session["RoleId"] = objList.roleid;
                                AuthorizeAttribute objAuth = new AuthorizeAttribute();
                                if (objList.roleid == (int)Role.User)
                                {
                                    objAuth.Roles = Role.User.ToString();
                                    Session["RoleName"] = (Role)objList.roleid;
                                    return RedirectToAction("Dashboard", "Home");
                                }
                                else
                                {
                                    objAuth.Roles = Role.User.ToString();
                                    Session["RoleName"] = (Role)objList.roleid;
                                    return RedirectToAction("Dashboard", "Home");
                                }
                            }
                        }
                        else
                        {
                            message = "Username and/or password is incorrect.";
                        }
                    }
                    ViewBag.Message = message;
                    return View(user);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                IGUtilities.WriteLog(ex.Message);
                IGUtilities.WriteLog(ex.Data.ToString());
                if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
                IGUtilities.WriteLog(ex.TargetSite.ToString());
                throw ex;
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegisterViewModel user)
        {
            try
            {
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    using (cybereumEntities objdmsEntities = new cybereumEntities())
                    {
                        int? userId = 0; // objdmsEntities.ValidateUsers(user.email, user.password).FirstOrDefault();

                        var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.email && x.isactive != 2).FirstOrDefault();

                        if (objList == null)
                        {
                            EncryptDecrypt encrypt = new EncryptDecrypt();
                            if (ModelState.IsValid)
                            {
                                tbl_user usertbl = new tbl_user();
                                usertbl.firstname = user.firstname;
                                usertbl.lastname = user.lastname;
                                usertbl.username = "";
                                usertbl.emailid = user.email;
                                usertbl.password = encrypt.Encrypt(user.Password);
                                usertbl.createddate = DateTime.Now;
                                usertbl.organization = user.organization;
                                usertbl.roleid = (int)Role.ProjectManager;
                                usertbl.isactive = 0;
                                usertbl.emailverification = false;

                                string OTP = IGUtilities.GeneratePassword();
                                usertbl.otp = OTP;

                                string password = encrypt.Decrypt(usertbl.password);

                                usertbl.activationcode = Guid.NewGuid().ToString();

                                objdmsEntities.tbl_user.Add(usertbl);
                                objdmsEntities.SaveChanges();


                                //SendEmailToUser(user.email, usertbl.activationcode,user.firstname +" " + user.lastname );
                                var GenarateUserVerificationLink = "/Account/UserVerification/" + usertbl.activationcode;
                                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);
                                IGUtilities.SendRegisterEmailToUser(link, usertbl.emailid, usertbl.activationcode, usertbl.firstname + " " + usertbl.lastname);
                                message = "Verification mail sent to registered mail id";
                                ViewBag.Message = message;
                                //user = new RegisterViewModel();
                                return View(user);
                            }
                        }
                        else if (objList.emailverification == false)
                        {
                            message = "Email already exists. Yet to be verified by the user.";
                        }
                        else if (objList.isactive == 0 && objList.emailverification == true)
                        {
                            message = "Email already exists. Yet to be approved by admin.";
                        }
                        else if (objList.isactive == 1 && objList.emailverification == true)
                        {
                            message = "Email already exists.";
                        }
                    }
                    ViewBag.Message = message;
                    return View(user);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                IGUtilities.WriteLog(ex.Message);
                IGUtilities.WriteLog(ex.Data.ToString());
                if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
                IGUtilities.WriteLog(ex.TargetSite.ToString());
                throw ex;
            }
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotPwdViewModel user)
        {
            try
            {
                string message = string.Empty;
                if (ModelState.IsValid)
                {
                    cybereumEntities objEntities = new cybereumEntities();

                    //var objList = objdmsEntities.FetchLoginDetails(user.email, "").FirstOrDefault();
                    var objList = objEntities.tbl_user.FirstOrDefault(l => l.emailid == user.email);

                    if (objList != null)
                    {
                        switch (objList.isactive)
                        {
                            case 0:
                                message = "Account has not been activated.";
                                break;
                            case 1:
                                try
                                {
                                    List<string> strAttachmentFile = new List<string>();
                                    var Codeid = Guid.NewGuid();
                                    string strFromMailId = ConfigurationManager.AppSettings["SMTPUserName"].ToString();
                                    string strToMailId = objList.emailid;
                                    string strSubject = "Password Reset Link for Cybereum";
                                    string strSignature = "<br/>Best regards, <br/>The cybereum team.";
                                    string strResetLink = "<a href='" + Url.Action("RecoverPassword", "Account", new { email = objList.emailid, code = Convert.ToString(Codeid) }, "http") + "'>Reset Password</a>";
                                    string strMessage = "<html><body><span style='font-family:Calibri;font-size: 11pt;'>Hi " + objList.firstname
                                                        + ",<br><br> You recently requested to reset your password for your Cybereum account. " +
                                                        " <br> Please click below link to reset the password <br>" + strResetLink + "<br>"
                                                        + strSignature + "</span></body></html>";
                                    message = "Please check your mail to reset the password.";
                                    IGUtilities.SendEmail(strFromMailId, strToMailId, strSubject, strAttachmentFile, strMessage);

                                    using (cybereumEntities objEnt = new cybereumEntities())
                                    {
                                        tbl_user tbluser = objEnt.tbl_user.Find(objList.userid);
                                        tbluser.GUID = Convert.ToString(Codeid);
                                        objEnt.Entry(tbluser).State = EntityState.Modified;
                                        objEnt.SaveChanges();
                                    }
                                }
                                catch (DbEntityValidationException e)
                                {

                                    foreach (var eve in e.EntityValidationErrors)
                                    {
                                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                        IGUtilities.WriteLog(eve.Entry.Entity.GetType().Name);
                                        IGUtilities.WriteLog(Convert.ToString(eve.Entry.State));
                                        foreach (var ve in eve.ValidationErrors)
                                        {
                                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                                            IGUtilities.WriteLog(ve.PropertyName);
                                            IGUtilities.WriteLog(ve.ErrorMessage);
                                        }
                                    }
                                    throw;
                                }
                                break;
                            default:
                                return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        message = "Mail ID is not registered.";
                    }

                    if (message == "Please check mail to reset the password.")
                        ViewBag.Message2 = message;
                    else
                        ViewBag.Message = message;
                    //return RedirectToAction("Login");                
                    return View(user);
                }

                return View(user);
            }
            catch (Exception ex)
            {
                IGUtilities.WriteLog(ex.Message);
                throw ex;
            }


        }

        public ActionResult RecoverPassword(string email, string code)
        {
            cybereumEntities objEntities = new cybereumEntities();
            var objList = objEntities.tbl_user.FirstOrDefault(l => l.emailid == email && l.isactive == 1 && l.GUID == code);

            if (objList == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            else
            {
                ViewBag.email = email;
                ViewBag.code = code;
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult RecoverPassword(ResetPwdViewModel pwd)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                cybereumEntities objEntities = new cybereumEntities();
                EncryptDecrypt encrypt = new EncryptDecrypt();

                var mailId = Request.Form["email"];
                var code = Request.Form["code"];
                var objList = objEntities.tbl_user.FirstOrDefault(l => l.emailid == mailId && l.GUID == code);

                int roleid = 0;

                if (objList != null)
                {
                    switch (objList.isactive)
                    {
                        case 0:
                            message = "Link is invalid";
                            break;
                        case 1:
                            tbl_user tbluser = objEntities.tbl_user.Find(objList.userid);
                            tbluser.password = encrypt.Encrypt(pwd.ConfirmPassword);
                            tbluser.GUID = "";
                            objEntities.Entry(tbluser).State = EntityState.Modified;
                            objEntities.SaveChanges();

                            roleid = tbluser.roleid;
                            break;
                        default:
                            if (roleid == 3)
                            {
                                return RedirectToAction("UserLogin");
                            }
                            else
                            {
                                return RedirectToAction("Login");
                            }
                    }
                }
                if (roleid == 3)
                {
                    return RedirectToAction("UserLogin");
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            return View(pwd);
        }

        #region Verification from Email Account.    
        public ActionResult UserVerification(string id)
        {
            try
            {
                bool Status = false;

                cybereumEntities objEnt = new cybereumEntities();

                objEnt.Configuration.ValidateOnSaveEnabled = false; // Ignore to password confirmation     
                var IsVerify = objEnt.tbl_user.Where(u => u.activationcode == new Guid(id).ToString()).FirstOrDefault();
                if (IsVerify != null)
                {
                    if (IsVerify.emailverification == true && IsVerify.isactive != 2)
                    {
                        ViewBag.Message = "Email already verified";
                        ViewBag.Status = true;
                    }
                    else
                    {
                        IsVerify.emailverification = true;
                        objEnt.SaveChanges();
                        ViewBag.Message = "Email Verification completed";
                        ViewBag.Status = true;
                    }
                }
                else
                {
                    ViewBag.Message = "Invalid Request...";
                    ViewBag.Status = false;
                }

                return View();
            }
            catch (Exception ex)
            {
                IGUtilities.WriteLog(ex.Message);
                IGUtilities.WriteLog(ex.Data.ToString());
                if (ex.InnerException != null) IGUtilities.WriteLog(ex.InnerException.Message);
                IGUtilities.WriteLog(ex.TargetSite.ToString());
                throw ex;
            }
        }
        #endregion
    }
}