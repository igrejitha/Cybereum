<<<<<<< Updated upstream
//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tbl_user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_user()
        {
            this.tbl_milestone = new HashSet<tbl_milestone>();
            this.tbl_project = new HashSet<tbl_project>();
            this.tbl_project1 = new HashSet<tbl_project>();
            this.tbl_subtask = new HashSet<tbl_subtask>();
            this.tbl_task = new HashSet<tbl_task>();
            this.tbl_tasktype = new HashSet<tbl_tasktype>();
            this.tbl_user1 = new HashSet<tbl_user>();
        }
    
        public int userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int roleid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Nullable<int> isactive { get; set; }
        public string emailid { get; set; }
        public string organization { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<bool> emailverification { get; set; }
        public string otp { get; set; }
        public string activationcode { get; set; }
        public Nullable<int> pmuserid { get; set; }
        public string GUID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_milestone> tbl_milestone { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_project> tbl_project { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_project> tbl_project1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_subtask> tbl_subtask { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_task> tbl_task { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_tasktype> tbl_tasktype { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_user> tbl_user1 { get; set; }
        public virtual tbl_user tbl_user2 { get; set; }
        public virtual tbl_userrole tbl_userrole { get; set; }
    }
}
=======
//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class tbl_user
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_user()
        {
            this.tbl_user1 = new HashSet<tbl_user>();
            this.tbl_tasktype = new HashSet<tbl_tasktype>();
        }
    
        public int userid { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public int roleid { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public Nullable<int> isactive { get; set; }
        public string emailid { get; set; }
        public string organization { get; set; }
        public Nullable<System.DateTime> createddate { get; set; }
        public Nullable<bool> emailverification { get; set; }
        public string otp { get; set; }
        public string activationcode { get; set; }
        public Nullable<int> pmuserid { get; set; }
        public string GUID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_user> tbl_user1 { get; set; }
        public virtual tbl_user tbl_user2 { get; set; }
        public virtual tbl_userrole tbl_userrole { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_tasktype> tbl_tasktype { get; set; }
    }
}
>>>>>>> Stashed changes
