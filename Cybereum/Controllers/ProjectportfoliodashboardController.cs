using Cybereum.Filters;
using Cybereum.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cybereum.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class ProjectportfoliodashboardController : Controller
    {
        // GET: projectportfoliodashboard
        public ActionResult Projectportfoliodashboard(string projectid)
        {
            return View();
        }
        public ActionResult masterdashboard(string projectid)
        {
            ViewBag.projectid = projectid;
            Session["ProjectId"] = ViewBag.projectid;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            return View();
        }
        public ActionResult projectdashboard()
        {
=======
=======
>>>>>>> Stashed changes
            ViewBag.Keyinternal = getkeyinternal(projectid);
            return View();
        }
        public ActionResult projectdashboard(string projectid)
        {
            ViewBag.projectid = projectid;
            Session["ProjectId"] = ViewBag.projectid;
            ViewBag.TotalParticipant = getparticipant(projectid);
            ViewBag.TotalMilestone = getTotalMilestone(projectid, true);
            ViewBag.TotalActivity = getTotalMilestone(projectid, false);
            ViewBag.TotalMilestoneacheived = getTotalMilestoneacheived(projectid, true);
            ViewBag.TotalActivityCompleted = getTotalMilestoneacheived(projectid, false);
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            return View();
        }
        public ActionResult index(string projectid)
        {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
            ViewBag.projectid = projectid;
            Session["ProjectId"] = ViewBag.projectid;
            ViewBag.Keyinternal = getkeyinternal(projectid);
            ViewBag.Taskcompletion = GettaskcompletionData(projectid);
=======
=======
>>>>>>> Stashed changes
            ViewBag.Projectid = projectid;
            Session["ProjectId"] = ViewBag.Projectid;
            ViewBag.Keyinternal = getkeyinternal(projectid);
            //ViewBag.Taskcompletion = GettaskcompletionData(projectid);
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            return View();
        }
        public ActionResult ProjectOverview()
        {
            return View();
        }
        public ActionResult TeamHub()
        {
            return View();
        }


        #region project dashboard

        public List<tblkeyinternal> getkeyinternal(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').order().by('startdate',incr).order().by('enddate',incr).project('activityname','progress').by(values('activityname')).by(values('progress'))";
                var keyinternaldata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                string pList = JsonConvert.SerializeObject(keyinternaldata);
                List<tblkeyinternal> keyinternallist = JsonConvert.DeserializeObject<List<tblkeyinternal>>(pList);
                return keyinternallist;
                //return new JsonResult { Data = keyinternallist, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }

        }

<<<<<<< Updated upstream
<<<<<<< Updated upstream
        public string GettaskcompletionData(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').project('id').by(values('id'))";
=======
=======
>>>>>>> Stashed changes
        public string getparticipant(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('project','id','{projectid}').project('projectmembers').by(values('projectmembers').fold())";
                var data = IGUtilities.ExecuteGremlinScript(gremlinScript);
                int membercount = 0;
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                string pList = JsonConvert.SerializeObject(results);
                List<Project> projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);
                foreach (var item in projectlist)
                {
                    var projectmembers = item.projectmembers.Count();
                    membercount = projectmembers.ToInt();
                }
                membercount++;
                return membercount.ToString();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }

        public string getTotalMilestone(string projectid, bool ismilestone)
        {
            try
            {
                string gremlinScript = "";
                if (ismilestone == true)
                {
                    gremlinScript = $"g.V().has('activity','projectid','{projectid}').has('activity','ismilestone','{ismilestone}').count()";
                }
                else
                {
                    gremlinScript = $"g.V().has('activity','projectid','{projectid}').count()";
                }
                var data = IGUtilities.ExecuteGremlinScript(gremlinScript);
                int activitycount = data.Count();
                foreach (var item in data)
                {
                    var projectmembers =(int) item;
                    activitycount = projectmembers.ToInt();
                }
                
                return activitycount.ToString();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }

        public string getTotalMilestoneacheived(string projectid, bool ismilestone)
        {
            try
            {
                string gremlinScript = "";
                if (ismilestone == true)
                {
                    gremlinScript = $"g.V().has('activity','projectid','{projectid}').has('activity','ismilestone','{ismilestone}').has('activity','progress','100').count()";
                }
                else
                {
                    gremlinScript = $"g.V().has('activity','projectid','{projectid}').has('activity','progress','100').count()";
                }
                var data = IGUtilities.ExecuteGremlinScript(gremlinScript);
                int activitycount = data.Count();
                foreach (var item in data)
                {
                    var projectmembers = (int)item;
                    activitycount = projectmembers.ToInt();
                }

                return activitycount.ToString();
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }

        public JsonResult GettaskcompletionData(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').project('id').by(id())";
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                List<tbltaskcompletion> taskcompletionlist = new List<tbltaskcompletion>();
                foreach (var result in activitydata)
                {
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                    gremlinScript = $"g.V().has('task','activityid','{result["id"]}').project('taskstatus').by(values('taskstatus'))";
                    var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    foreach (var item in res)
                    {
                        if (item.Count > 0)
                        {                            
                            tbltaskcompletion task = new tbltaskcompletion();
                            if (Convert.ToInt16(item["taskstatus"]) == (int)TaskSubTaskStatus.ToDo)
                            {
                                task.status = TaskSubTaskStatus.ToDo.ToString();
                            }
                            else if (Convert.ToInt16(item["taskstatus"]) == (int)TaskSubTaskStatus.Inprogress)
                            {
                                task.status = TaskSubTaskStatus.Inprogress.ToString();
                            }
                            else if (Convert.ToInt16(item["taskstatus"]) == (int)TaskSubTaskStatus.Completed)
                            {
                                task.status = TaskSubTaskStatus.Completed.ToString();
                            }
                            task.taskcount = 1;
=======
=======
>>>>>>> Stashed changes
                    foreach (var enumValue in Enum.GetValues(typeof(TaskSubTaskStatus)))
                    {
                        int taskstatus = (int)enumValue;
                        gremlinScript = $"g.V().has('task','activityid','{result["id"]}').has('task','taskstatus','{taskstatus}').count()";
                        var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        foreach (var item in res)
                        {
                            tbltaskcompletion task = new tbltaskcompletion();
                            task.label = enumValue.ToString();
                            task.value = (int)item;
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
                            taskcompletionlist.Add(task);
                        }
                    }
                }
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                var numberGroups = taskcompletionlist.GroupBy(n => n.status).Select(c => new { Key = c.Key, total = c.Count() });
=======
                var numberGroups = taskcompletionlist.GroupBy(t => t.label).Select(t => new { Key = t.Key, total = t.Sum(u => u.value) }).ToList();
>>>>>>> Stashed changes
=======
                var numberGroups = taskcompletionlist.GroupBy(t => t.label).Select(t => new { Key = t.Key, total = t.Sum(u => u.value) }).ToList();
>>>>>>> Stashed changes
                taskcompletionlist = new List<tbltaskcompletion>();
                foreach (var grp in numberGroups)
                {
                    tbltaskcompletion task = new tbltaskcompletion();
<<<<<<< Updated upstream
<<<<<<< Updated upstream
                    task.status = grp.Key;
                    task.taskcount = grp.total;
                    taskcompletionlist.Add(task);
                }

                //return taskcompletionlist;                
                string pList = JsonConvert.SerializeObject(taskcompletionlist);
                return pList;
                //List<tbltaskcompletion> list = JsonConvert.DeserializeObject<List<tbltaskcompletion>>(pList);
                //return new JsonResult { Data = list, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
=======
=======
>>>>>>> Stashed changes
                    task.label = grp.Key;
                    task.value = grp.total;
                    taskcompletionlist.Add(task);
                }

                string pList = JsonConvert.SerializeObject(taskcompletionlist);
                return new JsonResult { Data = pList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
<<<<<<< Updated upstream
>>>>>>> Stashed changes
=======
>>>>>>> Stashed changes
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }

        }

        #endregion
    }
}