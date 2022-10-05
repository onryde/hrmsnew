using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeContact
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("contactNumber")]
        [StringLength(100)]
        public string ContactNumber { get; set; }
        [Column("alternateNumber")]
        [StringLength(100)]
        public string AlternateNumber { get; set; }
        [Column("officialNumber")]
        [StringLength(100)]
        public string OfficialNumber { get; set; }
        [Column("personalEmailId")]
        [StringLength(100)]
        public string PersonalEmailId { get; set; }
        [Column("officialEmailId")]
        [StringLength(100)]
        public string OfficialEmailId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("permanentAddressSame")]
        public bool? PermanentAddressSame { get; set; }
        [Column("presentAddressId")]
        public long? PresentAddressId { get; set; }
        [Column("permanentAddressId")]
        public long? PermanentAddressId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeContactAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeContact")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeContactEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("PermanentAddressId")]
        [InverseProperty("EmployeeContactPermanentAddress")]
        public virtual EmployeeAddress PermanentAddress { get; set; }
        [ForeignKey("PresentAddressId")]
        [InverseProperty("EmployeeContactPresentAddress")]
        public virtual EmployeeAddress PresentAddress { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeContactUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
