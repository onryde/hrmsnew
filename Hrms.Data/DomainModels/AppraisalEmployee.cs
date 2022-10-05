using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppraisalEmployee
    {
        public AppraisalEmployee()
        {
            AppraisalAnswer = new HashSet<AppraisalAnswer>();
            AppraisalBusinessNeed = new HashSet<AppraisalBusinessNeed>();
            AppraisalFeedback = new HashSet<AppraisalFeedback>();
            AppraisalSelfAnswer = new HashSet<AppraisalSelfAnswer>();
            AppraisalTraining = new HashSet<AppraisalTraining>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("appraisalId")]
        public long AppraisalId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Required]
        [Column("status")]
        [StringLength(100)]
        public string Status { get; set; }
        [Column("rating")]
        public long? Rating { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("selfSubmittedOn", TypeName = "datetime")]
        public DateTime? SelfSubmittedOn { get; set; }
        [Column("rmSubmittedOn", TypeName = "datetime")]
        public DateTime? RmSubmittedOn { get; set; }
        [Column("closedOn", TypeName = "datetime")]
        public DateTime? ClosedOn { get; set; }
        [Column("hrSubmittedOn", TypeName = "datetime")]
        public DateTime? HrSubmittedOn { get; set; }
        [Column("internalSelf")]
        public double? InternalSelf { get; set; }
        [Column("internalMgmt")]
        public double? InternalMgmt { get; set; }
        [Column("internalL2")]
        public double? InternalL2 { get; set; }
        [Column("l2SubmittedOn", TypeName = "datetime")]
        public DateTime? L2SubmittedOn { get; set; }
        [Column("selfObjectiveSubmittedOn", TypeName = "datetime")]
        public DateTime? SelfObjectiveSubmittedOn { get; set; }
        [Column("rmObjectiveSubmittedOn", TypeName = "datetime")]
        public DateTime? RmObjectiveSubmittedOn { get; set; }
        [Column("l2ObjectiveSubmittedOn", TypeName = "datetime")]
        public DateTime? L2ObjectiveSubmittedOn { get; set; }
        [Column("hrObjectiveSubmittedOn", TypeName = "datetime")]
        public DateTime? HrObjectiveSubmittedOn { get; set; }
        [Column("internalVariableSelf")]
        public double? InternalVariableSelf { get; set; }
        [Column("internalVariableMgmt")]
        public double? InternalVariableMgmt { get; set; }
        [Column("internalVariableL2")]
        public double? InternalVariableL2 { get; set; }
        [Column("variableBonusRating")]
        public long? VariableBonusRating { get; set; }
        [Column("selfVariableSubmittedOn", TypeName = "datetime")]
        public DateTime? SelfVariableSubmittedOn { get; set; }
        [Column("rmVariableSubmittedOn", TypeName = "datetime")]
        public DateTime? RmVariableSubmittedOn { get; set; }
        [Column("l2VariableSubmittedOn", TypeName = "datetime")]
        public DateTime? L2VariableSubmittedOn { get; set; }
        [Column("hrVariableSubmittedOn", TypeName = "datetime")]
        public DateTime? HrVariableSubmittedOn { get; set; }
        public bool? IsRecommendedForFigment { get; set; }
        public bool? IsRecommendedForPromotion { get; set; }
        [StringLength(250)]
        public string TrainingComments { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AppraisalEmployeeAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AppraisalId")]
        [InverseProperty("AppraisalEmployee")]
        public virtual Appraisal Appraisal { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("AppraisalEmployee")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("AppraisalEmployeeEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("Rating")]
        [InverseProperty("AppraisalEmployeeRatingNavigation")]
        public virtual SettingsAppraisalRatings RatingNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AppraisalEmployeeUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [ForeignKey("VariableBonusRating")]
        [InverseProperty("AppraisalEmployeeVariableBonusRatingNavigation")]
        public virtual SettingsAppraisalRatings VariableBonusRatingNavigation { get; set; }
        [InverseProperty("AppraisalEmployee")]
        public virtual ICollection<AppraisalAnswer> AppraisalAnswer { get; set; }
        [InverseProperty("AppraisalEmployee")]
        public virtual ICollection<AppraisalBusinessNeed> AppraisalBusinessNeed { get; set; }
        [InverseProperty("AppraisalEmployee")]
        public virtual ICollection<AppraisalFeedback> AppraisalFeedback { get; set; }
        [InverseProperty("AppraisalEmployee")]
        public virtual ICollection<AppraisalSelfAnswer> AppraisalSelfAnswer { get; set; }
        [InverseProperty("AppraisalEmployee")]
        public virtual ICollection<AppraisalTraining> AppraisalTraining { get; set; }
    }
}
