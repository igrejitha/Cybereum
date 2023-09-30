<<<<<<< Updated upstream
﻿using System;
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
    [Authorize]
    public class UserController : Controller
    {
        private cybereumEntities db = new cybereumEntities();
        EncryptDecrypt encrypt = new EncryptDecrypt();

        // GET: User
        public ActionResult Index()
        {
            //GetUser(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]));
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

            
            var UserLink = "/Account/login";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, UserLink);
            IGUtilities.SendConfirmationEmailToUser(tbl_user.emailid, tbl_user.firstname + " " + tbl_user.lastname, link);
            return "Success";            
        }

        
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

            user = db.tbl_user.Find(Id);
            user.password = encrypt.Decrypt(user.password);
            
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
=======
﻿using System;
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
    [Authorize]
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
    [SessionTimeout]
>>>>>>> Stashed changes
=======
    [SessionTimeout]
>>>>>>> Stashed changes
    public class UserController : Controller
    {
        private cybereumEntities db = new cybereumEntities();
        EncryptDecrypt encrypt = new EncryptDecrypt();

        // GET: User
        //[Authorize(Roles = "OrganizationManager")]
        [Authorize]
        public ActionResult Index()
        {
            //GetUser(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]));
            return View();
        }

        [Authorize]
        public ActionResult ApprovedUsers()
        {
            GetApprovedUser();
            return View();
        }

        [Authorize]
        public ActionResult PendingUsers()
        {
            GetPendingUser();
            return View();
        }

        [Authorize]
        public ActionResult OrganizationList()
        {
            //GetOrganization();
            return View();
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            int userid = Convert.ToInt16(Session["LoggedInUserId"]);
            var mod= db.tbl_user.FirstOrDefault(l => l.userid == userid);
            ViewBag.userid = mod.userid;
            ViewBag.email = mod.emailid;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ChangePassword(ChangePwdViewModel pwd)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                EncryptDecrypt encrypt = new EncryptDecrypt();

                //var mailId = Request.Form["email"];
                //var code = Request.Form["code"];
                var objList = db.tbl_user.FirstOrDefault(l => l.userid == pwd.userid);

                if (objList != null)
                {
                    if (pwd.OldPassword != encrypt.Decrypt(objList.password))
                    {
                        message = "Old password is invalid";
                    }
                    else if (pwd.OldPassword == pwd.Password)
                    {
                        message = "Old password and new password should not be same";
                    }
                    else if (objList.isactive == 1)
                    {
                        tbl_user tbluser = db.tbl_user.Find(objList.userid);
                        tbluser.password = encrypt.Encrypt(pwd.ConfirmPassword);
                        db.Entry(tbluser).State = EntityState.Modified;
                        db.SaveChanges();
                        message = "Password changed successfully";
                    }
                }                
            }
            ViewBag.userid = pwd.userid;
            ViewBag.email = pwd.email;            
            ViewBag.Message = message;
            //return RedirectToAction("ChangePassword", new { userid = pwd.userid });
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


            var UserLink = "/Account/login";
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, UserLink);
            IGUtilities.SendConfirmationEmailToUser(tbl_user.emailid, tbl_user.firstname + " " + tbl_user.lastname, link);
            return "Success";
        }


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
            IGUtilities.SendRejectEmailToUser (tbl_user.emailid, tbl_user.firstname + " " + tbl_user.lastname);
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


        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetOrganization()
        {
            JsonResult result = new JsonResult();
            try
            {
                cybereumEntities entities = new cybereumEntities();

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

                var files = (from p in entities.tbl_user
                             where p.emailverification == true && p.isactive == 1 && p.roleid != 1
                             select new { organization = p.organization }).Distinct().ToList();
                // Total record count.  
                //recordsTotal = files.Count > 0 ? files[0].TotalRecordCount : 0;
                if (Convert.ToInt32(System.Web.HttpContext.Current.Session["TotalRecordCount"]) == 0)
                {
                    recordsTotal = files.Count > 0 ? files.Count() : 0;
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

        //[CustomAuthorize(Roles = "OrganizationManager")]
        [Authorize]
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

            user = db.tbl_user.Find(Id);
            user.password = encrypt.Decrypt(user.password);

            return RedirectToAction("AddUsers", user);
        }

        //[CustomAuthorize(Roles = "OrganizationManager")]
        [Authorize]
        [SessionTimeout]
        public ViewResult AddUsers(int? id, tbl_user Users)
        {
            //ViewBag.roleid = new SelectList(db.tbl_userrole, "roleid", "rolename");
            var pm = new int[] { 2, 3, 5 };
            ViewBag.roleid = new SelectList(db.tbl_userrole.Where(m => pm.Contains(m.roleid)), "roleid", "rolename").ToList();
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
                logindetails.organization = Session["Organization"].ToString();
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
                        user.roleid = logindetails.roleid;
                        user.isactive = 0;
                        user.emailverification = false;

                        if (Convert.ToInt16(Session["RoleId"]) == (int)Role.OrganizationAdmin)
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
                            IGUtilities.SendRegisterEmailToUser(link, user.emailid, user.activationcode, user.firstname + " " + user.lastname);
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
                            useredit.pmuserid = user.pmuserid;
                            db.Entry(useredit).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        return RedirectToAction("Index");
                }
                ViewBag.Message = message;                
                var pm = new int[] { 2, 3, 5 };
                ViewBag.roleid = new SelectList(db.tbl_userrole.Where(m => pm.Contains(m.roleid)), "roleid", "rolename").ToList();
                return View(logindetails);
            }
            var pm1 = new int[] { 2, 3, 5 };
            ViewBag.roleid = new SelectList(db.tbl_userrole.Where(m => pm1.Contains(m.roleid)), "roleid", "rolename").ToList();
            return View(logindetails);
        }
    }
}
>>>>>>> Stashed changes
