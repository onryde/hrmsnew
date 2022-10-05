using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Employee
    {
        public Employee()
        {
            AnnouncementAddedByNavigation = new HashSet<Announcement>();
            AnnouncementAttachmentAddedByNavigation = new HashSet<AnnouncementAttachment>();
            AnnouncementAttachmentUpdatedByNavigation = new HashSet<AnnouncementAttachment>();
            AnnouncementUpdatedByNavigation = new HashSet<Announcement>();
            AppraisalAddedByNavigation = new HashSet<Appraisal>();
            AppraisalAnswerAddedByNavigation = new HashSet<AppraisalAnswer>();
            AppraisalAnswerUpdatedByNavigation = new HashSet<AppraisalAnswer>();
            AppraisalBusinessNeedAddedByNavigation = new HashSet<AppraisalBusinessNeed>();
            AppraisalBusinessNeedUpdatedByNavigation = new HashSet<AppraisalBusinessNeed>();
            AppraisalEmployeeAddedByNavigation = new HashSet<AppraisalEmployee>();
            AppraisalEmployeeEmployee = new HashSet<AppraisalEmployee>();
            AppraisalEmployeeUpdatedByNavigation = new HashSet<AppraisalEmployee>();
            AppraisalFeedbackAddedByNavigation = new HashSet<AppraisalFeedback>();
            AppraisalFeedbackGivenByNavigation = new HashSet<AppraisalFeedback>();
            AppraisalFeedbackUpdatedByNavigation = new HashSet<AppraisalFeedback>();
            AppraisalQuestionAddedByNavigation = new HashSet<AppraisalQuestion>();
            AppraisalQuestionUpdatedByNavigation = new HashSet<AppraisalQuestion>();
            AppraisalSelfAnswerAddedByNavigation = new HashSet<AppraisalSelfAnswer>();
            AppraisalSelfAnswerUpdatedByNavigation = new HashSet<AppraisalSelfAnswer>();
            AppraisalTraining = new HashSet<AppraisalTraining>();
            AppraisalUpdatedByNavigation = new HashSet<Appraisal>();
            AttachmentAddedByNavigation = new HashSet<Attachment>();
            AttachmentUpdatedByNavigation = new HashSet<Attachment>();
            AuditLog = new HashSet<AuditLog>();
            CommentAddedByNavigation = new HashSet<Comment>();
            CommentUpdatedByNavigation = new HashSet<Comment>();
            EmployeeAssetAddedByNavigation = new HashSet<EmployeeAsset>();
            EmployeeAssetEmployee = new HashSet<EmployeeAsset>();
            EmployeeAssetUpdatedByNavigation = new HashSet<EmployeeAsset>();
            EmployeeAuditEmp = new HashSet<EmployeeAudit>();
            EmployeeAuditUpdatedByNavigation = new HashSet<EmployeeAudit>();
            EmployeeAuditVerifiedByNavigation = new HashSet<EmployeeAudit>();
            EmployeeBankAddedByNavigation = new HashSet<EmployeeBank>();
            EmployeeBankEmployee = new HashSet<EmployeeBank>();
            EmployeeBankUpdatedByNavigation = new HashSet<EmployeeBank>();
            EmployeeCareerAddedByNavigation = new HashSet<EmployeeCareer>();
            EmployeeCareerEmployee = new HashSet<EmployeeCareer>();
            EmployeeCareerUpdatedByNavigation = new HashSet<EmployeeCareer>();
            EmployeeCompanyAddedByNavigation = new HashSet<EmployeeCompany>();
            EmployeeCompanyEmployee = new HashSet<EmployeeCompany>();
            EmployeeCompanyReportingTo = new HashSet<EmployeeCompany>();
            EmployeeCompanyUpdatedByNavigation = new HashSet<EmployeeCompany>();
            EmployeeCompensationAddedByNavigation = new HashSet<EmployeeCompensation>();
            EmployeeCompensationEmployee = new HashSet<EmployeeCompensation>();
            EmployeeCompensationUpdatedByNavigation = new HashSet<EmployeeCompensation>();
            EmployeeContactAddedByNavigation = new HashSet<EmployeeContact>();
            EmployeeContactEmployee = new HashSet<EmployeeContact>();
            EmployeeContactUpdatedByNavigation = new HashSet<EmployeeContact>();
            EmployeeDataVerificationEmployee = new HashSet<EmployeeDataVerification>();
            EmployeeDataVerificationUpdatedByNavigation = new HashSet<EmployeeDataVerification>();
            EmployeeDataVerificationVerifiedByNavigation = new HashSet<EmployeeDataVerification>();
            EmployeeDocumentAddedByNavigation = new HashSet<EmployeeDocument>();
            EmployeeDocumentEmployee = new HashSet<EmployeeDocument>();
            EmployeeDocumentUpdatedByNavigation = new HashSet<EmployeeDocument>();
            EmployeeEducationAddedByNavigation = new HashSet<EmployeeEducation>();
            EmployeeEducationEmployee = new HashSet<EmployeeEducation>();
            EmployeeEducationUpdatedByNavigation = new HashSet<EmployeeEducation>();
            EmployeeEmergencyContactAddedByNavigation = new HashSet<EmployeeEmergencyContact>();
            EmployeeEmergencyContactEmployee = new HashSet<EmployeeEmergencyContact>();
            EmployeeEmergencyContactUpdatedByNavigation = new HashSet<EmployeeEmergencyContact>();
            EmployeeExitAddedByNavigation = new HashSet<EmployeeExit>();
            EmployeeExitAnswersEmployee = new HashSet<EmployeeExitAnswers>();
            EmployeeExitAnswersUpdatedByNavigation = new HashSet<EmployeeExitAnswers>();
            EmployeeExitAssetAddedByNavigation = new HashSet<EmployeeExitAsset>();
            EmployeeExitAssetAssetOwnerNavigation = new HashSet<EmployeeExitAsset>();
            EmployeeExitAssetManagerNavigation = new HashSet<EmployeeExitAsset>();
            EmployeeExitAssetSeniorManagerNavigation = new HashSet<EmployeeExitAsset>();
            EmployeeExitAssetUpdatedByNavigation = new HashSet<EmployeeExitAsset>();
            EmployeeExitConfirmedByNavigation = new HashSet<EmployeeExit>();
            EmployeeExitEmployee = new HashSet<EmployeeExit>();
            EmployeeExitFormAddedByNavigation = new HashSet<EmployeeExitForm>();
            EmployeeExitFormEmployee = new HashSet<EmployeeExitForm>();
            EmployeeExitFormUpdatedByNavigation = new HashSet<EmployeeExitForm>();
            EmployeeExitHodfeedBackFormAddedByNavigation = new HashSet<EmployeeExitHodfeedBackForm>();
            EmployeeExitHodfeedBackFormEmployee = new HashSet<EmployeeExitHodfeedBackForm>();
            EmployeeExitHodfeedBackFormUpdatedByNavigation = new HashSet<EmployeeExitHodfeedBackForm>();
            EmployeeExitHrEmployeeNavigation = new HashSet<EmployeeExit>();
            EmployeeExitHrfeedBackFormAddedByNavigation = new HashSet<EmployeeExitHrfeedBackForm>();
            EmployeeExitHrfeedBackFormEmployee = new HashSet<EmployeeExitHrfeedBackForm>();
            EmployeeExitHrfeedBackFormUpdatedByNavigation = new HashSet<EmployeeExitHrfeedBackForm>();
            EmployeeExitManagerEmployeeNavigation = new HashSet<EmployeeExit>();
            EmployeeExitSeniorManagerEmployeeNavigation = new HashSet<EmployeeExit>();
            EmployeeExitUpdatedByNavigation = new HashSet<EmployeeExit>();
            EmployeeFamilyAddedByNavigation = new HashSet<EmployeeFamily>();
            EmployeeFamilyEmployee = new HashSet<EmployeeFamily>();
            EmployeeFamilyUpdatedByNavigation = new HashSet<EmployeeFamily>();
            EmployeeLanguageAddedByNavigation = new HashSet<EmployeeLanguage>();
            EmployeeLanguageEmployee = new HashSet<EmployeeLanguage>();
            EmployeeLanguageUpdatedByNavigation = new HashSet<EmployeeLanguage>();
            EmployeePersonalAddedByNavigation = new HashSet<EmployeePersonal>();
            EmployeePersonalEmployee = new HashSet<EmployeePersonal>();
            EmployeePersonalUpdatedByNavigation = new HashSet<EmployeePersonal>();
            EmployeePreviousCompanyAddedByNavigation = new HashSet<EmployeePreviousCompany>();
            EmployeePreviousCompanyEmployee = new HashSet<EmployeePreviousCompany>();
            EmployeePreviousCompanyUpdatedByNavigation = new HashSet<EmployeePreviousCompany>();
            EmployeeReferenceAddedByNavigation = new HashSet<EmployeeReference>();
            EmployeeReferenceEmployee = new HashSet<EmployeeReference>();
            EmployeeReferenceUpdatedByNavigation = new HashSet<EmployeeReference>();
            EmployeeStatutoryAddedByNavigation = new HashSet<EmployeeStatutory>();
            EmployeeStatutoryEmployee = new HashSet<EmployeeStatutory>();
            EmployeeStatutoryUpdatedByNavigation = new HashSet<EmployeeStatutory>();
            InverseAddedByNavigation = new HashSet<Employee>();
            InverseUpdatedByNavigation = new HashSet<Employee>();
            Notification = new HashSet<Notification>();
            SettingsAnnouncementTypeAddedByNavigation = new HashSet<SettingsAnnouncementType>();
            SettingsAnnouncementTypeUpdatedByNavigation = new HashSet<SettingsAnnouncementType>();
            SettingsAppraisalModeAddedByNavigation = new HashSet<SettingsAppraisalMode>();
            SettingsAppraisalModeUpdatedByNavigation = new HashSet<SettingsAppraisalMode>();
            SettingsAppraisalQuestionAddedByNavigation = new HashSet<SettingsAppraisalQuestion>();
            SettingsAppraisalQuestionUpdatedByNavigation = new HashSet<SettingsAppraisalQuestion>();
            SettingsAppraisalRatingsAddedByNavigation = new HashSet<SettingsAppraisalRatings>();
            SettingsAppraisalRatingsUpdatedByNavigation = new HashSet<SettingsAppraisalRatings>();
            SettingsAppraiseeTypeAddedByNavigation = new HashSet<SettingsAppraiseeType>();
            SettingsAppraiseeTypeUpdatedByNavigation = new HashSet<SettingsAppraiseeType>();
            SettingsAssetTypeOwner = new HashSet<SettingsAssetTypeOwner>();
            SettingsAssetTypesAddedByNavigation = new HashSet<SettingsAssetTypes>();
            SettingsAssetTypesUpdatedByNavigation = new HashSet<SettingsAssetTypes>();
            SettingsAttachmentTypeAddedByNavigation = new HashSet<SettingsAttachmentType>();
            SettingsAttachmentTypeUpdatedByNavigation = new HashSet<SettingsAttachmentType>();
            SettingsCategoryAddedByNavigation = new HashSet<SettingsCategory>();
            SettingsCategoryUpdatedByNavigation = new HashSet<SettingsCategory>();
            SettingsDepartmentAddedByNavigation = new HashSet<SettingsDepartment>();
            SettingsDepartmentDesignationAddedByNavigation = new HashSet<SettingsDepartmentDesignation>();
            SettingsDepartmentDesignationUpdatedByNavigation = new HashSet<SettingsDepartmentDesignation>();
            SettingsDepartmentUpdatedByNavigation = new HashSet<SettingsDepartment>();
            SettingsDocumentTypeAddedByNavigation = new HashSet<SettingsDocumentType>();
            SettingsDocumentTypeUpdatedByNavigation = new HashSet<SettingsDocumentType>();
            SettingsExitQuestionAddedByNavigation = new HashSet<SettingsExitQuestion>();
            SettingsExitQuestionUpdatedByNavigation = new HashSet<SettingsExitQuestion>();
            SettingsGradeAddedByNavigation = new HashSet<SettingsGrade>();
            SettingsGradeUpdatedByNavigation = new HashSet<SettingsGrade>();
            SettingsHolidayAddedByNavigation = new HashSet<SettingsHoliday>();
            SettingsHolidayUpdatedByNavigation = new HashSet<SettingsHoliday>();
            SettingsLocationAddedByNavigation = new HashSet<SettingsLocation>();
            SettingsLocationUpdatedByNavigation = new HashSet<SettingsLocation>();
            SettingsProductLineAddedByNavigation = new HashSet<SettingsProductLine>();
            SettingsProductLineUpdatedByNavigation = new HashSet<SettingsProductLine>();
            SettingsRegionAddedByNavigation = new HashSet<SettingsRegion>();
            SettingsRegionUpdatedByNavigation = new HashSet<SettingsRegion>();
            SettingsReportAddedByNavigation = new HashSet<SettingsReport>();
            SettingsReportInputsAddedByNavigation = new HashSet<SettingsReportInputs>();
            SettingsReportInputsUpdatedByNavigation = new HashSet<SettingsReportInputs>();
            SettingsReportUpdatedByNavigation = new HashSet<SettingsReport>();
            SettingsRoleAddedByNavigation = new HashSet<SettingsRole>();
            SettingsRoleUpdatedByNavigation = new HashSet<SettingsRole>();
            SettingsModuleAccessAddedByNavigation = new HashSet<SettingsModuleAccess>();
            SettingsModuleAccessUpdatedByNavigation = new HashSet<SettingsModuleAccess>();
            SettingsTeamAddedByNavigation = new HashSet<SettingsTeam>();
            SettingsTeamUpdatedByNavigation = new HashSet<SettingsTeam>();
            SettingsTicketCategoryAddedByNavigation = new HashSet<SettingsTicketCategory>();
            SettingsTicketCategoryOwner = new HashSet<SettingsTicketCategoryOwner>();
            SettingsTicketCategoryUpdatedByNavigation = new HashSet<SettingsTicketCategory>();
            SettingsTicketFaqAddedByNavigation = new HashSet<SettingsTicketFaq>();
            SettingsTicketFaqUpdatedByNavigation = new HashSet<SettingsTicketFaq>();
            SettingsTicketStatusAddedByNavigation = new HashSet<SettingsTicketStatus>();
            SettingsTicketStatusUpdatedByNavigation = new HashSet<SettingsTicketStatus>();
            SettingsTicketSubCategoryAddedByNavigation = new HashSet<SettingsTicketSubCategory>();
            SettingsTicketSubCategoryUpdatedByNavigation = new HashSet<SettingsTicketSubCategory>();
            SettingsTrainingAddedByNavigation = new HashSet<SettingsTraining>();
            SettingsTrainingUpdatedByNavigation = new HashSet<SettingsTraining>();
            
            TaskAddedByNavigation = new HashSet<Task>();
            TaskAssignedToNavigation = new HashSet<Task>();
            TaskUpdatedByNavigation = new HashSet<Task>();
            TicketAddedByNavigation = new HashSet<Ticket>();
            TicketAttachmentAddedByNavigation = new HashSet<TicketAttachment>();
            TicketAttachmentUpdatedByNavigation = new HashSet<TicketAttachment>();
            TicketStatus = new HashSet<TicketStatus>();
            TicketUpdatedByNavigation = new HashSet<Ticket>();
            TrainingAddedByNavigation = new HashSet<Training>();
            TrainingNomineesEmployee = new HashSet<TrainingNominees>();
            TrainingNomineesHr = new HashSet<TrainingNominees>();
            TrainingNomineesManager = new HashSet<TrainingNominees>();
            TrainingOrganizer = new HashSet<TrainingOrganizer>();
            TrainingUpdatedByNavigation = new HashSet<Training>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("emailId")]
        [StringLength(200)]
        public string EmailId { get; set; }
        [Required]
        [Column("password")]
        [StringLength(200)]
        public string Password { get; set; }
        [Required]
        [Column("passwordSalt")]
        [StringLength(100)]
        public string PasswordSalt { get; set; }
        [Column("isTemporaryPasswordSet")]
        public bool? IsTemporaryPasswordSet { get; set; }
        [Column("roleId")]
        public long? RoleId { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("temporaryPassword")]
        [StringLength(100)]
        public string TemporaryPassword { get; set; }
        [Column("OTPPassword")]
        [StringLength(100)]
        public string OTPPassword { get; set; }

        [Column("temporaryPasswordExpiry", TypeName = "datetime")]
        public DateTime? TemporaryPasswordExpiry { get; set; }
        [Column("canLogin")]
        public bool? CanLogin { get; set; }
        [Column("companyId")]
        public long? CompanyId { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("InverseAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("RoleId")]
        [InverseProperty("Employee")]
        public virtual SettingsRole Role { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("InverseUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Announcement> AnnouncementAddedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AnnouncementAttachment> AnnouncementAttachmentAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AnnouncementAttachment> AnnouncementAttachmentUpdatedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Announcement> AnnouncementUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Appraisal> AppraisalAddedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalAnswer> AppraisalAnswerAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AppraisalAnswer> AppraisalAnswerUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalBusinessNeed> AppraisalBusinessNeedAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AppraisalBusinessNeed> AppraisalBusinessNeedUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployeeAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployeeEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployeeUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedbackAddedByNavigation { get; set; }
        [InverseProperty("GivenByNavigation")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedbackGivenByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedbackUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalQuestion> AppraisalQuestionAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AppraisalQuestion> AppraisalQuestionUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalSelfAnswer> AppraisalSelfAnswerAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<AppraisalSelfAnswer> AppraisalSelfAnswerUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<AppraisalTraining> AppraisalTraining { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Appraisal> AppraisalUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Attachment> AttachmentAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Attachment> AttachmentUpdatedByNavigation { get; set; }
        [InverseProperty("PerformedByNavigation")]
        public virtual ICollection<AuditLog> AuditLog { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Comment> CommentAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Comment> CommentUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeAsset> EmployeeAssetAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeAsset> EmployeeAssetEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeAsset> EmployeeAssetUpdatedByNavigation { get; set; }
        [InverseProperty("Emp")]
        public virtual ICollection<EmployeeAudit> EmployeeAuditEmp { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeAudit> EmployeeAuditUpdatedByNavigation { get; set; }
        [InverseProperty("VerifiedByNavigation")]
        public virtual ICollection<EmployeeAudit> EmployeeAuditVerifiedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeBank> EmployeeBankAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeBank> EmployeeBankEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeBank> EmployeeBankUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeCareer> EmployeeCareerAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeCareer> EmployeeCareerEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeCareer> EmployeeCareerUpdatedByNavigation { get; set; }

        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeCompany> EmployeeCompanyAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeCompany> EmployeeCompanyEmployee { get; set; }
        [InverseProperty("ReportingTo")]
        public virtual ICollection<EmployeeCompany> EmployeeCompanyReportingTo { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeCompany> EmployeeCompanyUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensationAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensationEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeCompensation> EmployeeCompensationUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeContact> EmployeeContactAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeContact> EmployeeContactEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeContact> EmployeeContactUpdatedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeDataVerification> EmployeeDataVerificationEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeDataVerification> EmployeeDataVerificationUpdatedByNavigation { get; set; }
        [InverseProperty("VerifiedByNavigation")]
        public virtual ICollection<EmployeeDataVerification> EmployeeDataVerificationVerifiedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeDocument> EmployeeDocumentAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeDocument> EmployeeDocumentEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeDocument> EmployeeDocumentUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeEducation> EmployeeEducationAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeEducation> EmployeeEducationEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeEducation> EmployeeEducationUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeEmergencyContact> EmployeeEmergencyContactAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeEmergencyContact> EmployeeEmergencyContactEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeEmergencyContact> EmployeeEmergencyContactUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeExit> EmployeeExitAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeExitAnswers> EmployeeExitAnswersEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeExitAnswers> EmployeeExitAnswersUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAssetAddedByNavigation { get; set; }
        [InverseProperty("AssetOwnerNavigation")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAssetAssetOwnerNavigation { get; set; }
        [InverseProperty("ManagerNavigation")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAssetManagerNavigation { get; set; }
        [InverseProperty("SeniorManagerNavigation")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAssetSeniorManagerNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeExitAsset> EmployeeExitAssetUpdatedByNavigation { get; set; }
        [InverseProperty("ConfirmedByNavigation")]
        public virtual ICollection<EmployeeExit> EmployeeExitConfirmedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeExit> EmployeeExitEmployee { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeExitForm> EmployeeExitFormAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeExitForm> EmployeeExitFormEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeExitForm> EmployeeExitFormUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeExitHodfeedBackForm> EmployeeExitHodfeedBackFormAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeExitHodfeedBackForm> EmployeeExitHodfeedBackFormEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeExitHodfeedBackForm> EmployeeExitHodfeedBackFormUpdatedByNavigation { get; set; }
        [InverseProperty("HrEmployeeNavigation")]
        public virtual ICollection<EmployeeExit> EmployeeExitHrEmployeeNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeExitHrfeedBackForm> EmployeeExitHrfeedBackFormAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeExitHrfeedBackForm> EmployeeExitHrfeedBackFormEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeExitHrfeedBackForm> EmployeeExitHrfeedBackFormUpdatedByNavigation { get; set; }
        [InverseProperty("ManagerEmployeeNavigation")]
        public virtual ICollection<EmployeeExit> EmployeeExitManagerEmployeeNavigation { get; set; }
        [InverseProperty("SeniorManagerEmployeeNavigation")]
        public virtual ICollection<EmployeeExit> EmployeeExitSeniorManagerEmployeeNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeExit> EmployeeExitUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeFamily> EmployeeFamilyAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeFamily> EmployeeFamilyEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeFamily> EmployeeFamilyUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeLanguage> EmployeeLanguageAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeLanguage> EmployeeLanguageEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeLanguage> EmployeeLanguageUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeePersonal> EmployeePersonalAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeePersonal> EmployeePersonalEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeePersonal> EmployeePersonalUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeePreviousCompany> EmployeePreviousCompanyAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeePreviousCompany> EmployeePreviousCompanyEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeePreviousCompany> EmployeePreviousCompanyUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeReference> EmployeeReferenceAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeReference> EmployeeReferenceEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeReference> EmployeeReferenceUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<EmployeeStatutory> EmployeeStatutoryAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<EmployeeStatutory> EmployeeStatutoryEmployee { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<EmployeeStatutory> EmployeeStatutoryUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Employee> InverseAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Employee> InverseUpdatedByNavigation { get; set; }
        [InverseProperty("NotificationToNavigation")]
        public virtual ICollection<Notification> Notification { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAnnouncementType> SettingsAnnouncementTypeAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAnnouncementType> SettingsAnnouncementTypeUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAppraisalMode> SettingsAppraisalModeAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAppraisalMode> SettingsAppraisalModeUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAppraisalQuestion> SettingsAppraisalQuestionAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAppraisalQuestion> SettingsAppraisalQuestionUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAppraisalRatings> SettingsAppraisalRatingsAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAppraisalRatings> SettingsAppraisalRatingsUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAppraiseeType> SettingsAppraiseeTypeAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAppraiseeType> SettingsAppraiseeTypeUpdatedByNavigation { get; set; }
        [InverseProperty("Owner")]
        public virtual ICollection<SettingsAssetTypeOwner> SettingsAssetTypeOwner { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAssetTypes> SettingsAssetTypesAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAssetTypes> SettingsAssetTypesUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsAttachmentType> SettingsAttachmentTypeAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsAttachmentType> SettingsAttachmentTypeUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsCategory> SettingsCategoryAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsCategory> SettingsCategoryUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsDepartment> SettingsDepartmentAddedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsDepartmentDesignation> SettingsDepartmentDesignationAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsDepartmentDesignation> SettingsDepartmentDesignationUpdatedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsDepartment> SettingsDepartmentUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsDocumentType> SettingsDocumentTypeAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsDocumentType> SettingsDocumentTypeUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsExitQuestion> SettingsExitQuestionAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsExitQuestion> SettingsExitQuestionUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsGrade> SettingsGradeAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsGrade> SettingsGradeUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsHoliday> SettingsHolidayAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsHoliday> SettingsHolidayUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsLocation> SettingsLocationAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsLocation> SettingsLocationUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsProductLine> SettingsProductLineAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsProductLine> SettingsProductLineUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsRegion> SettingsRegionAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsRegion> SettingsRegionUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsReport> SettingsReportAddedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsReportInputs> SettingsReportInputsAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsReportInputs> SettingsReportInputsUpdatedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsReport> SettingsReportUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsRole> SettingsRoleAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsRole> SettingsRoleUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsModuleAccess> SettingsModuleAccessAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsModuleAccess> SettingsModuleAccessUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsTeam> SettingsTeamAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsTeam> SettingsTeamUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsTicketCategory> SettingsTicketCategoryAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<SettingsTicketCategoryOwner> SettingsTicketCategoryOwner { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsTicketCategory> SettingsTicketCategoryUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsTicketFaq> SettingsTicketFaqAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsTicketFaq> SettingsTicketFaqUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsTicketStatus> SettingsTicketStatusAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsTicketStatus> SettingsTicketStatusUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsTicketSubCategory> SettingsTicketSubCategoryAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsTicketSubCategory> SettingsTicketSubCategoryUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<SettingsTraining> SettingsTrainingAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<SettingsTraining> SettingsTrainingUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Task> TaskAddedByNavigation { get; set; }
        [InverseProperty("AssignedToNavigation")]
        public virtual ICollection<Task> TaskAssignedToNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Task> TaskUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Ticket> TicketAddedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<TicketAttachment> TicketAttachmentAddedByNavigation { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<TicketAttachment> TicketAttachmentUpdatedByNavigation { get; set; }
        [InverseProperty("ChangedByNavigation")]
        public virtual ICollection<TicketStatus> TicketStatus { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Ticket> TicketUpdatedByNavigation { get; set; }
        [InverseProperty("AddedByNavigation")]
        public virtual ICollection<Training> TrainingAddedByNavigation { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<TrainingNominees> TrainingNomineesEmployee { get; set; }
        [InverseProperty("Hr")]
        public virtual ICollection<TrainingNominees> TrainingNomineesHr { get; set; }
        [InverseProperty("Manager")]
        public virtual ICollection<TrainingNominees> TrainingNomineesManager { get; set; }
        [InverseProperty("Employee")]
        public virtual ICollection<TrainingOrganizer> TrainingOrganizer { get; set; }
        [InverseProperty("UpdatedByNavigation")]
        public virtual ICollection<Training> TrainingUpdatedByNavigation { get; set; }

    }
}
