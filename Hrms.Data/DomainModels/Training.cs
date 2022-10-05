using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class Training
    {
        public Training()
        {
            TrainingAttendance = new HashSet<TrainingAttendance>();
            TrainingDate = new HashSet<TrainingDate>();
            TrainingDepartment = new HashSet<TrainingDepartment>();
            TrainingDesignation = new HashSet<TrainingDesignation>();
            TrainingFeedback = new HashSet<TrainingFeedback>();
            TrainingGrade = new HashSet<TrainingGrade>();
            TrainingLocation1 = new HashSet<TrainingLocation>();
            TrainingNominees = new HashSet<TrainingNominees>();
            TrainingOrganizer = new HashSet<TrainingOrganizer>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Required]
        [Column("guid")]
        [StringLength(64)]
        public string Guid { get; set; }
        [Column("addedOn", TypeName = "datetime")]
        public DateTime AddedOn { get; set; }
        [Column("addedBy")]
        public long AddedBy { get; set; }
        [Column("updatedOn", TypeName = "datetime")]
        public DateTime? UpdatedOn { get; set; }
        [Column("updatedBy")]
        public long? UpdatedBy { get; set; }
        [Column("isActive")]
        public bool IsActive { get; set; }
        [Column("otherLocation")]
        [StringLength(100)]
        public string OtherLocation { get; set; }
        [Column("isOfficeLocation")]
        public bool IsOfficeLocation { get; set; }
        [Column("trainingLocation")]
        public long? TrainingLocation { get; set; }
        [Column("totalDays")]
        public int TotalDays { get; set; }
        [Column("maxNominees")]
        public int MaxNominees { get; set; }
        [Column("description")]
        [StringLength(200)]
        public string Description { get; set; }
        [Column("cancellationReason")]
        [StringLength(200)]
        public string CancellationReason { get; set; }
        [Column("trainingId")]
        public long TrainingId { get; set; }
        [Column("trainingNumber")]
        public int TrainingNumber { get; set; }
        [Column("trainerName")]
        [StringLength(1000)]
        public string TrainerName { get; set; }
        [Column("isConfirmed")]
        public bool? IsConfirmed { get; set; }
        [Column("isCompleted")]
        public bool? IsCompleted { get; set; }
        [Column("timeOfDay")]
        [StringLength(100)]
        public string TimeOfDay { get; set; }
        [Column("isStarted")]
        public bool? IsStarted { get; set; }
        [Column("title")]
        [StringLength(100)]
        public string Title { get; set; }
        [Column("code")]
        [StringLength(100)]
        public string Code { get; set; }
        [Column("category")]
        [StringLength(100)]
        public string Category { get; set; }
        [Column("isFeedbackClosed")]
        public bool? IsFeedbackClosed { get; set; }

        [ForeignKey("AddedBy")]
        [InverseProperty("TrainingAddedByNavigation")]
        public virtual Employee AddedByNavigation { get; set; }
        [ForeignKey("TrainingLocation")]
        [InverseProperty("Training")]
        public virtual SettingsLocation TrainingLocationNavigation { get; set; }
        [ForeignKey("TrainingId")]
        [InverseProperty("Training")]
        public virtual SettingsTraining TrainingNavigation { get; set; }
        [ForeignKey("UpdatedBy")]
        [InverseProperty("TrainingUpdatedByNavigation")]
        public virtual Employee UpdatedByNavigation { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingAttendance> TrainingAttendance { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingDate> TrainingDate { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingDepartment> TrainingDepartment { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingDesignation> TrainingDesignation { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingFeedback> TrainingFeedback { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingGrade> TrainingGrade { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingLocation> TrainingLocation1 { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingNominees> TrainingNominees { get; set; }
        [InverseProperty("Training")]
        public virtual ICollection<TrainingOrganizer> TrainingOrganizer { get; set; }
    }
}
