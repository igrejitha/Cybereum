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
    
    public partial class tbl_status
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tbl_status()
        {
            this.tbl_subtask = new HashSet<tbl_subtask>();
        }
    
        public int statusid { get; set; }
        public string statusname { get; set; }
        public int isactive { get; set; }
        public int createdby { get; set; }
        public System.DateTime createdon { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifiedon { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tbl_subtask> tbl_subtask { get; set; }
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
    
    public partial class tbl_status
    {
        public int statusid { get; set; }
        public string statusname { get; set; }
        public int isactive { get; set; }
        public int createdby { get; set; }
        public System.DateTime createdon { get; set; }
        public Nullable<int> modifiedby { get; set; }
        public Nullable<System.DateTime> modifiedon { get; set; }
    }
}
>>>>>>> Stashed changes
