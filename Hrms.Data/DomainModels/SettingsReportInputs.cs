using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsReportInputs
    {
        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Required]
        [Column("fieldName")]
        [StringLength(100)]
        public string FieldName { get; set; }
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
        [Column("reportId")]
        public long ReportId { get; set; }
        [Column("description")]
        [StringLength(500)]
        public string Description { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("SettingsReportInputsAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsReportInputs")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("ReportId")]
        [InverseProperty("SettingsReportInputs")]
        public virtual SettingsReport Report { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("SettingsReportInputsUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
