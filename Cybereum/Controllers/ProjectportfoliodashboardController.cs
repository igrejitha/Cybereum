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
            ViewBag.Keyinternal = getkeyinternal(projectid);
            return View();
        }
        public ActionResult projectdashboard(string projectid)
        {
            ViewBag.projectid = projectid;
            Session["ProjectId"] = ViewBag.projectid;
            //ViewBag.TotalParticipant = getparticipant(projectid);
            ViewBag.TotalMilestone = getTotalMilestone(projectid, true);
            ViewBag.TotalActivity = getTotalMilestone(projectid, false);
            ViewBag.TotalMilestoneacheived = getTotalMilestoneacheived(projectid, true);
            ViewBag.TotalActivityCompleted = getTotalMilestoneacheived(projectid, false);
            ViewBag.TotalMilestonedelayed = getTotalMilestonedelayed(projectid, true);
            ViewBag.TotalActivitydelayed = getTotalMilestonedelayed(projectid, false);
            ViewBag.Projectenddate = getprojectenddate(projectid);
            ViewBag.ActiveMembers = GetActiveMembers(projectid);
            return View();
        }
        public ActionResult index(string projectid)
        {
            ViewBag.Projectid = projectid;
            Session["ProjectId"] = ViewBag.Projectid;
            ViewBag.Keyinternal = getkeyinternal(projectid);
            //ViewBag.Taskcompletion = GettaskcompletionData(projectid);
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

        public string getparticipant(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('project','id','{projectid}').project('projectmembers').by(values('projectmembers').fold())";
                //var data = IGUtilities.ExecuteGremlinScript(gremlinScript);
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

        public string getTotalMilestonedelayed(string projectid, bool ismilestone)
        {
            try
            {
                string gremlinScript = "";
                string date = DateTime.Now.ToString("yyyy-MM-dd");
                if (ismilestone == true)
                {
                    gremlinScript = $"g.V().has('activity','projectid','{projectid}').has('activity','ismilestone','{ismilestone}').not(has('activity','activitystatus','2')).has('activity', 'enddate', lt('{ date }')).count()";
                }
                else
                {
                    gremlinScript = $"g.V().has('activity','projectid','{projectid}').not(has('activity','activitystatus','2')).has('activity', 'enddate', lt('{ date }')).count()";
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

        public string getprojectenddate(string projectid)
        {
            try
            {
                string gremlinScript = $"g.V().has('project','id','{projectid}').project('enddate').by(values('enddate'))";
                var results = IGUtilities.ExecuteGremlinScript(gremlinScript);
                string pList = JsonConvert.SerializeObject(results);
                List<Project> projectlist = JsonConvert.DeserializeObject<List<Project>>(pList);
                string enddate = string.Empty;
                foreach (var item in projectlist)
                {
                    DateTime date = item.enddate;
                    enddate = date.ToString("MMM dd, yyyy");
                }

                return enddate;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return null;
            }
        }

        public string GetActiveMembers(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').project('id').by(id())";
                var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                List<tblactivemembers> tasklist = new List<tblactivemembers>();
                foreach (var result in activitydata)
                {
                    //task users
                    gremlinScript = $"g.V().has('task','activityid','{result["id"]}').project('taskid','assignedto').by(id()).by(values('assignedto'))";
                    var res = IGUtilities.ExecuteGremlinScript(gremlinScript);
                    string pList = JsonConvert.SerializeObject(res);
                    List<ProjectTask> list = JsonConvert.DeserializeObject<List<ProjectTask>>(pList);
                    foreach (var item in list)
                    {
                        tblactivemembers task = new tblactivemembers();
                        task.userid = Convert.ToInt16(item.assignedto.ToString());
                        tasklist.Add(task);

                        //Subtask users
                        gremlinScript = $"g.V().has('subtask','taskid','{item.taskid}').project('assignedto').by(values('assignedto'))";
                        var ressubtask = IGUtilities.ExecuteGremlinScript(gremlinScript);
                        pList = JsonConvert.SerializeObject(ressubtask);
                        List<ProjectSubTask> listsubtask = JsonConvert.DeserializeObject<List<ProjectSubTask>>(pList);
                        foreach (var itemsubtask in listsubtask)
                        {
                            tblactivemembers subtask = new tblactivemembers();
                            subtask.userid = Convert.ToInt16(itemsubtask.assignedto.ToString());
                            tasklist.Add(subtask);
                        }
                    }
                }
                var count = tasklist.Select(x=>x.userid).Distinct().Count();
                count++;
                return count.ToString();
            }
            catch (Exception ex)
            {
                // Info     
                Console.Write(ex);
                return null;
            }
        }

        public JsonResult GettaskcompletionData(string projectid)
        {
            try
            {
                var gremlinScript = $"g.V().has('activity','projectid','{projectid}').project('id').by(id())";
                var activitydata = IGUtilities.ExecuteGremlinScript(gremlinScript);
                List<tbltaskcompletion> taskcompletionlist = new List<tbltaskcompletion>();
                foreach (var result in activitydata)
                {
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
                            taskcompletionlist.Add(task);
                        }
                    }
                }
                var numberGroups = taskcompletionlist.GroupBy(t => t.label).Select(t => new { Key = t.Key, total = t.Sum(u => u.value) }).ToList();
                taskcompletionlist = new List<tbltaskcompletion>();
                foreach (var grp in numberGroups)
                {
                    tbltaskcompletion task = new tbltaskcompletion();
                    task.label = grp.Key;
                    task.value = grp.total;
                    taskcompletionlist.Add(task);
                }

                string pList = JsonConvert.SerializeObject(taskcompletionlist);
                return new JsonResult { Data = pList, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
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