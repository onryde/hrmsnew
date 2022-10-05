using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsReport
    {
        public SettingsReport()
        {
            SettingsReportInputs = new HashSet<SettingsReportInputs>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("reportName")]
        [StringLength(100)]
        public string ReportName { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsReportAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsReport")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsReportUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Report")]
        public virtual ICollection<SettingsReportInputs> SettingsReportInputs { get; set; }
    }
}
