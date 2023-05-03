using Cybereum.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Cybereum.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            //LoginViewModel user = new LoginViewModel();
            //user.Email = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            return View();
        }

        public ActionResult UserLogin()
        {
            //LoginViewModel user = new LoginViewModel();
            //user.Email = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
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
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel user)
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
                        //var objList = objdmsEntities.tbl_user.Where(x => x.emailid == user.Email && x.password == password).FirstOrDefault();                        
                        var objList = objdmsEntities.sp_FetchLoginDetails(user.Email, password).FirstOrDefault();
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
                                    return RedirectToAction("Dashboard", "Home");
                                }
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
                IGUtilities.WriteLog(ex.InnerException.Message);
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
                        //var objList = objdmsEntities.sp_FetchLoginDetails(user.Email, password).FirstOrDefault();
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
                IGUtilities.WriteLog(ex.InnerException.Message);
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
                IGUtilities.WriteLog(ex.InnerException.Message);
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
        //public void SendEmailToUser(string emailId, string activationCode,string name)
        //{
        //    try
        //    {
        //        var GenarateUserVerificationLink = "/Account/UserVerification/" + activationCode;
        //        var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);

        //        var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
        //        var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
        //        var toEmail = new MailAddress(emailId);

        //        var smtp = new SmtpClient();
        //        smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
        //        smtp.Port = 25;
        //        //smtp.EnableSsl = true;            
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

        //        var Message = new MailMessage(fromMail, toEmail);
        //        Message.Subject = "Welcome to Cybereum Project Management - Confirm Your email for Registration";
        //        Message.Body = "Dear " + name + ","+
        //                        "< br/> We're thrilled to have you on board the cybereum project management platform! " +
        //                        " We're excited to help you streamline your project management process with our cutting-edge data analytics and ML integration." +
        //                        "<br/> To complete your registration, we need you to confirm your email address. Simply click on the link below to verify your email:" +
        //                        "<br/><br/><a href=" + link + ">" + link + "</a>" +
        //                        "<br/><br/> Once you've confirmed your email and your registration is granted, you'll have access to our full suite of project management tools, including advanced data analytics and ML features that will help you make more informed decisions and drive better outcomes for your projects." +
        //                        "<br/> If you have any questions or need assistance getting started, please don't hesitate to reach out to our support team at support@cybereum.io " +
        //                        "<br/> We're here to help." +
        //                        "<br/> <br/> Best regards," +
        //                        "<br/> The cybereum Team";
        //        Message.IsBodyHtml = true;
        //        smtp.Send(Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        IGUtilities.WriteLog(ex.Message);
        //        IGUtilities.WriteLog(ex.Data.ToString());
        //        IGUtilities.WriteLog(ex.InnerException.Message);
        //        IGUtilities.WriteLog(ex.TargetSite.ToString());
        //        throw ex;
        //    }
        //}

        #region Verification from Email Account.    
        public ActionResult UserVerification(string id)
        {
            try
            {
                bool Status = false;

                cybereumEntities objEnt = new cybereumEntities();

                objEnt.Configuration.ValidateOnSaveEnabled = false; // Ignor to password confirmation     
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
                IGUtilities.WriteLog(ex.InnerException.Message);
                IGUtilities.WriteLog(ex.TargetSite.ToString());
                throw ex;
            }
        }
        #endregion

    }
}