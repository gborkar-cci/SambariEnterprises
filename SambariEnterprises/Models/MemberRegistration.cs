//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SambariEnterprises.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MemberRegistration
    {
        public long ID { get; set; }
        public System.Guid ResourceID { get; set; }
        public long MemberID { get; set; }
        public Nullable<long> MasterID { get; set; }
        public string PharmacyName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string DrugLicenceNumber { get; set; }
        public string TinNumber { get; set; }
        public string GSTResgistrationNumber { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string OwnerName { get; set; }
        public string ContactPersonName { get; set; }
        public Nullable<System.DateTime> DrugLicenceExpiry { get; set; }
        public string Constitution { get; set; }
        public string PanNumber { get; set; }
        public Nullable<bool> RegisteredForGst { get; set; }
    
        public virtual Member Member { get; set; }
    }
}
