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

namespace Cybereum.Controllers
{
    public class TaskController : Controller
    {
        private cybereumEntities db = new cybereumEntities();

        // GET: Task
        [Authorize]
        [SessionTimeout]
        public ActionResult Index(int? milestoneid)
        {
            //var tbl_task = db.tbl_task.Include(t => t.tbl_project).Include(t => t.tbl_user).Include(t => t.tbl_tasktype);
            //return View(tbl_task.ToList());

            TempData["MilestoneId"] = milestoneid;
            ViewBag.milestoneid = TempData["MilestoneId"];
            TempData.Keep();
            if (milestoneid == 0)
            {
                ViewBag.milestoneid = Convert.ToInt32(Session["MilestoneId"]);
                Session["MilestoneId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.milestoneid = milestoneid;
                Session["MilestoneId"] = milestoneid;
            }
            GetTask(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]), milestoneid);
            return View();
        }

        // GET: Task/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_task tbl_task = db.tbl_task.Find(id);
            if (tbl_task == null)
            {
                return HttpNotFound();
            }
            return View(tbl_task);
        }

        public ActionResult Addrecord(int Id)
        {
            ViewBag.Message = "Edit Task";
            tbl_task task = new tbl_task();

            task = db.tbl_task.Find(Id);
            return RedirectToAction("Create", task);
        }

        // GET: Task/Create
        [Authorize]
        [SessionTimeout]
        public ActionResult Create(int? taskid, int? milestoneid, TaskViewModel Tasks)
        {
            if (milestoneid == 0)
            {
                ViewBag.milestoneid = Convert.ToInt32(Session["MilestoneId"]);
                Session["MilestoneId"] = ViewBag.milestoneid;
            }
            else
            {
                ViewBag.milestoneid = Tasks.milestoneid;
                Session["MilestoneId"] = milestoneid;
            }

            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            List<SelectListItem> user = Filluser(pmuserid);
            ViewBag.assignedto = user;

            List<SelectListItem> tasktype = (from b in db.tbl_tasktype
                                             where b.isactive == 1
                                             select new SelectListItem
                                             {
                                                 Text = b.tasktypename,
                                                 Value = b.tasktypeid.ToString()
                                             }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.tasktypeid = tasktype;

            List<SelectListItem> status = (from b in db.tbl_status
                                           where b.isactive == 1
                                           select new SelectListItem
                                           {
                                               Text = b.statusname,
                                               Value = b.statusid.ToString()
                                           }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.statusid = status;

            if (taskid == null)
            {
                Tasks.startdate = DateTime.Today;
                Tasks.enddate = DateTime.Today;
            }
            return View(Tasks);
        }

        public List<SelectListItem> Filluser(int? pmuserid)
        {
            List<SelectListItem> user = (from b in db.tbl_user
                                         where b.pmuserid == pmuserid && b.isactive == 1 && b.roleid==3
                                         select new SelectListItem
                                         {
                                             Text = b.firstname + " " + b.lastname,
                                             Value = b.userid.ToString()
                                         }).Distinct().OrderBy(x => x.Text).ToList();
            //user.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            return user;
        }

        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "taskid,taskname,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,milestoneid,projectid,priority,tasktypeid,assignedto")] TaskViewModel tbl_task)
        //public ActionResult AddUsers(FormCollection formval, UserLevelOne _mod)
        {
            tbl_task.milestoneid = Convert.ToInt16(TempData["MilestoneId"]);
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                //int? objList = 0;
                ////objList = db.sp_FetchUserExists(logindetails.emailid, logindetails.password).FirstOrDefault();
                //if (tbl_task.taskid == 0)
                //{
                //    objList = db.sp_FetchTaskExists(tbl_task.taskname).FirstOrDefault();                    
                //}
                var objList = db.tbl_task.Find(tbl_task.taskid);
                if (tbl_task.taskid == 0)
                {
                    //objList = db.sp_FetchTaskExists(tbl_milestone.milestonename).FirstOrDefault();
                    objList = db.tbl_task.Where(x => x.taskname == tbl_task.taskname).FirstOrDefault();
                    //objList = db.tbl_milestone.Where(a=> a.milestonename == tbl_milestone.milestonename).FirstOrDefault();
                }
                else
                {
                    objList = db.tbl_task.Where(x => x.taskname == tbl_task.taskname && x.taskid != tbl_task.taskid).FirstOrDefault();
                }

                if (objList != null)
                {
                    message = "task name already exists.";
                }
                else
                {
                    if (tbl_task.taskid == 0)
                    {
                        tbl_task task = new tbl_task();
                        task.taskname = tbl_task.taskname;
                        task.milestoneid = tbl_task.milestoneid;
                        task.startdate = tbl_task.startdate;
                        task.enddate = tbl_task.enddate;
                        task.priority = tbl_task.priority;
                        task.assignedto = tbl_task.assignedto;
                        task.tasktypeid = tbl_task.tasktypeid;
                        task.statusid = tbl_task.statusid;
                        task.isactive = 1;
                        task.createddate = DateTime.Now;
                        task.createdby = Convert.ToInt16(Session["LoggedInUserId"]);
                        db.tbl_task.Add(task);
                        db.SaveChanges();
                        message = "Saved Successfully";
                    }
                    else
                    {
                        tbl_task taskedit = db.tbl_task.Find(tbl_task.taskid);

                        taskedit.taskname = tbl_task.taskname;
                        taskedit.milestoneid = tbl_task.milestoneid;
                        taskedit.startdate = tbl_task.startdate;
                        taskedit.enddate = tbl_task.enddate;
                        taskedit.priority = tbl_task.priority;
                        taskedit.assignedto = tbl_task.assignedto;
                        taskedit.tasktypeid = tbl_task.tasktypeid;
                        taskedit.statusid = tbl_task.statusid;
                        taskedit.isactive = 1;
                        taskedit.modifieddate = DateTime.Now;
                        taskedit.modifiedby = Convert.ToInt16(Session["LoggedInUserId"]);
                        taskedit.isactive = 1;
                        db.Entry(taskedit).State = EntityState.Modified;                        
                        db.SaveChanges();
                        message = "Modified Successfully";
                    }

                    return RedirectToAction("Index", new { milestoneid = tbl_task.milestoneid });
                }
                ViewBag.Message = message;
                return RedirectToAction("Create", "Task", new { milestoneid = tbl_task.milestoneid });
            }
            int pmuserid = Convert.ToInt32(Session["LoggedInUserId"]);
            List<SelectListItem> user = Filluser(pmuserid);
            ViewBag.assignedto = user;

            List<SelectListItem> tasktype = (from b in db.tbl_tasktype
                                             where b.isactive == 1
                                             select new SelectListItem
                                             {
                                                 Text = b.tasktypename,
                                                 Value = b.tasktypeid.ToString()
                                             }).Distinct().OrderBy(x => x.Text).ToList();
            ViewBag.tasktypeid = tasktype;

            List<SelectListItem> status = (from b in db.tbl_status
                                           where b.isactive == 1
                                           select new SelectListItem
                                           {
                                               Text = b.statusname,
                                               Value = b.statusid.ToString()
                                           }).Distinct().OrderBy(x => x.Text).ToList();
            //status.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            ViewBag.statusid = status;
            //return View(tbl_task);
            return RedirectToAction("Create", "Task", new { milestoneid = tbl_task.milestoneid });
        }

        // GET: Task/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_task tbl_task = db.tbl_task.Find(id);
            if (tbl_task == null)
            {
                return HttpNotFound();
            }
            ViewBag.projectid = new SelectList(db.tbl_project, "projectid", "projectname", tbl_task.projectid);
            ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username", tbl_task.assignedto);
            ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename", tbl_task.tasktypeid);
            return View(tbl_task);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "taskid,taskname,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,projectid,priority,tasktypeid,assignedto")] tbl_task tbl_task)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_task).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.projectid = new SelectList(db.tbl_project, "projectid", "projectname", tbl_task.projectid);
            ViewBag.assignedto = new SelectList(db.tbl_user, "userid", "username", tbl_task.assignedto);
            ViewBag.tasktypeid = new SelectList(db.tbl_tasktype, "tasktypeid", "tasktypename", tbl_task.tasktypeid);
            return View(tbl_task);
        }

        // GET: Task/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_task tbl_task = db.tbl_task.Find(id);
            if (tbl_task == null)
            {
                return HttpNotFound();
            }
            return View(tbl_task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_task tbl_task = db.tbl_task.Find(id);
            db.tbl_task.Remove(tbl_task);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [SessionTimeout]
        [Authorize]
        [HttpPost]
        public ActionResult GetTask(int userid = 0, int roleid = 0, int? milestoneid = 0)
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

                //if (projectid == 0)
                //{
                //    ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                //    Session["ProjectId"] = ViewBag.projectid;
                //    projectid = Convert.ToInt32(Session["ProjectId"]);
                //}
                //else
                //{
                //    ViewBag.projectid = projectid;
                //    Session["ProjectId"] = projectid;
                //}

                var files = entities.sp_FetchTasks(userid, roleid, milestoneid, skip, pageSize, sortColumn, sortColumnDir).ToList();

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
