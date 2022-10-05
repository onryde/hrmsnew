using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppCompany
    {
        public AppCompany()
        {
            Announcement = new HashSet<Announcement>();
            Appraisal = new HashSet<Appraisal>();
            AppraisalAnswer = new HashSet<AppraisalAnswer>();
            AppraisalBusinessNeed = new HashSet<AppraisalBusinessNeed>();
            AppraisalEmployee = new HashSet<AppraisalEmployee>();
            AppraisalFeedback = new HashSet<AppraisalFeedback>();
            AppraisalQuestion = new HashSet<AppraisalQuestion>();
            AppraisalSelfAnswer = new HashSet<AppraisalSelfAnswer>();
            AppraisalTraining = new HashSet<AppraisalTraining>();
            EmployeeAsset = new HashSet<EmployeeAsset>();
            EmployeeBank = new HashSet<EmployeeBank>();
            EmployeeCompany = new HashSet<EmployeeCompany>();
            EmployeeCareer = new HashSet<EmployeeCareer>();
            EmployeeCompensation = new HashSet<EmployeeCompensation>();
            EmployeeContact = new HashSet<EmployeeContact>();
            EmployeeDataVerification = new HashSet<EmployeeDataVerification>();
            EmployeeDocument = new HashSet<EmployeeDocument>();
            EmployeeEducation = new HashSet<EmployeeEducation>();
            EmployeeEmergencyContact = new HashSet<EmployeeEmergencyContact>();
            EmployeeExit = new HashSet<EmployeeExit>();
            EmployeeFamily = new HashSet<EmployeeFamily>();
            EmployeeLanguage = new HashSet<EmployeeLanguage>();
            EmployeePersonal = new HashSet<EmployeePersonal>();
            EmployeePreviousCompany = new HashSet<EmployeePreviousCompany>();
            EmployeeReference = new HashSet<EmployeeReference>();
            EmployeeStatutory = new HashSet<EmployeeStatutory>();
            SettingsAnnouncementType = new HashSet<SettingsAnnouncementType>();
            SettingsAppraisalMode = new HashSet<SettingsAppraisalMode>();
            SettingsAppraisalQuestion = new HashSet<SettingsAppraisalQuestion>();
            SettingsAppraisalRatings = new HashSet<SettingsAppraisalRatings>();
            SettingsAppraiseeType = new HashSet<SettingsAppraiseeType>();
            SettingsAssetTypes = new HashSet<SettingsAssetTypes>();
            SettingsCategory = new HashSet<SettingsCategory>();
            SettingsDepartment = new HashSet<SettingsDepartment>();
            SettingsDepartmentDesignation = new HashSet<SettingsDepartmentDesignation>();
            SettingsDocumentType = new HashSet<SettingsDocumentType>();
            SettingsExitQuestion = new HashSet<SettingsExitQuestion>();
            SettingsGrade = new HashSet<SettingsGrade>();
            SettingsHoliday = new HashSet<SettingsHoliday>();
            SettingsHolidayLocation = new HashSet<SettingsHolidayLocation>();
            SettingsLocation = new HashSet<SettingsLocation>();
            SettingsProductLine = new HashSet<SettingsProductLine>();
            SettingsRegion = new HashSet<SettingsRegion>();
            SettingsReport = new HashSet<SettingsReport>();
            SettingsReportInputs = new HashSet<SettingsReportInputs>();
            SettingsRole = new HashSet<SettingsRole>();
            SettingsModuleAccess = new HashSet<SettingsModuleAccess>();
            SettingsRoleActionAccess = new HashSet<SettingsRoleActionAccess>();
            SettingsTeam = new HashSet<SettingsTeam>();
            SettingsTicketCategory = new HashSet<SettingsTicketCategory>();
            SettingsTicketCategoryOwner = new HashSet<SettingsTicketCategoryOwner>();
            SettingsTicketFaq = new HashSet<SettingsTicketFaq>();
            SettingsTicketStatus = new HashSet<SettingsTicketStatus>();
            SettingsTicketSubCategory = new HashSet<SettingsTicketSubCategory>();
            SettingsTraining = new HashSet<SettingsTraining>();
            SettingsTrainingGrade = new HashSet<SettingsTrainingGrade>();
            Task = new HashSet<Task>();
            TaskComment = new HashSet<TaskComment>();
            Ticket = new HashSet<Ticket>();
            TicketComment = new HashSet<TicketComment>();
            TicketStatus = new HashSet<TicketStatus>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("address")]
        [StringLength(100)]
        public string Address { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Column("phone")]
        [StringLength(15)]
        public string Phone { get; set; }
        [Column("gstNumber")]
        [StringLength(100)]
        public string GstNumber { get; set; }
        [Column("panNumber")]
        [StringLength(100)]
        public string PanNumber { get; set; }
        [Column("fullLogo")]
        [StringLength(1000)]
        public string FullLogo { get; set; }
        [Column("smallLogo")]
        [StringLength(1000)]
        public string SmallLogo { get; set; }
        [Column("alternateLogo")]
        [StringLength(1000)]
        public string AlternateLogo { get; set; }

        [InverseProperty("Company")]
        public virtual ICollection<Announcement> Announcement { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<Appraisal> Appraisal { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalAnswer> AppraisalAnswer { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalBusinessNeed> AppraisalBusinessNeed { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployee { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedback { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalQuestion> AppraisalQuestion { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalSelfAnswer> AppraisalSelfAnswer { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<AppraisalTraining> AppraisalTraining { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeAsset> EmployeeAsset { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeBank> EmployeeBank { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeCareer> EmployeeCareer { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeCompany> EmployeeCompany { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensation { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeContact> EmployeeContact { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeDataVerification> EmployeeDataVerification { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeDocument> EmployeeDocument { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeEducation> EmployeeEducation { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeEmergencyContact> EmployeeEmergencyContact { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeExit> EmployeeExit { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeFamily> EmployeeFamily { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeLanguage> EmployeeLanguage { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeePersonal> EmployeePersonal { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeePreviousCompany> EmployeePreviousCompany { get; set; }
        [InverseProperty("CompanyNavigation")]
        public virtual ICollection<EmployeeReference> EmployeeReference { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<EmployeeStatutory> EmployeeStatutory { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsAnnouncementType> SettingsAnnouncementType { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsAppraisalMode> SettingsAppraisalMode { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsAppraisalQuestion> SettingsAppraisalQuestion { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsAppraisalRatings> SettingsAppraisalRatings { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsAppraiseeType> SettingsAppraiseeType { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsAssetTypes> SettingsAssetTypes { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsCategory> SettingsCategory { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsDepartment> SettingsDepartment { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsDepartmentDesignation> SettingsDepartmentDesignation { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsDocumentType> SettingsDocumentType { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsExitQuestion> SettingsExitQuestion { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsGrade> SettingsGrade { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsHoliday> SettingsHoliday { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsHolidayLocation> SettingsHolidayLocation { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsLocation> SettingsLocation { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsProductLine> SettingsProductLine { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsRegion> SettingsRegion { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsReport> SettingsReport { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsReportInputs> SettingsReportInputs { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsRole> SettingsRole { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsModuleAccess> SettingsModuleAccess { get; set; }
        
        [InverseProperty("Company")]
        public virtual ICollection<SettingsRoleActionAccess> SettingsRoleActionAccess { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTeam> SettingsTeam { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTicketCategory> SettingsTicketCategory { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTicketCategoryOwner> SettingsTicketCategoryOwner { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTicketFaq> SettingsTicketFaq { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTicketStatus> SettingsTicketStatus { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTicketSubCategory> SettingsTicketSubCategory { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTraining> SettingsTraining { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<SettingsTrainingGrade> SettingsTrainingGrade { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<Task> Task { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<TaskComment> TaskComment { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<Ticket> Ticket { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<TicketComment> TicketComment { get; set; }
        [InverseProperty("Company")]
        public virtual ICollection<TicketStatus> TicketStatus { get; set; }
    }
}
