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
    
    public partial class tbl_tasktype
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_tasktype()
        {
            this.tbl_milestone = new HashSet<tbl_milestone>();
            this.tbl_task = new HashSet<tbl_task>();
        }
    
        public int tasktypeid { get; set; }
        public string tasktypename { get; set; }
        public int isactive { get; set; }
        public int createdby { get; set; }
        public System.DateTime createdon { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifiedon { get; set; }
    
        public virtual tbl_user tbl_user { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_milestone> tbl_milestone { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_task> tbl_task { get; set; }
    }
}
