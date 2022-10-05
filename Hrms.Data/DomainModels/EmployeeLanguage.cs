using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeLanguage
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
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("languageName")]
        [StringLength(100)]
        public string LanguageName { get; set; }
        [Column("canRead")]
        public bool? CanRead { get; set; }
        [Column("canSpeak")]
        public bool? CanSpeak { get; set; }
        [Column("canWrite")]
        public bool? CanWrite { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("level")]
        [StringLength(100)]
        public string Level { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeeLanguageAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeeLanguage")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeeLanguageEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeeLanguageUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
