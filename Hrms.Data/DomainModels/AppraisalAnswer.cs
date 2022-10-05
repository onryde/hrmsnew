using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppraisalAnswer
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("addedBy")]
        public long? AddedBy { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime? AddedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("companyId")]
        public long? CompanyId { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("appraisalQuestionId")]
        public long AppraisalQuestionId { get; set; }
        [Column("appraisalEmployeeId")]
        public long AppraisalEmployeeId { get; set; }
        [Column("selfWeightage")]
        public double SelfWeightage { get; set; }
        [Column("managementWeightage")]
        public double ManagementWeightage { get; set; }
        [Required]
        [Column("answer")]
        [StringLength(1000)]
        public string Answer { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("l2Weightage")]
        public double L2Weightage { get; set; }
        [Column("selfVariableWeightage")]
        public double? SelfVariableWeightage { get; set; }
        [Column("managementVariableWeightage")]
        public double? ManagementVariableWeightage { get; set; }
        [Column("l2VariableWeightage")]
        public double? L2VariableWeightage { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("AppraisalAnswerAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("AppraisalEmployeeId")]
        [InverseProperty("AppraisalAnswer")]
        public virtual AppraisalEmployee AppraisalEmployee { get; set; }
        [ForeignKey("AppraisalQuestionId")]
        [InverseProperty("AppraisalAnswer")]
        public virtual AppraisalQuestion AppraisalQuestion { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("AppraisalAnswer")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("AppraisalAnswerUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
