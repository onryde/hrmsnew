using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeCompensation
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("year")]
        public int Year { get; set; }
        [Column("annualBasic")]
        public double AnnualBasic { get; set; }
        [Column("annualHra")]
        public double AnnualHra { get; set; }
        [Column("annualConvAllow")]
        public double AnnualConvAllow { get; set; }
        [Column("annualSplAllow")]
        public double AnnualSplAllow { get; set; }
        [Column("annualMedAllow")]
        public double AnnualMedAllow { get; set; }
        [Column("annualLta")]
        public double AnnualLta { get; set; }
        [Column("annualWashing")]
        public double AnnualWashing { get; set; }
        [Column("annualChildEdu")]
        public double AnnualChildEdu { get; set; }
        [Column("annualGross")]
        public double AnnualGross { get; set; }
        [Column("statutoryBonus")]
        public double StatutoryBonus { get; set; }
        [Column("annualVarBonus")]
        public double AnnualVarBonus { get; set; }
        [Column("annualVarBonusPaid1")]
        public double AnnualVarBonusPaid1 { get; set; }
        [Column("annualVarBonusPaid2")]
        public double AnnualVarBonusPaid2 { get; set; }
        [Column("annualAccidIns")]
        public double AnnualAccidIns { get; set; }
        [Column("annualHealthIns")]
        public double AnnualHealthIns { get; set; }
        [Column("annualGratuity")]
        public double AnnualGratuity { get; set; }
        [Column("annualPF")]
        public double AnnualPf { get; set; }
        [Column("annualEsi")]
        public double AnnualEsi { get; set; }
        [Column("annualCtc")]
        public double AnnualCtc { get; set; }
        [Column("vendorCharges")]
        public double VendorCharges { get; set; }
        [Column("offrollCtc")]
        public double OffrollCtc { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("otherBenefits")]
        public double OtherBenefits { get; set; }
        [Column("gradeId")]
        public long? GradeId { get; set; }
        [Column("departmentId")]
        public long? DepartmentId { get; set; }
        [Column("designationId")]
        public long? DesignationId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeCompensationAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeCompensation")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("EmployeeCompensation")]
        public virtual SettingsDepartment Department { get; set; }
        [ForeignKey("DesignationId")]
        [InverseProperty("EmployeeCompensation")]
        public virtual SettingsDepartmentDesignation Designation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeCompensationEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("GradeId")]
        [InverseProperty("EmployeeCompensation")]
        public virtual SettingsGrade Grade { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeCompensationUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
