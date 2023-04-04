﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cybereum.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class cybereumEntities : DbContext
    {
        public cybereumEntities()
            : base("name=cybereumEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tbl_milestone> tbl_milestone { get; set; }
        public virtual DbSet<tbl_project> tbl_project { get; set; }
        public virtual DbSet<tbl_projectdet> tbl_projectdet { get; set; }
        public virtual DbSet<tbl_status> tbl_status { get; set; }
        public virtual DbSet<tbl_subtask> tbl_subtask { get; set; }
        public virtual DbSet<tbl_task> tbl_task { get; set; }
        public virtual DbSet<tbl_tasktype> tbl_tasktype { get; set; }
        public virtual DbSet<tbl_user> tbl_user { get; set; }
        public virtual DbSet<tbl_userrole> tbl_userrole { get; set; }
    
        public virtual ObjectResult<sp_FetchApprovedUsers_Result> sp_FetchApprovedUsers(Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchApprovedUsers_Result>("sp_FetchApprovedUsers", p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<sp_FetchLoginDetails_Result> sp_FetchLoginDetails(string p_email, string p_password)
        {
            var p_emailParameter = p_email != null ?
                new ObjectParameter("P_email", p_email) :
                new ObjectParameter("P_email", typeof(string));
    
            var p_passwordParameter = p_password != null ?
                new ObjectParameter("P_password", p_password) :
                new ObjectParameter("P_password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchLoginDetails_Result>("sp_FetchLoginDetails", p_emailParameter, p_passwordParameter);
        }
    
        public virtual ObjectResult<sp_FetchPendingUsers_Result> sp_FetchPendingUsers(Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchPendingUsers_Result>("sp_FetchPendingUsers", p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<sp_FetchProjects_Result> sp_FetchProjects(Nullable<int> p_PMid, Nullable<int> p_Roleid, Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PMidParameter = p_PMid.HasValue ?
                new ObjectParameter("P_PMid", p_PMid) :
                new ObjectParameter("P_PMid", typeof(int));
    
            var p_RoleidParameter = p_Roleid.HasValue ?
                new ObjectParameter("P_Roleid", p_Roleid) :
                new ObjectParameter("P_Roleid", typeof(int));
    
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchProjects_Result>("sp_FetchProjects", p_PMidParameter, p_RoleidParameter, p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<sp_FetchUsers_Result> sp_FetchUsers(Nullable<int> p_PMid, Nullable<int> p_Roleid, Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PMidParameter = p_PMid.HasValue ?
                new ObjectParameter("P_PMid", p_PMid) :
                new ObjectParameter("P_PMid", typeof(int));
    
            var p_RoleidParameter = p_Roleid.HasValue ?
                new ObjectParameter("P_Roleid", p_Roleid) :
                new ObjectParameter("P_Roleid", typeof(int));
    
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchUsers_Result>("sp_FetchUsers", p_PMidParameter, p_RoleidParameter, p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<Nullable<short>> sp_FetchProjectExists(string p_projectname)
        {
            var p_projectnameParameter = p_projectname != null ?
                new ObjectParameter("P_projectname", p_projectname) :
                new ObjectParameter("P_projectname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<short>>("sp_FetchProjectExists", p_projectnameParameter);
        }
    
        public virtual ObjectResult<Nullable<short>> sp_FetchTaskExists(string p_taskname)
        {
            var p_tasknameParameter = p_taskname != null ?
                new ObjectParameter("P_taskname", p_taskname) :
                new ObjectParameter("P_taskname", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<short>>("sp_FetchTaskExists", p_tasknameParameter);
        }
    
        public virtual ObjectResult<Nullable<short>> sp_FetchUserExists(string p_email)
        {
            var p_emailParameter = p_email != null ?
                new ObjectParameter("P_email", p_email) :
                new ObjectParameter("P_email", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<short>>("sp_FetchUserExists", p_emailParameter);
        }
    
        public virtual ObjectResult<sp_FetchTasks_Result> sp_FetchTasks(Nullable<int> p_PMid, Nullable<int> p_Roleid, Nullable<int> p_milestoneid, Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PMidParameter = p_PMid.HasValue ?
                new ObjectParameter("P_PMid", p_PMid) :
                new ObjectParameter("P_PMid", typeof(int));
    
            var p_RoleidParameter = p_Roleid.HasValue ?
                new ObjectParameter("P_Roleid", p_Roleid) :
                new ObjectParameter("P_Roleid", typeof(int));
    
            var p_milestoneidParameter = p_milestoneid.HasValue ?
                new ObjectParameter("P_milestoneid", p_milestoneid) :
                new ObjectParameter("P_milestoneid", typeof(int));
    
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchTasks_Result>("sp_FetchTasks", p_PMidParameter, p_RoleidParameter, p_milestoneidParameter, p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<sp_FetchMilestones_Result> sp_FetchMilestones(Nullable<int> p_PMid, Nullable<int> p_Roleid, Nullable<int> p_Projectid, Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PMidParameter = p_PMid.HasValue ?
                new ObjectParameter("P_PMid", p_PMid) :
                new ObjectParameter("P_PMid", typeof(int));
    
            var p_RoleidParameter = p_Roleid.HasValue ?
                new ObjectParameter("P_Roleid", p_Roleid) :
                new ObjectParameter("P_Roleid", typeof(int));
    
            var p_ProjectidParameter = p_Projectid.HasValue ?
                new ObjectParameter("P_Projectid", p_Projectid) :
                new ObjectParameter("P_Projectid", typeof(int));
    
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchMilestones_Result>("sp_FetchMilestones", p_PMidParameter, p_RoleidParameter, p_ProjectidParameter, p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<sp_FetchSubTasks_Result> sp_FetchSubTasks(Nullable<int> p_PMid, Nullable<int> p_Roleid, Nullable<int> p_taskid, Nullable<int> p_Pageno, Nullable<int> p_pagesize, string p_SortColumn, string p_SortOrder)
        {
            var p_PMidParameter = p_PMid.HasValue ?
                new ObjectParameter("P_PMid", p_PMid) :
                new ObjectParameter("P_PMid", typeof(int));
    
            var p_RoleidParameter = p_Roleid.HasValue ?
                new ObjectParameter("P_Roleid", p_Roleid) :
                new ObjectParameter("P_Roleid", typeof(int));
    
            var p_taskidParameter = p_taskid.HasValue ?
                new ObjectParameter("P_taskid", p_taskid) :
                new ObjectParameter("P_taskid", typeof(int));
    
            var p_PagenoParameter = p_Pageno.HasValue ?
                new ObjectParameter("P_Pageno", p_Pageno) :
                new ObjectParameter("P_Pageno", typeof(int));
    
            var p_pagesizeParameter = p_pagesize.HasValue ?
                new ObjectParameter("P_pagesize", p_pagesize) :
                new ObjectParameter("P_pagesize", typeof(int));
    
            var p_SortColumnParameter = p_SortColumn != null ?
                new ObjectParameter("P_SortColumn", p_SortColumn) :
                new ObjectParameter("P_SortColumn", typeof(string));
    
            var p_SortOrderParameter = p_SortOrder != null ?
                new ObjectParameter("P_SortOrder", p_SortOrder) :
                new ObjectParameter("P_SortOrder", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_FetchSubTasks_Result>("sp_FetchSubTasks", p_PMidParameter, p_RoleidParameter, p_taskidParameter, p_PagenoParameter, p_pagesizeParameter, p_SortColumnParameter, p_SortOrderParameter);
        }
    
        public virtual ObjectResult<sp_getganttchart_Result> sp_getganttchart(Nullable<int> p_roleid, Nullable<int> p_userid)
        {
            var p_roleidParameter = p_roleid.HasValue ?
                new ObjectParameter("p_roleid", p_roleid) :
                new ObjectParameter("p_roleid", typeof(int));
    
            var p_useridParameter = p_userid.HasValue ?
                new ObjectParameter("p_userid", p_userid) :
                new ObjectParameter("p_userid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_getganttchart_Result>("sp_getganttchart", p_roleidParameter, p_useridParameter);
        }
    
        public virtual ObjectResult<sp_get_project_Result> sp_get_project(Nullable<int> p_roleid, Nullable<int> p_userid, Nullable<int> p_id)
        {
            var p_roleidParameter = p_roleid.HasValue ?
                new ObjectParameter("p_roleid", p_roleid) :
                new ObjectParameter("p_roleid", typeof(int));
    
            var p_useridParameter = p_userid.HasValue ?
                new ObjectParameter("p_userid", p_userid) :
                new ObjectParameter("p_userid", typeof(int));
    
            var p_idParameter = p_id.HasValue ?
                new ObjectParameter("p_id", p_id) :
                new ObjectParameter("p_id", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_get_project_Result>("sp_get_project", p_roleidParameter, p_useridParameter, p_idParameter);
        }
    
        public virtual ObjectResult<sp_get_milestone_Result> sp_get_milestone(Nullable<int> p_roleid, Nullable<int> p_userid, Nullable<int> p_id, Nullable<int> p_rowno)
        {
            var p_roleidParameter = p_roleid.HasValue ?
                new ObjectParameter("p_roleid", p_roleid) :
                new ObjectParameter("p_roleid", typeof(int));
    
            var p_useridParameter = p_userid.HasValue ?
                new ObjectParameter("p_userid", p_userid) :
                new ObjectParameter("p_userid", typeof(int));
    
            var p_idParameter = p_id.HasValue ?
                new ObjectParameter("p_id", p_id) :
                new ObjectParameter("p_id", typeof(int));
    
            var p_rownoParameter = p_rowno.HasValue ?
                new ObjectParameter("p_rowno", p_rowno) :
                new ObjectParameter("p_rowno", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_get_milestone_Result>("sp_get_milestone", p_roleidParameter, p_useridParameter, p_idParameter, p_rownoParameter);
        }
    
        public virtual ObjectResult<sp_get_task_Result> sp_get_task(Nullable<int> p_roleid, Nullable<int> p_userid, Nullable<int> p_id, Nullable<int> p_rowno)
        {
            var p_roleidParameter = p_roleid.HasValue ?
                new ObjectParameter("p_roleid", p_roleid) :
                new ObjectParameter("p_roleid", typeof(int));
    
            var p_useridParameter = p_userid.HasValue ?
                new ObjectParameter("p_userid", p_userid) :
                new ObjectParameter("p_userid", typeof(int));
    
            var p_idParameter = p_id.HasValue ?
                new ObjectParameter("p_id", p_id) :
                new ObjectParameter("p_id", typeof(int));
    
            var p_rownoParameter = p_rowno.HasValue ?
                new ObjectParameter("p_rowno", p_rowno) :
                new ObjectParameter("p_rowno", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_get_task_Result>("sp_get_task", p_roleidParameter, p_useridParameter, p_idParameter, p_rownoParameter);
        }
    
        public virtual ObjectResult<sp_get_subtask_Result> sp_get_subtask(Nullable<int> p_roleid, Nullable<int> p_userid, Nullable<int> p_id, Nullable<int> p_rowno)
        {
            var p_roleidParameter = p_roleid.HasValue ?
                new ObjectParameter("p_roleid", p_roleid) :
                new ObjectParameter("p_roleid", typeof(int));
    
            var p_useridParameter = p_userid.HasValue ?
                new ObjectParameter("p_userid", p_userid) :
                new ObjectParameter("p_userid", typeof(int));
    
            var p_idParameter = p_id.HasValue ?
                new ObjectParameter("p_id", p_id) :
                new ObjectParameter("p_id", typeof(int));
    
            var p_rownoParameter = p_rowno.HasValue ?
                new ObjectParameter("p_rowno", p_rowno) :
                new ObjectParameter("p_rowno", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_get_subtask_Result>("sp_get_subtask", p_roleidParameter, p_useridParameter, p_idParameter, p_rownoParameter);
        }
    }
}
