using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsAppraisalRatings
    {
        public SettingsAppraisalRatings()
        {
            AppraisalEmployeeRatingNavigation = new HashSet<AppraisalEmployee>();
            AppraisalEmployeeVariableBonusRatingNavigation = new HashSet<AppraisalEmployee>();
        }

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
        [Required]
        [Column("ratingTitle")]
        [StringLength(100)]
        public string RatingTitle { get; set; }
        [Required]
        [Column("description")]
        [StringLength(1000)]
        public string Description { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(100)]
        public string Guid { get; set; }
        [Column("ratingTag")]
        [StringLength(10)]
        public string RatingTag { get; set; }
        [Column("ratingTotal")]
        public double? RatingTotal { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsAppraisalRatingsAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsAppraisalRatings")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsAppraisalRatingsUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("RatingNavigation")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployeeRatingNavigation { get; set; }
        [InverseProperty("VariableBonusRatingNavigation")]
        public virtual ICollection<AppraisalEmployee> AppraisalEmployeeVariableBonusRatingNavigation { get; set; }
    }
}
