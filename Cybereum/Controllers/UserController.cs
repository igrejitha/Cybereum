using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cybereum.Models;
using Cybereum.Filters;
using System.Net.Mail;
using System.Configuration;

namespace Cybereum.Controllers
{
    public class UserController : Controller
    {
        private cybereumEntities db = new cybereumEntities();
        EncryptDecrypt encrypt = new EncryptDecrypt();

        // GET: User
        public ActionResult Index()
        {
            //var tbl_user = db.tbl_user.Include(t => t.tbl_userrole).Where(t=> t.isactive==1);
            //return View(tbl_user.ToList());         

            GetUser(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]));
            return View();
        }

        public ActionResult ApprovedUsers()
        {
            GetApprovedUser();
            return View();
        }

        public ActionResult PendingUsers()
        {
            GetPendingUser();
            return View();
        }

        public string AcceptUser(int datuserid)
        {
            if (datuserid == null)
            {
                return "Invalid user";
            }

            tbl_user tbl_user = db.tbl_user.Find(datuserid);
            if (tbl_user == null)
            {
                return "Invalid user";
            }
            tbl_user.isactive = 1;
            db.Entry(tbl_user).State = EntityState.Modified;
            db.SaveChanges();

            //SendEmailToUser(tbl_user.emailid, tbl_user.firstname +" " + tbl_user.lastname);
            var UserLink = "/Account/login";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, UserLink);
            IGUtilities.SendConfirmationEmailToUser(tbl_user.emailid, tbl_user.firstname + " " + tbl_user.lastname, link);
            return "Success";
        }


        //public void SendEmailToUser(string emailId,string name)
        //{
        //    try
        //    {
        //        var UserLink = "/Account/login";
        //        var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, UserLink);

        //        var fromMail = new MailAddress(ConfigurationManager.AppSettings["SMTPUserName"].ToString()); // set your email    
        //        var fromEmailpassword = ConfigurationManager.AppSettings["SMTPPassword"].ToString(); // Set your password     
        //        var toEmail = new MailAddress(emailId);

        //        var smtp = new SmtpClient();
        //        smtp.Host = ConfigurationManager.AppSettings["SMTPServer"].ToString();
        //        smtp.Port = 25;
        //        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtp.UseDefaultCredentials = false;
        //        smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

        //        var Message = new MailMessage(fromMail, toEmail);
        //        Message.Subject = "Welcome to cybereum - Your One-Stop Project Management Solution with Cutting Edge Data Analytics and ML";
        //        Message.Body = "<br/> Dear " + name +","+
        //                        "<br/> We are so excited to welcome you to cybereum! You are now a part of a community of innovative individuals who are transforming the way they manage their projects and tasks." +
        //                        "<br/><br/> Cybereum is a powerful project management platform that combines cutting edge data analytics and ML to deliver a truly unique and comprehensive solution. With its user-friendly interface and advanced features, you'll be able to manage your projects and tasks with ease, increase efficiency, and achieve your goals like never before."+
        //                        "<br/> We wanted to take a moment to thank you for confirming your email and granting access to our platform. Now that you're all set up, it's time to dive in and start exploring! Here's what you can expect:" +
        //                        "<br/> Access to a comprehensive project management dashboard" +
        //                        "<br/> The ability to create projects and tasks with ease" +
        //                        "<br/> Assign tasks to team members with ease" +
        //                        "<br/> Track progress and deadlines in real - time" +
        //                        "<br/> Collaborate with team members in real - time" +
        //                        "<br/> And much more!" +
        //                        "<br/> We believe that cybereum will have a profound impact on your work, and we're eager to see what you'll achieve. So why wait? Start exploring and experience the power of cybereum for yourself!" +
        //                        "<br/><br/> Best regards," +
        //                        "<br/> The cybereum team." +
        //                        "<br/> Click below link to login." +
        //                        "<br/><br/><a href=" + link + ">" + link + "</a>";
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


        //public void SendRegisterEmailToUser(string emailId, string activationCode, string name)
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
        //        Message.Body = "Dear " + name + "," +
        //                       "< br/> We're thrilled to have you on board the cybereum project management platform! " +
        //                       " We're excited to help you streamline your project management process with our cutting-edge data analytics and ML integration." +
        //                       "<br/> To complete your registration, we need you to confirm your email address. Simply click on the link below to verify your email:" +
        //                       "<br/><br/><a href=" + link + ">" + link + "</a>";
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

        public string RejectUser(int datuserid)
        {
            if (datuserid == null)
            {
                return "Invalid user";
            }

            tbl_user tbl_user = db.tbl_user.Find(datuserid);
            if (tbl_user == null)
            {
                return "Invalid user";
            }
            tbl_user.isactive = 2;
            db.Entry(tbl_user).State = EntityState.Modified;
            db.SaveChanges();
            return "Success";
        }

        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetUser(int userid = 0, int roleid = 0)
        {
            JsonResult result = new JsonResult();
            try
            {
                cybereumEntities entities = new cybereumEntities();

                //var RecCount = entities.tblfiles.Count();
                // Initialization.  
                var search = Request.Form.GetValues("search[value]")[0];
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                //var length = Request.Form.GetValues("length").FirstOrDefault() == "-1" ? RecCount.ToString() : Request.Form.GetValues("length").FirstOrDefault();
                var length = "";
                if (Request.Form.GetValues("length").FirstOrDefault() == "-1")
                {
                    var RecCount = entities.tbl_user.Count();
                    length = RecCount.ToString();
                }
                else
                {
                    length = Request.Form.GetValues("length").FirstOrDefault();
                }
                //Find Order Column  
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (skip == 0)
                {
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = 0;
                }

                var files = entities.sp_FetchUsers(userid, roleid, skip, pageSize, sortColumn, sortColumnDir).ToList();

                // Total record count.  
                //recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]) == 0)
                {
                    recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = recordsTotal;
                }
                else
                {
                    recordsTotal = Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]);
                }
                // Loading drop down lists.                  
                result = this.Json(new { data = files, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
            }
            // Return info.     
            return result;
        }

        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetApprovedUser()
        {
            JsonResult result = new JsonResult();
            try
            {
                cybereumEntities entities = new cybereumEntities();

                //var RecCount = entities.tblfiles.Count();
                // Initialization.  
                var search = Request.Form.GetValues("search[value]")[0];
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                //var length = Request.Form.GetValues("length").FirstOrDefault() == "-1" ? RecCount.ToString() : Request.Form.GetValues("length").FirstOrDefault();
                var length = "";
                if (Request.Form.GetValues("length").FirstOrDefault() == "-1")
                {
                    var RecCount = entities.tbl_user.Count();
                    length = RecCount.ToString();
                }
                else
                {
                    length = Request.Form.GetValues("length").FirstOrDefault();
                }
                //Find Order Column  
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (skip == 0)
                {
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = 0;
                }

                var files = entities.sp_FetchApprovedUsers(skip, pageSize, sortColumn, sortColumnDir).ToList();

                // Total record count.  
                //recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]) == 0)
                {
                    recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = recordsTotal;
                }
                else
                {
                    recordsTotal = Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]);
                }
                // Loading drop down lists.                  
                result = this.Json(new { data = files, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
            }
            // Return info.     
            return result;
        }

        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetPendingUser()
        {
            JsonResult result = new JsonResult();
            try
            {
                cybereumEntities entities = new cybereumEntities();

                //var RecCount = entities.tblfiles.Count();
                // Initialization.  
                var search = Request.Form.GetValues("search[value]")[0];
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                //var length = Request.Form.GetValues("length").FirstOrDefault() == "-1" ? RecCount.ToString() : Request.Form.GetValues("length").FirstOrDefault();
                var length = "";
                if (Request.Form.GetValues("length").FirstOrDefault() == "-1")
                {
                    var RecCount = entities.tbl_user.Count();
                    length = RecCount.ToString();
                }
                else
                {
                    length = Request.Form.GetValues("length").FirstOrDefault();
                }
                //Find Order Column  
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][data]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();


                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                if (skip == 0)
                {
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = 0;
                }

                var files = entities.sp_FetchPendingUsers(skip, pageSize, sortColumn, sortColumnDir).ToList();

                // Total record count.  
                //recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]) == 0)
                {
                    recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                    System.Web.HttpContext.Current.Session["TotalRecordCount"] = recordsTotal;
                }
                else
                {
                    recordsTotal = Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]);
                }
                // Loading drop down lists.                  
                result = this.Json(new { data = files, recordsTotal = recordsTotal, recordsFiltered = recordsTotal }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
            }
            // Return info.     
            return result;
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_user tbl_user = db.tbl_user.Find(id);
            if (tbl_user == null)
            {
                return HttpNotFound();
            }
            return View(tbl_user);
        }

        public ActionResult List()
        {
            var tbl_user = db.tbl_user.Include(t => t.tbl_userrole).Where(t => t.isactive == 1);
            return View(tbl_user.ToList());
        }

        // GET: User/Create
        public ActionResult Create()
        {
            ViewBag.roleid = new SelectList(db.tbl_userrole, "roleid", "rolename");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "userid,username,password,roleid,firstname,lastname,isactive,emailid,organization,createddate,emailverification,otp,activationcode")] UserViewModel tbluser)
        {
            if (ModelState.IsValid)
            {
                tbl_user user = new tbl_user();
                user.userid = tbluser.userid;
                user.firstname = tbluser.firstname;
                user.lastname = tbluser.lastname;
                user.username = "";
                user.emailid = tbluser.emailid;
                user.password = encrypt.Encrypt(tbluser.Password);
                user.createddate = DateTime.Now;
                user.organization = tbluser.organization;
                user.roleid = (int)Role.User;
                user.isactive = 0;
                user.emailverification = false;

                if (Convert.ToInt16(Session["RoleId"]) == (int)Role.ProjectManager)
                {
                    user.pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                }

                string OTP = IGUtilities.GeneratePassword();
                user.otp = OTP;

                string password = encrypt.Decrypt(user.password);

                user.activationcode = Guid.NewGuid().ToString();

                db.tbl_user.Add(user);
                db.SaveChanges();
                var GenarateUserVerificationLink = "/Account/UserVerification/" + user.activationcode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);
                IGUtilities.SendRegisterEmailToUser(link, user.emailid, user.activationcode, user.firstname + " " + user.lastname);
                //message = "Verification mail sent to your mail id";
                return RedirectToAction("Index");
            }

            ViewBag.roleid = new SelectList(db.tbl_userrole, "roleid", "rolename", tbluser.roleid);
            return View(tbluser);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_user tbl_user = db.tbl_user.Find(id);
            if (tbl_user == null)
            {
                return HttpNotFound();
            }
            EncryptDecrypt encrypt = new EncryptDecrypt();
            tbl_user.password = encrypt.Decrypt(tbl_user.password);
            ViewBag.roleid = new SelectList(db.tbl_userrole, "roleid", "rolename", tbl_user.roleid);
            return View(tbl_user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "userid,username,password,roleid,firstname,lastname,isactive,emailid,organization,createddate,emailverification,otp,activationcode")] tbl_user tbl_user)
        {
            if (ModelState.IsValid)
            {
                EncryptDecrypt encrypt = new EncryptDecrypt();
                tbl_user.password = encrypt.Encrypt(tbl_user.password);
                db.Entry(tbl_user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ApprovedUsers");
            }
            ViewBag.roleid = new SelectList(db.tbl_userrole, "roleid", "rolename", tbl_user.roleid);
            return View(tbl_user);
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_user tbl_user = db.tbl_user.Find(id);
            if (tbl_user == null)
            {
                return HttpNotFound();
            }
            return View(tbl_user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_user tbl_user = db.tbl_user.Find(id);
            db.tbl_user.Remove(tbl_user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit User";
            tbl_user user = new tbl_user();

            //var Result = db.tbl_user.ToList();
            //var OutResult = Result.Where(a => a.userid == Id).FirstOrDefault();
            user = db.tbl_user.Find(Id);
            user.password = encrypt.Decrypt(user.password);
            //if (OutResult != null)
            //{
            //    user.userid = OutResult.userid;
            //    user.emailid = OutResult.emailid;                
            //    user.firstname = OutResult.firstname;
            //    user.lastname = OutResult.lastname;
            //    user.organization = OutResult.organization;
            //    user.password = encrypt.Decrypt(OutResult.password);
            //    user.createddate = OutResult.createddate;
            //    user.organization = OutResult.organization;
            //    user.roleid = OutResult.roleid;
            //    user.isactive = OutResult.isactive;                
            //    user.emailverification = OutResult.emailverification;

            //    user.activationcode  = OutResult.activationcode;
            //    user.otp = OutResult.otp;
            //    user.pmuserid = OutResult.pmuserid;
            //    user.username = OutResult.username;
            //}
            return RedirectToAction("AddUsers", user);
        }

        [Authorize]
        [SessionTimeout]
        public ViewResult AddUsers(int? id, tbl_user Users)
        {
            ViewBag.roleid = new SelectList(db.tbl_userrole, "roleid", "rolename");
            //List<SelectListItem> levelone = GetLevelOne(Users.userid);            
            return View(Users);
        }

        //[Authorize]
        //[SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public ActionResult AddUsers([Bind(Include = "userid,username,password,roleid,firstname,lastname,isactive,emailid,organization,createddate,emailverification,otp,activationcode,pmuserid")] tbl_user logindetails)
        //public ActionResult AddUsers(FormCollection formval, UserLevelOne _mod)
        {            
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                int? objList = 0;
                //objList = db.sp_FetchUserExists(logindetails.emailid, logindetails.password).FirstOrDefault();
                if (logindetails.userid == 0)
                {
                    objList = db.sp_FetchUserExists(logindetails.emailid).FirstOrDefault();
                    //var objList1 = db.tbl_user.Where(x => x.emailid == logindetails.emailid && x.isactive != 2).FirstOrDefault();
                    //objList = objList1.userid;
                }

                switch (objList)
                {
                    case 1:
                        message = "Email already exists.";
                        break;
                    default:                        
                        tbl_user user = new tbl_user();
                        user.userid = logindetails.userid;
                        user.firstname = logindetails.firstname;
                        user.lastname = logindetails.lastname;
                        user.username = "";
                        user.emailid = logindetails.emailid;
                        user.password = encrypt.Encrypt(logindetails.password);
                        user.createddate = DateTime.Now;
                        user.organization = logindetails.organization;
                        user.roleid = (int)Role.User;
                        user.isactive = 0;
                        user.emailverification = false;

                        if (Convert.ToInt16(Session["RoleId"]) == (int)Role.ProjectManager)
                        {
                            user.pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
                        }

                        string OTP = IGUtilities.GeneratePassword();
                        user.otp = OTP;

                        string password = encrypt.Decrypt(user.password);

                        user.activationcode = Guid.NewGuid().ToString();                        

                        if (logindetails.userid == 0)
                        {
                            db.tbl_user.Add(user);
                            db.SaveChanges();

                            var GenarateUserVerificationLink = "/Account/UserVerification/" + user.activationcode;
                            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, GenarateUserVerificationLink);
                            IGUtilities.SendRegisterEmailToUser(link,user.emailid,user.activationcode,user.firstname + " " + user.lastname);
                            message = "Verification mail sent to your mail id";
                        }
                        else
                        {
                            tbl_user useredit = db.tbl_user.Find(logindetails.userid);

                            useredit.firstname = logindetails.firstname;
                            useredit.lastname = logindetails.lastname;
                            useredit.username = "";
                            useredit.emailid = logindetails.emailid;
                            useredit.password = encrypt.Encrypt(logindetails.password);
                            useredit.organization = logindetails.organization;
                            useredit.roleid = logindetails.roleid;
                            useredit.pmuserid = logindetails.pmuserid;
                            //if ((int)Session["RoleId"] == (int)Role.ProjectManager)
                            //{
                            //    user.pmuserid = (int)Session["LoggedInUserId"];
                            //}
                            db.Entry(useredit).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                                                
                        return RedirectToAction("Index");
                }
                ViewBag.Message = message;
                //return RedirectToAction("Login");                
                return View(logindetails);
            }
            return View(logindetails);
        }
    }
}
