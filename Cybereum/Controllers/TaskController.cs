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
        public ActionResult Index(int projectid)
        {
            //var tbl_task = db.tbl_task.Include(t => t.tbl_project).Include(t => t.tbl_user).Include(t => t.tbl_tasktype);
            //return View(tbl_task.ToList());
            GetTask(Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserId"]), Convert.ToInt32(System.Web.HttpContext.Current.Session["RoleId"]), projectid);
            ViewBag.projectid = projectid;
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

        // GET: Task/Create
        public ViewResult Create(int? taskid, int projectid, TaskViewModel Tasks)
        {
            ViewBag.projectid = projectid;

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
            tasktype.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            ViewBag.tasktypeid = tasktype;

            List<SelectListItem> status = (from b in db.tbl_status
                                           where b.isactive == 1
                                           select new SelectListItem
                                           {
                                               Text = b.statusname,
                                               Value = b.statusid.ToString()
                                           }).Distinct().OrderBy(x => x.Text).ToList();
            status.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            ViewBag.statusid = status;


            ViewBag.projectid = projectid;

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
                                         where (b.pmuserid == pmuserid) && b.isactive == 1
                                         select new SelectListItem
                                         {
                                             Text = b.firstname + " " + b.lastname,
                                             Value = b.userid.ToString()
                                         }).Distinct().OrderBy(x => x.Text).ToList();
            user.Insert(0, new SelectListItem { Text = "<-- Select -->", Value = "" });
            return user;
        }

        [Authorize]
        [SessionTimeout]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "taskid,taskname,isactive,startdate,enddate,statusid,createdby,createddate,modifiedby,modifieddate,projectid,priority,tasktypeid,assignedto")] TaskViewModel tbl_task)
        //public ActionResult AddUsers(FormCollection formval, UserLevelOne _mod)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                int? objList = 0;
                //objList = db.sp_FetchUserExists(logindetails.emailid, logindetails.password).FirstOrDefault();
                if (tbl_task.taskid == 0)
                {
                    objList = db.sp_FetchTaskExists(tbl_task.taskname).FirstOrDefault();                    
                }

                switch (objList)
                {
                    case 1:
                        message = "Task name already exists.";
                        break;
                    default:
                        tbl_task task = new tbl_task();
                        task.taskname = tbl_task.taskname;
                        task.projectid = tbl_task.projectid;                        
                        task.startdate = tbl_task.startdate;
                        task.enddate = tbl_task.enddate;
                        task.priority = tbl_task.priority;
                        task.assignedto = tbl_task.assignedto;
                        task.tasktypeid = tbl_task.tasktypeid;
                        task.statusid = tbl_task.statusid;
                        task.isactive = 1;
                        if (tbl_task.taskid == 0)
                        {
                            task.createddate = DateTime.Now;
                            task.createdby = Convert.ToInt16(Session["LoggedInUserId"]);
                            db.tbl_task.Add(task);
                            db.SaveChanges();
                            message = "Saved Successfully";
                        }
                        else
                        {
                            tbl_task taskedit = db.tbl_task.Find(tbl_task.taskid);

                            task.taskname = tbl_task.taskname;
                            task.projectid = tbl_task.projectid;
                            task.startdate = tbl_task.startdate;
                            task.enddate = tbl_task.enddate;
                            task.priority = tbl_task.priority;
                            task.assignedto = tbl_task.assignedto;
                            task.tasktypeid = tbl_task.tasktypeid;
                            task.statusid = tbl_task.statusid;
                            task.isactive = 1;
                            taskedit.modifieddate = DateTime.Now;
                            taskedit.modifiedby = Convert.ToInt16(Session["LoggedInUserId"]);
                            taskedit.isactive = 1;
                            db.Entry(taskedit).State = EntityState.Modified;
                            db.SaveChanges();
                            message = "Modified Successfully";
                        }

                        return RedirectToAction("Index");
                }
                ViewBag.Message = message;
                return View(tbl_task);
            }
            return View(tbl_task);
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
        public ActionResult GetTask(int userid = 0, int roleid = 0, int projectid = 0)
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

                var files = entities.sp_FetchTasks(userid, roleid, projectid, skip, pageSize, sortColumn, sortColumnDir).ToList();

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
