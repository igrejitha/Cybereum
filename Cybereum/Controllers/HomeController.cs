using Cybereum.Filters;
using Cybereum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Cybereum.Controllers
{
    public class HomeController : Controller
    {

        cybereumEntities entities = new cybereumEntities();

        [Authorize]
        [SessionTimeout]
        public ActionResult Index()
        {
            //using (DashboardContext _context = new DashboardContext())
            //{
            //    ViewBag.CountCustomers = _context.CustomerSet.Count();
            //    ViewBag.CountOrders = _context.OrderSet.Count();
            //    ViewBag.CountProducts = _context.ProductSet.Count();
            //}

            var countapproved = (from p in entities.tbl_user
                                 where p.emailverification == true && p.isactive == 1 && p.roleid != 1
                                 select p.userid).Count();

            var countpending = (from p in entities.tbl_user
                                where p.emailverification == true && p.isactive == 0 && p.roleid != 1
                                select p.userid).Count();

            var countproject = (from p in entities.tbl_project
                                where p.isactive == 1
                                select p.projectid).Count();

            ViewBag.countApproved = countapproved;
            ViewBag.countPending = countpending;
            ViewBag.countProject = countproject;

            return View();
        }

        [Authorize]
        [SessionTimeout]
        public ActionResult Dashboard()
        {
            int pmuserid = Convert.ToInt16(Session["LoggedInUserId"]);
            cybereumEntities entities = new cybereumEntities();
            var countUser = (from p in entities.tbl_user
                             where p.emailverification == true && p.isactive == 1 && p.pmuserid == pmuserid && p.roleid == 3
                             select p.userid).Count();

            var countProject = (from p in entities.tbl_project
                                where p.isactive == 1 && p.createdby == pmuserid
                                select p.projectid).Count();

            var countTask = (from p in entities.tbl_task
                             join a in entities.tbl_milestone on p.milestoneid equals a.milestoneid
                             join b in entities.tbl_project on a.projectid equals b.projectid
                             where p.isactive == 1 && b.createdby == pmuserid
                             select p.taskid).Count();

            ViewBag.countUser = countUser;
            ViewBag.countProject = countProject;
            ViewBag.countTask = countTask;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        [SessionTimeout]
        // GET: Project
        public ActionResult GANTT()
        {
            return View();
        }


        [Authorize]
        [SessionTimeout]
        public ActionResult GANTTChart(int? projectid)
        {
            TempData["ProjectId"] = projectid;
            ViewBag.projectid = TempData["ProjectId"];
            TempData.Keep();
            if (projectid == 0)
            {
                ViewBag.projectid = Convert.ToInt32(Session["ProjectId"]);
                Session["ProjectId"] = ViewBag.projectid;
            }
            else
            {
                ViewBag.projectid = projectid;
                Session["ProjectId"] = projectid;
            }
            return View();
        }


        public JsonResult getTestvalue()
        {
            try
            {
                var projectdata = entities.sp_get_project(1, 1, 0).ToList();

                List<TestModel> ganttchartmodellist = new List<TestModel>();
                foreach (var project in projectdata)
                {
                    TestModel ganttchart = new TestModel();
                    ganttchart.task = project.name.ToString();
                    ganttchart.type = project.name.ToString();
                    ganttchart.startTime = project.startdate.ToString("yyyy-MM-dd");
                    ganttchart.endTime = project.enddate.ToString("yyyy-MM-dd");
                    ganttchartmodellist.Add(ganttchart);

                    var milestonedata = entities.sp_get_milestone(1, 1, project.id, 0).ToList();
                    foreach (var itemmilestone in milestonedata)
                    {
                        TestModel ganttchart1 = new TestModel();
                        ganttchart1.task = itemmilestone.name.ToString();
                        ganttchart1.type = project.name.ToString();
                        ganttchart1.startTime = itemmilestone.startdate.ToString("yyyy-MM-dd");
                        ganttchart1.endTime = itemmilestone.enddate.ToString("yyyy-MM-dd");
                        ganttchartmodellist.Add(ganttchart1);

                        var taskdata = entities.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                        foreach (var itemtask in taskdata)
                        {
                            TestModel ganttchart2 = new TestModel();
                            ganttchart2.task = itemtask.name.ToString();
                            ganttchart2.type = itemmilestone.name.ToString();
                            ganttchart2.startTime = itemtask.startdate.ToString("yyyy-MM-dd");
                            ganttchart2.endTime = itemtask.enddate.ToString("yyyy-MM-dd");
                            ganttchartmodellist.Add(ganttchart2);

                            var subtaskdata = entities.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                            foreach (var itemsubtask in subtaskdata)
                            {
                                TestModel ganttchart3 = new TestModel();
                                ganttchart3.task = itemsubtask.name.ToString();
                                ganttchart3.type = itemtask.name.ToString();
                                ganttchart3.startTime = itemsubtask.startdate.ToString("yyyy-MM-dd");
                                ganttchart3.endTime = itemsubtask.enddate.ToString("yyyy-MM-dd");
                                ganttchartmodellist.Add(ganttchart3);
                            }
                        }
                    }                    
                    
                }

                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
                //return Json(ganttchartmodellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        public JsonResult getgannchartvalue(int? projectid)
        {
            try
            {
                var projectdata = entities.sp_get_project(1, 1, projectid).ToList();

                List<ganttchartmodel> ganttchartmodellist = new List<ganttchartmodel>();
                int i = 1;
                foreach (var project in projectdata)
                {
                    ganttchartmodel ganttchart = new ganttchartmodel();
                    ganttchart.id = i.ToString();
                    i++;
                    ganttchart.name = project.name.ToString();
                    ganttchart.progressValue = project.progressValue.ToString() + "%";
                    ganttchart.actualStart = project.startdate.ToString("yyyy-MM-dd");
                    ganttchart.actualEnd = project.enddate.ToString("yyyy-MM-dd");
                    ganttchart.connectTo = i.ToString();
                    ganttchart.connecterType = "finish - start";
                    
                    List<Children> Milestone = new List<Children>();
                    var milestonedata = entities.sp_get_milestone(1, 1, project.id, 0).ToList();
                    foreach (var itemmilestone in milestonedata)
                    {
                        Children objMilestone = new Children();
                        objMilestone.id = i.ToString();
                        i++;
                        objMilestone.name = itemmilestone.name.ToString();
                        objMilestone.progressValue = itemmilestone.progressValue.ToString() + "%";
                        //objMilestone.actualStart = "Date.UTC(" + itemmilestone.startdate.ToString("yyyy,MM,dd") + ")";
                        objMilestone.actualStart = itemmilestone.startdate.ToString("yyyy-MM-dd");
                        objMilestone.actualEnd =  itemmilestone.enddate.ToString("yyyy-MM-dd");
                        objMilestone.connectTo = i.ToString();
                        objMilestone.connecterType = "finish - start";

                        List<Children> Task = new List<Children>();
                        var taskdata = entities.sp_get_task(1, 1, itemmilestone.id, 0).ToList();
                        foreach (var itemtask in taskdata)
                        {
                            Children objTask = new Children();
                            objTask.id = i.ToString();
                            i++;
                            objTask.name = itemtask.name.ToString();
                            objTask.progressValue = itemtask.progressValue.ToString() + "%";
                            objTask.actualStart =  itemtask.startdate.ToString("yyyy-MM-dd");
                            objTask.actualEnd =  itemtask.enddate.ToString("yyyy-MM-dd");
                            objTask.connectTo = i.ToString();
                            objTask.connecterType = "finish - start";

                            List<Children> SubTask = new List<Children>();
                            var subtaskdata = entities.sp_get_subtask(1, 1, itemtask.id, 0).ToList();
                            int subtaskrow = 0;
                            foreach (var itemsubtask in subtaskdata)
                            {
                                subtaskrow ++;
                                Children objSubTask = new Children();
                                objSubTask.id = i.ToString();
                                i++;
                                objSubTask.name = itemsubtask.name.ToString();
                                objSubTask.progressValue = itemsubtask.progressValue.ToString() + "%";
                                objSubTask.actualStart =  itemsubtask.startdate.ToString("yyyy-MM-dd");
                                objSubTask.actualEnd =  itemsubtask.enddate.ToString("yyyy-MM-dd");
                                if (subtaskrow != subtaskdata.Count)
                                {
                                    objSubTask.connectTo = i.ToString();
                                    objSubTask.connecterType = "finish - finish";
                                }
                                

                                SubTask.Add(objSubTask);
                            }

                            if (subtaskdata.Count > 0)
                            {
                                objTask.children = SubTask;
                                Task.Add(objTask);
                            }
                        }

                        if (taskdata.Count > 0)
                        {
                            objMilestone.children = Task;
                            Milestone.Add(objMilestone);
                        }
                    }

                    if (milestonedata.Count > 0)
                    {
                        ganttchart.children = Milestone;
                        ganttchartmodellist.Add(ganttchart);
                    }
                }
                
                var x = JsonConvert.SerializeObject(ganttchartmodellist, Formatting.Indented);
                return Json(x, JsonRequestBehavior.AllowGet);
                //return Json(ganttchartmodellist, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        //protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        ContentType = contentType,
        //        ContentEncoding = contentEncoding,
        //        JsonRequestBehavior = behavior,
        //        MaxJsonLength = Int32.MaxValue
        //    };
        //}
    }
}