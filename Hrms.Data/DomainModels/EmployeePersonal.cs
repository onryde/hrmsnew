using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeePersonal
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
        [Column("nationality")]
        [StringLength(100)]
        public string Nationality { get; set; }
        [Column("dobRecord", TypeName = "date")]
        public DateTime? DobRecord { get; set; }
        [Column("dobActual", TypeName = "date")]
        public DateTime? DobActual { get; set; }
        [Column("age")]
        public int? Age { get; set; }
        [Column("gender")]
        [StringLength(100)]
        public string Gender { get; set; }
        [Column("bloodGroup")]
        [StringLength(100)]
        public string BloodGroup { get; set; }
        [Column("maritalStatus")]
        [StringLength(100)]
        public string MaritalStatus { get; set; }
        [Column("marriageDate", TypeName = "date")]
        public DateTime? MarriageDate { get; set; }
        [Column("sports")]
        [StringLength(1000)]
        public string Sports { get; set; }
        [Column("specializedTraining")]
        [StringLength(1000)]
        public string SpecializedTraining { get; set; }
        [Column("height")]
        public double? Height { get; set; }
        [Column("weight")]
        public double? Weight { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("photoUrl")]
        [StringLength(1000)]
        public string PhotoUrl { get; set; }
        [Column("photoLinkUrl")]
        [StringLength(1000)]
        public string PhotoLinkUrl { get; set; }
        [Column("hideBirthday")]
        public bool? HideBirthday { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("EmployeePersonalAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("EmployeePersonal")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("EmployeePersonalEmployee")]
        public virtual Employee Employee { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("EmployeePersonalUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
    }
}
