using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeStatutory
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
        [Column("panNumber")]
        [StringLength(100)]
        public string PanNumber { get; set; }
        [Column("pfNumber")]
        [StringLength(100)]
        public string PfNumber { get; set; }
        [Column("uanNumber")]
        [StringLength(100)]
        public string UanNumber { get; set; }
        [Column("previousEmpPension")]
        [StringLength(100)]
        public string PreviousEmpPension { get; set; }
        [Column("aadharNumber")]
        [StringLength(100)]
        public string AadharNumber { get; set; }
        [Column("aadharName")]
        [StringLength(100)]
        public string AadharName { get; set; }
        [Column("drivingLicenseNumber")]
        [StringLength(100)]
        public string DrivingLicenseNumber { get; set; }
        [Column("drivingLicenseValidity", TypeName = "date")]
        public DateTime? DrivingLicenseValidity { get; set; }
        [Column("passportNumber")]
        [StringLength(100)]
        public string PassportNumber { get; set; }
        [Column("passportValidity", TypeName = "date")]
        public DateTime? PassportValidity { get; set; }
        [Column("esiNumber")]
        [StringLength(100)]
        public string EsiNumber { get; set; }
        [Column("licIdNumber")]
        [StringLength(100)]
        public string LicIdNumber { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeStatutoryAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeStatutory")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeStatutoryEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeStatutoryUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
