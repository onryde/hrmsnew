using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeCompany
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
        [Column("employeeCode")]
        [StringLength(100)]
        public string EmployeeCode { get; set; }
        [Column("status")]
        [StringLength(100)]
        public string Status { get; set; }
        [Column("addressingName")]
        [StringLength(100)]
        public string AddressingName { get; set; }
        [Column("regionId")]
        public long? RegionId { get; set; }
        [Column("departmentId")]
        public long? DepartmentId { get; set; }
        [Column("teamId")]
        public long? TeamId { get; set; }
        [Column("locationId")]
        public long? LocationId { get; set; }
        [Column("categoryId")]
        public long? CategoryId { get; set; }
        [Column("doj", TypeName = "date")]
        public DateTime? Doj { get; set; }
        [Column("gradeId")]
        public long? GradeId { get; set; }
        [Column("designationId")]
        public long? DesignationId { get; set; }
        [Column("trainingPeriod", TypeName = "date")]
        public DateTime? TrainingPeriod { get; set; }
        [Column("trainingAssessmentDate", TypeName = "date")]
        public DateTime? TrainingAssessmentDate { get; set; }
        [Column("probationPeriod")]
        public int? ProbationPeriod { get; set; }
        [Column("probationExtension")]
        public int? ProbationExtension { get; set; }
        [Column("confirmationRemarks")]
        [StringLength(1000)]
        public string ConfirmationRemarks { get; set; }
        [Column("photo")]
        [StringLength(5000)]
        public string Photo { get; set; }
        [Column("vendorName")]
        [StringLength(100)]
        public string VendorName { get; set; }
        [Column("confirmationStatus")]
        public bool? ConfirmationStatus { get; set; }
        [Column("offRoleCode")]
        [StringLength(100)]
        public string OffRoleCode { get; set; }
        [Column("trainingStartDate", TypeName = "date")]
        public DateTime? TrainingStartDate { get; set; }
        [Column("probationStartDate", TypeName = "date")]
        public DateTime? ProbationStartDate { get; set; }
        [Column("probationEndDate", TypeName = "date")]
        public DateTime? ProbationEndDate { get; set; }
        [Column("confirmedOn", TypeName = "date")]
        public DateTime? ConfirmedOn { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("onRollDate", TypeName = "date")]
        public DateTime? OnRollDate { get; set; }
        [Column("locationBifurcation")]
        [StringLength(100)]
        public string LocationBifurcation { get; set; }
        [Column("vendor")]
        [StringLength(100)]
        public string Vendor { get; set; }
        [Column("reportingToId")]
        public long? ReportingToId { get; set; }
        [Column("productLineId")]
        public long? ProductLineId { get; set; }
        [Column("uniqueCode")]
        [StringLength(100)]
        public string UniqueCode { get; set; }
        [Column("statusCategory")]
        [StringLength(100)]
        public string StatusCategory { get; set; }
        [Column("isResigned")]
        public bool? IsResigned { get; set; }
        [Column("isRehired")]
        public bool? IsRehired { get; set; }
        [Column("locationForField")]
        [StringLength(200)]
        public string LocationForField { get; set; }
        [Column("division")]
        [StringLength(100)]
        public string Division { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeCompanyAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsCategory Category { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeCompany")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("DepartmentId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsDepartment Department { get; set; }
        [ForeignKey("DesignationId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsDepartmentDesignation Designation { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeCompanyEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("GradeId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsGrade Grade { get; set; }
        [ForeignKey("LocationId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsLocation Location { get; set; }
        [ForeignKey("ProductLineId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsProductLine ProductLine { get; set; }
        [ForeignKey("RegionId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsRegion Region { get; set; }
        [ForeignKey("ReportingToId")]
        [InverseProperty("EmployeeCompanyReportingTo")]
        public virtual Employee ReportingTo { get; set; }
        [ForeignKey("TeamId")]
        [InverseProperty("EmployeeCompany")]
        public virtual SettingsTeam Team { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeCompanyUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
