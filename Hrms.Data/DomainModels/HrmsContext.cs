using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Hrms.Data.DomainModels
{
    public partial class HrmsContext : DbContext
    {
        public HrmsContext()
        {
        }

        public HrmsContext(DbContextOptions<HrmsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Announcement> Announcement { get; set; }
        public virtual DbSet<AnnouncementAttachment> AnnouncementAttachment { get; set; }
        public virtual DbSet<AnnouncementDepartment> AnnouncementDepartment { get; set; }
        public virtual DbSet<AnnouncementLocation> AnnouncementLocation { get; set; }
        public virtual DbSet<AppAction> AppAction { get; set; }
        public virtual DbSet<AppCompany> AppCompany { get; set; }
        public virtual DbSet<AppModule> AppModule { get; set; }
        public virtual DbSet<Appraisal> Appraisal { get; set; }
        public virtual DbSet<AppraisalAnswer> AppraisalAnswer { get; set; }
        public virtual DbSet<AppraisalBusinessNeed> AppraisalBusinessNeed { get; set; }
        public virtual DbSet<AppraisalEmployee> AppraisalEmployee { get; set; }
        public virtual DbSet<AppraisalFeedback> AppraisalFeedback { get; set; }
        public virtual DbSet<AppraisalGrade> AppraisalGrade { get; set; }
        public virtual DbSet<AppraisalQuestion> AppraisalQuestion { get; set; }
        public virtual DbSet<AppraisalSelfAnswer> AppraisalSelfAnswer { get; set; }
        public virtual DbSet<AppraisalTraining> AppraisalTraining { get; set; }
        public virtual DbSet<Attachment> Attachment { get; set; }
        public virtual DbSet<AuditLog> AuditLog { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<Employee> Employee { get; set; }
        public virtual DbSet<EmployeeAddress> EmployeeAddress { get; set; }
        public virtual DbSet<EmployeeAsset> EmployeeAsset { get; set; }
        public virtual DbSet<EmployeeAudit> EmployeeAudit { get; set; }
        public virtual DbSet<EmployeeBank> EmployeeBank { get; set; }
        public virtual DbSet<EmployeeCareer> EmployeeCareer { get; set; }
        public virtual DbSet<EmployeeCompany> EmployeeCompany { get; set; }
        public virtual DbSet<EmployeeCompensation> EmployeeCompensation { get; set; }
        public virtual DbSet<EmployeeContact> EmployeeContact { get; set; }
        public virtual DbSet<EmployeeDataVerification> EmployeeDataVerification { get; set; }
        public virtual DbSet<EmployeeDocument> EmployeeDocument { get; set; }
        public virtual DbSet<EmployeeEducation> EmployeeEducation { get; set; }
        public virtual DbSet<EmployeeEmergencyContact> EmployeeEmergencyContact { get; set; }
        public virtual DbSet<EmployeeExit> EmployeeExit { get; set; }
        public virtual DbSet<EmployeeExitAnswers> EmployeeExitAnswers { get; set; }
        public virtual DbSet<EmployeeExitAsset> EmployeeExitAsset { get; set; }
        public virtual DbSet<EmployeeExitForm> EmployeeExitForm { get; set; }
        public virtual DbSet<EmployeeExitHodfeedBackForm> EmployeeExitHodfeedBackForm { get; set; }
        public virtual DbSet<EmployeeExitHrfeedBackForm> EmployeeExitHrfeedBackForm { get; set; }
        public virtual DbSet<EmployeeFamily> EmployeeFamily { get; set; }
        public virtual DbSet<EmployeeLanguage> EmployeeLanguage { get; set; }
        public virtual DbSet<EmployeePersonal> EmployeePersonal { get; set; }
        public virtual DbSet<EmployeePreviousCompany> EmployeePreviousCompany { get; set; }
        public virtual DbSet<EmployeeReference> EmployeeReference { get; set; }
        public virtual DbSet<EmployeeStatutory> EmployeeStatutory { get; set; }
        public virtual DbSet<Notification> Notification { get; set; }
        public virtual DbSet<SettingsAnnouncementType> SettingsAnnouncementType { get; set; }
        public virtual DbSet<SettingsAppraisalMode> SettingsAppraisalMode { get; set; }
        public virtual DbSet<SettingsAppraisalQuestion> SettingsAppraisalQuestion { get; set; }
        public virtual DbSet<SettingsAppraisalRatings> SettingsAppraisalRatings { get; set; }
        public virtual DbSet<SettingsAppraiseeType> SettingsAppraiseeType { get; set; }
        public virtual DbSet<SettingsAssetTypeOwner> SettingsAssetTypeOwner { get; set; }
        public virtual DbSet<SettingsAssetTypes> SettingsAssetTypes { get; set; }
        public virtual DbSet<SettingsAttachmentType> SettingsAttachmentType { get; set; }
        public virtual DbSet<SettingsCategory> SettingsCategory { get; set; }
        public virtual DbSet<SettingsDepartment> SettingsDepartment { get; set; }
        public virtual DbSet<SettingsDepartmentDesignation> SettingsDepartmentDesignation { get; set; }
        public virtual DbSet<SettingsDocumentType> SettingsDocumentType { get; set; }
        public virtual DbSet<SettingsExitGrade> SettingsExitGrade { get; set; }
        public virtual DbSet<SettingsExitQuestion> SettingsExitQuestion { get; set; }
        public virtual DbSet<SettingsGrade> SettingsGrade { get; set; }
        public virtual DbSet<SettingsHoliday> SettingsHoliday { get; set; }
        public virtual DbSet<SettingsHolidayLocation> SettingsHolidayLocation { get; set; }
        public virtual DbSet<SettingsLocation> SettingsLocation { get; set; }
        public virtual DbSet<SettingsProductLine> SettingsProductLine { get; set; }
        public virtual DbSet<SettingsRegion> SettingsRegion { get; set; }
        public virtual DbSet<SettingsReport> SettingsReport { get; set; }
        public virtual DbSet<SettingsReportInputs> SettingsReportInputs { get; set; }
        public virtual DbSet<SettingsRole> SettingsRole { get; set; }
        public virtual DbSet<SettingsModuleAccess> SettingsModuleAccess { get; set; }
        public virtual DbSet<SettingsRoleActionAccess> SettingsRoleActionAccess { get; set; }
        public virtual DbSet<SettingsTeam> SettingsTeam { get; set; }
        public virtual DbSet<SettingsTicketCategory> SettingsTicketCategory { get; set; }
        public virtual DbSet<SettingsTicketCategoryOwner> SettingsTicketCategoryOwner { get; set; }
        public virtual DbSet<SettingsTicketFaq> SettingsTicketFaq { get; set; }
        public virtual DbSet<SettingsTicketStatus> SettingsTicketStatus { get; set; }
        public virtual DbSet<SettingsTicketSubCategory> SettingsTicketSubCategory { get; set; }
        public virtual DbSet<SettingsTraining> SettingsTraining { get; set; }
        public virtual DbSet<SettingsTrainingFeedbackQuestion> SettingsTrainingFeedbackQuestion { get; set; }
        public virtual DbSet<SettingsTrainingGrade> SettingsTrainingGrade { get; set; }
        public virtual DbSet<Task> Task { get; set; }
        public virtual DbSet<TaskComment> TaskComment { get; set; }
        public virtual DbSet<Ticket> Ticket { get; set; }
        public virtual DbSet<TicketAttachment> TicketAttachment { get; set; }
        public virtual DbSet<TicketComment> TicketComment { get; set; }
        public virtual DbSet<TicketStatus> TicketStatus { get; set; }
        public virtual DbSet<Training> Training { get; set; }
        public virtual DbSet<TrainingAttendance> TrainingAttendance { get; set; }
        public virtual DbSet<TrainingDate> TrainingDate { get; set; }
        public virtual DbSet<TrainingDepartment> TrainingDepartment { get; set; }
        public virtual DbSet<TrainingDesignation> TrainingDesignation { get; set; }
        public virtual DbSet<TrainingFeedback> TrainingFeedback { get; set; }
        public virtual DbSet<TrainingGrade> TrainingGrade { get; set; }
        public virtual DbSet<TrainingLocation> TrainingLocation { get; set; }
        public virtual DbSet<TrainingNominees> TrainingNominees { get; set; }
        public virtual DbSet<TrainingOrganizer> TrainingOrganizer { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=tcp:kubotahrms.database.windows.net,1433;Initial Catalog=hrms-dev;Persist Security Info=False;User ID=hrms_user;Password=kubota1234$;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Announcement>(entity =>
            {
                entity.Property(e => e.SubTitle).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AnnouncementAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Announcem__added__02084FDA");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Announcement)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Announcement_FK");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Announcement)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Announcem__typeI__03F0984C");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AnnouncementUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Announcem__updat__02FC7413");
            });

            modelBuilder.Entity<AnnouncementAttachment>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AnnouncementAttachmentAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnouncementAttachment_1");

                entity.HasOne(d => d.Announcement)
                    .WithMany(p => p.AnnouncementAttachment)
                    .HasForeignKey(d => d.AnnouncementId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnouncementAttachment_4");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.AnnouncementAttachment)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AnnouncementAttachment_2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AnnouncementAttachmentUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_AnnouncementAttachment_3");
            });

            modelBuilder.Entity<AnnouncementDepartment>(entity =>
            {
                entity.HasOne(d => d.Announcement)
                    .WithMany(p => p.AnnouncementDepartment)
                    .HasForeignKey(d => d.AnnouncementId)
                    .HasConstraintName("AnnouncementDepartment_FK");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.AnnouncementDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("AnnouncementDepartment_FK_1");
            });

            modelBuilder.Entity<AnnouncementLocation>(entity =>
            {
                entity.HasOne(d => d.Announcement)
                    .WithMany(p => p.AnnouncementLocation)
                    .HasForeignKey(d => d.AnnouncementId)
                    .HasConstraintName("AnnouncementLocation_FK");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.AnnouncementLocation)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("AnnouncementLocation_FK_1");
            });

            modelBuilder.Entity<AppAction>(entity =>
            {
                entity.Property(e => e.ActionSlug).IsUnicode(false);
            });

            modelBuilder.Entity<AppCompany>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.AlternateLogo).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.FullLogo).IsUnicode(false);

                entity.Property(e => e.GstNumber).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.PanNumber).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.SmallLogo).IsUnicode(false);
            });

            modelBuilder.Entity<AppModule>(entity =>
            {
                entity.Property(e => e.ModuleSlug).IsUnicode(false);
            });

            modelBuilder.Entity<Appraisal>(entity =>
            {
                entity.Property(e => e.CalculationMethod).IsUnicode(false);

                entity.Property(e => e.Category).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Appraisal__added__06CD04F7");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Appraisal_FK");

                entity.HasOne(d => d.ModeNavigation)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.Mode)
                    .HasConstraintName("Appraisal_Mode_FK");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Appraisal__updat__07C12930");
            });

            modelBuilder.Entity<AppraisalAnswer>(entity =>
            {
                entity.Property(e => e.Answer).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalAnswerAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("AppraisalAnswer_FK");

                entity.HasOne(d => d.AppraisalEmployee)
                    .WithMany(p => p.AppraisalAnswer)
                    .HasForeignKey(d => d.AppraisalEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalAnswer_FK_5");

                entity.HasOne(d => d.AppraisalQuestion)
                    .WithMany(p => p.AppraisalAnswer)
                    .HasForeignKey(d => d.AppraisalQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalAnswer_FK_4");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalAnswer)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("AppraisalAnswer_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalAnswerUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("AppraisalAnswer_FK_1");
            });

            modelBuilder.Entity<AppraisalBusinessNeed>(entity =>
            {
                entity.Property(e => e.Answer).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalBusinessNeedAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("AppraisalBusinessNeed_FK");

                entity.HasOne(d => d.AppraisalEmployee)
                    .WithMany(p => p.AppraisalBusinessNeed)
                    .HasForeignKey(d => d.AppraisalEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalBusinessNeed_FK_5");

                entity.HasOne(d => d.AppraisalModeNavigation)
                    .WithMany(p => p.AppraisalBusinessNeed)
                    .HasForeignKey(d => d.AppraisalMode)
                    .HasConstraintName("AppraisalBusinessNeed_Mode_FK_3");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalBusinessNeed)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("AppraisalBusinessNeed_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalBusinessNeedUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("AppraisalBusinessNeed_FK_1");
            });

            modelBuilder.Entity<AppraisalEmployee>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.IsRecommendedForFigment).HasDefaultValueSql("((0))");

                entity.Property(e => e.IsRecommendedForPromotion).HasDefaultValueSql("((0))");

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.TrainingComments).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalEmployeeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalEmployee_FK");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.AppraisalEmployee)
                    .HasForeignKey(d => d.AppraisalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalEmployee_FK_4");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalEmployee)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalEmployee_FK_3");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.AppraisalEmployeeEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalEmployee_FK_5");

                entity.HasOne(d => d.RatingNavigation)
                    .WithMany(p => p.AppraisalEmployeeRatingNavigation)
                    .HasForeignKey(d => d.Rating)
                    .HasConstraintName("AppraisalEmployee_FK_6");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalEmployeeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("AppraisalEmployee_FK_1");

                entity.HasOne(d => d.VariableBonusRatingNavigation)
                    .WithMany(p => p.AppraisalEmployeeVariableBonusRatingNavigation)
                    .HasForeignKey(d => d.VariableBonusRating)
                    .HasConstraintName("AppraisalEmployee_VariableRating_FK");
            });

            modelBuilder.Entity<AppraisalFeedback>(entity =>
            {
                entity.Property(e => e.Feedback).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalFeedbackAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("AppraisalFeedback_FK");

                entity.HasOne(d => d.AppraisalEmployee)
                    .WithMany(p => p.AppraisalFeedback)
                    .HasForeignKey(d => d.AppraisalEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalFeedback_FK_4");

                entity.HasOne(d => d.AppraisalModeNavigation)
                    .WithMany(p => p.AppraisalFeedback)
                    .HasForeignKey(d => d.AppraisalMode)
                    .HasConstraintName("AppraisalFeedback_Mode_FK_3");

                entity.HasOne(d => d.AppraiseeTypeNavigation)
                    .WithMany(p => p.AppraisalFeedback)
                    .HasForeignKey(d => d.AppraiseeType)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalFeedback_FK_6");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalFeedback)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("AppraisalFeedback_FK_3");

                entity.HasOne(d => d.GivenByNavigation)
                    .WithMany(p => p.AppraisalFeedbackGivenByNavigation)
                    .HasForeignKey(d => d.GivenBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalFeedback_FK_2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalFeedbackUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("AppraisalFeedback_FK_1");
            });

            modelBuilder.Entity<AppraisalGrade>(entity =>
            {
                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.AppraisalGrade)
                    .HasForeignKey(d => d.AppraisalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AppraisalGrade1");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.AppraisalGrade)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AppraisalGrade2");
            });

            modelBuilder.Entity<AppraisalQuestion>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalQuestionAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("AppraisalQuestion_FK");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.AppraisalQuestion)
                    .HasForeignKey(d => d.AppraisalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalQuestion_FK_4");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalQuestion)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("AppraisalQuestion_FK_3");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.AppraisalQuestion)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalQuestion_FK_5");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalQuestionUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("AppraisalQuestion_FK_1");
            });

            modelBuilder.Entity<AppraisalSelfAnswer>(entity =>
            {
                entity.Property(e => e.Answer).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalSelfAnswerAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("AppraisalSelfAnswer_FK");

                entity.HasOne(d => d.AppraisalEmployee)
                    .WithMany(p => p.AppraisalSelfAnswer)
                    .HasForeignKey(d => d.AppraisalEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalSelfAnswer_FK_5");

                entity.HasOne(d => d.AppraisalModeNavigation)
                    .WithMany(p => p.AppraisalSelfAnswer)
                    .HasForeignKey(d => d.AppraisalMode)
                    .HasConstraintName("AppraisalSelfAnswer_Mode_FK_3");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalSelfAnswer)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("AppraisalSelfAnswer_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AppraisalSelfAnswerUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("AppraisalSelfAnswer_FK_1");
            });

            modelBuilder.Entity<AppraisalTraining>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AppraisalTraining)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalTraining_FK");

                entity.HasOne(d => d.AppraisalEmployee)
                    .WithMany(p => p.AppraisalTraining)
                    .HasForeignKey(d => d.AppraisalEmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalTraining_FK_5");

                entity.HasOne(d => d.Appraisal)
                    .WithMany(p => p.AppraisalTraining)
                    .HasForeignKey(d => d.AppraisalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalTraining_FK_4");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.AppraisalTraining)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalTraining_FK_3");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.AppraisalTraining)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AppraisalTraining_FK_2");
            });

            modelBuilder.Entity<Attachment>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.AttachmentAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_Attachment_1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.AttachmentUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("PK_Attachment_2");
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasOne(d => d.PerformedByNavigation)
                    .WithMany(p => p.AuditLog)
                    .HasForeignKey(d => d.PerformedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AuditLog_FK");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.CommentAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Comment__addedBy__17036CC0");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.CommentUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Comment__updated__17F790F9");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.TemporaryPassword).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.InverseAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("Employee_FK");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Employee)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("Employee_FK_2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.InverseUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("Employee_FK_1");
            });

            modelBuilder.Entity<EmployeeAddress>(entity =>
            {
                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.District).IsUnicode(false);

                entity.Property(e => e.DoorNo).IsUnicode(false);

                entity.Property(e => e.Landmark).IsUnicode(false);

                entity.Property(e => e.Pincode).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.Property(e => e.StreetName).IsUnicode(false);

                entity.Property(e => e.Village).IsUnicode(false);
            });

            modelBuilder.Entity<EmployeeAsset>(entity =>
            {
                entity.Property(e => e.AssetUniqueId).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.SignOffComment).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeAssetAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeAsset_FK_5");

                entity.HasOne(d => d.Asset)
                    .WithMany(p => p.EmployeeAsset)
                    .HasForeignKey(d => d.AssetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeAsset_FK_8");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeAsset)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeAsset_FK_4");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeAssetEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeAsset_FK_7");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeAssetUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeAsset_FK_6");
            });

            modelBuilder.Entity<EmployeeAudit>(entity =>
            {
                entity.HasKey(e => e.AuditId)
                    .HasName("PK__Employee__43D173997A071D45");

                entity.Property(e => e.EmployeeCode).IsUnicode(false);

                entity.Property(e => e.FieldName).IsUnicode(false);

                entity.Property(e => e.Module).IsUnicode(false);

                entity.HasOne(d => d.Emp)
                    .WithMany(p => p.EmployeeAuditEmp)
                    .HasForeignKey(d => d.EmpId)
                    .HasConstraintName("EmployeeAudit_FK");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeAuditUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeAudit_FK_1");

                entity.HasOne(d => d.VerifiedByNavigation)
                    .WithMany(p => p.EmployeeAuditVerifiedByNavigation)
                    .HasForeignKey(d => d.VerifiedBy)
                    .HasConstraintName("EmployeeAudit_FK_2");
            });

            modelBuilder.Entity<EmployeeBank>(entity =>
            {
                entity.Property(e => e.AccountType).IsUnicode(false);

                entity.Property(e => e.BankAccountNumber).IsUnicode(false);

                entity.Property(e => e.BankBranch).IsUnicode(false);

                entity.Property(e => e.BankName).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.IfscCode).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeBankAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeBank_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeBank)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeBank_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeBankEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeBank_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeBankUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeBank_FK_2");
            });

            modelBuilder.Entity<EmployeeCareer>(entity =>
            {
                entity.Property(e => e.EmployeeId).IsUnicode(false);
                entity.Property(e => e.EmployeeCode).IsUnicode(false);
                entity.Property(e => e.AddressingName).IsUnicode(false);
                entity.Property(e => e.AppraisalYear).IsUnicode(false);
                entity.Property(e => e.AppraisalType).IsUnicode(false);
                entity.Property(e => e.Rating).IsUnicode(false);
                entity.Property(e => e.Description).IsUnicode(false);
                entity.Property(e => e.GradeId).IsUnicode(false);
                entity.Property(e => e.DesignationId).IsUnicode(false);
                entity.Property(e => e.DepartmentId).IsUnicode(false);
                entity.Property(e => e.LocationId).IsUnicode(false);
                entity.Property(e => e.DateofChange).IsUnicode(false);
                entity.Property(e => e.EffectiveFrom).IsUnicode(false);
                entity.Property(e => e.ReasonForChange).IsUnicode(false);
                entity.Property(e => e.RnR).IsUnicode(false);
                entity.Property(e => e.Remarks).IsUnicode(false);
                entity.Property(e => e.MovementStatus).IsUnicode(false);
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeCareerAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCareer_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeCareer)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCareer_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCareerEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCareer_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeCareerUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeCareer_FK_2");

            });

            modelBuilder.Entity<EmployeeCompany>(entity =>
            {
                entity.Property(e => e.AddressingName).IsUnicode(false);

                entity.Property(e => e.ConfirmationRemarks).IsUnicode(false);

                entity.Property(e => e.Division).IsUnicode(false);

                entity.Property(e => e.EmployeeCode).IsUnicode(false);

                entity.Property(e => e.LocationBifurcation).IsUnicode(false);

                entity.Property(e => e.LocationForField).IsUnicode(false);

                entity.Property(e => e.OffRoleCode).IsUnicode(false);

                entity.Property(e => e.Photo).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.Property(e => e.StatusCategory).IsUnicode(false);

                entity.Property(e => e.UniqueCode).IsUnicode(false);

                entity.Property(e => e.Vendor).IsUnicode(false);

                entity.Property(e => e.VendorName).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeCompanyAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCompany_FK_1");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("EmployeeCompany_FK_3");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCompany_FK");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("EmployeeCompany_FK_4");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("EmployeeCompany_FK_5");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCompanyEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCompany_FK_10");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.GradeId)
                    .HasConstraintName("EmployeeCompany_FK_6");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.LocationId)
                    .HasConstraintName("EmployeeCompany_FK_7");

                entity.HasOne(d => d.ProductLine)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.ProductLineId)
                    .HasConstraintName("EmployeeCompany_FK_12");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("EmployeeCompany_FK_8");

                entity.HasOne(d => d.ReportingTo)
                    .WithMany(p => p.EmployeeCompanyReportingTo)
                    .HasForeignKey(d => d.ReportingToId)
                    .HasConstraintName("EmployeeCompany_FK_11");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.EmployeeCompany)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("EmployeeCompany_FK_9");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeCompanyUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeCompany_FK_2");
            });

            modelBuilder.Entity<EmployeeCompensation>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeCompensationAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCompensation_FK_2");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeCompensation)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCompensation_FK");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.EmployeeCompensation)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("EmployeeCompensation_FK_5");

                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.EmployeeCompensation)
                    .HasForeignKey(d => d.DesignationId)
                    .HasConstraintName("EmployeeCompensation_FK_6");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeCompensationEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeCompensation_FK_1");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.EmployeeCompensation)
                    .HasForeignKey(d => d.GradeId)
                    .HasConstraintName("EmployeeCompensation_FK_4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeCompensationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeCompensation_FK_3");
            });

            modelBuilder.Entity<EmployeeContact>(entity =>
            {
                entity.Property(e => e.AlternateNumber).IsUnicode(false);

                entity.Property(e => e.ContactNumber).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.OfficialEmailId).IsUnicode(false);

                entity.Property(e => e.OfficialNumber).IsUnicode(false);

                entity.Property(e => e.PersonalEmailId).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeContactAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeContact_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeContact)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeContact_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeContactEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeContact_FK_3");

                entity.HasOne(d => d.PermanentAddress)
                    .WithMany(p => p.EmployeeContactPermanentAddress)
                    .HasForeignKey(d => d.PermanentAddressId)
                    .HasConstraintName("EmployeeContact_FK_12");

                entity.HasOne(d => d.PresentAddress)
                    .WithMany(p => p.EmployeeContactPresentAddress)
                    .HasForeignKey(d => d.PresentAddressId)
                    .HasConstraintName("EmployeeContact_FK_11");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeContactUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeContact_FK_2");
            });

            modelBuilder.Entity<EmployeeDataVerification>(entity =>
            {
                entity.Property(e => e.EmployeeSection).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeDataVerification)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDataVerification_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeDataVerificationEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDataVerification_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeDataVerificationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDataVerification_FK_2");

                entity.HasOne(d => d.VerifiedByNavigation)
                    .WithMany(p => p.EmployeeDataVerificationVerifiedByNavigation)
                    .HasForeignKey(d => d.VerifiedBy)
                    .HasConstraintName("EmployeeDataVerification_FK_1");
            });

            modelBuilder.Entity<EmployeeDocument>(entity =>
            {
                entity.Property(e => e.FileLocation).IsUnicode(false);

                entity.Property(e => e.FileUrl).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Type).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeDocumentAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDocument_FK_5");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeDocument)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDocument_FK_4");

                entity.HasOne(d => d.DocumentType)
                    .WithMany(p => p.EmployeeDocument)
                    .HasForeignKey(d => d.DocumentTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDocument_FK_1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeDocumentEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeDocument_FK");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeDocumentUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeDocument_FK_6");
            });

            modelBuilder.Entity<EmployeeEducation>(entity =>
            {
                entity.Property(e => e.City).IsUnicode(false);

                entity.Property(e => e.CourseName).IsUnicode(false);

                entity.Property(e => e.CourseType).IsUnicode(false);

                entity.Property(e => e.Grade).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Institute).IsUnicode(false);

                entity.Property(e => e.MajorSubject).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeEducationAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeEducation_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeEducation)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeEducation_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeEducationEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("EmployeeEducation_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeEducationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeEducation_FK_2");
            });

            modelBuilder.Entity<EmployeeEmergencyContact>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.ContactNumber).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Relationship).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeEmergencyContactAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeEmergencyContact_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeEmergencyContact)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeEmergencyContact_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeEmergencyContactEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeEmergencyContact_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeEmergencyContactUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeEmergencyContact_FK_2");
            });

            modelBuilder.Entity<EmployeeExit>(entity =>
            {
                entity.Property(e => e.ClearanceComments).IsUnicode(false);

                entity.Property(e => e.Feedback).IsUnicode(false);

                entity.Property(e => e.FeedbackForOthers).IsUnicode(false);

                entity.Property(e => e.HrapprovalFeedback).IsUnicode(false);

                entity.Property(e => e.HrapprovalFeedbackForOthers).IsUnicode(false);

                entity.Property(e => e.L1approvalFeedback).IsUnicode(false);

                entity.Property(e => e.L1approvalFeedbackForOthers).IsUnicode(false);

                entity.Property(e => e.L2approvalFeedback).IsUnicode(false);

                entity.Property(e => e.L2approvalFeedbackForOthers).IsUnicode(false);

                entity.Property(e => e.ResignationReason).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeExitAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExit_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeExit)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EEmployeeExit_FK");

                entity.HasOne(d => d.ConfirmedByNavigation)
                    .WithMany(p => p.EmployeeExitConfirmedByNavigation)
                    .HasForeignKey(d => d.ConfirmedBy)
                    .HasConstraintName("EmployeeExit_FK_4");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeExitEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExit_FK_3");

                entity.HasOne(d => d.HrEmployeeNavigation)
                    .WithMany(p => p.EmployeeExitHrEmployeeNavigation)
                    .HasForeignKey(d => d.HrEmployee)
                    .HasConstraintName("EmployeeExit_FK_5");

                entity.HasOne(d => d.ManagerEmployeeNavigation)
                    .WithMany(p => p.EmployeeExitManagerEmployeeNavigation)
                    .HasForeignKey(d => d.ManagerEmployee)
                    .HasConstraintName("EmployeeExit_FK_6");

                entity.HasOne(d => d.SeniorManagerEmployeeNavigation)
                    .WithMany(p => p.EmployeeExitSeniorManagerEmployeeNavigation)
                    .HasForeignKey(d => d.SeniorManagerEmployee)
                    .HasConstraintName("EmployeeExit_FK_7");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeExitUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeExit_FK_2");
            });

            modelBuilder.Entity<EmployeeExitAnswers>(entity =>
            {
                entity.Property(e => e.Answer).IsUnicode(false);

                entity.HasOne(d => d.EmployeeExit)
                    .WithMany(p => p.EmployeeExitAnswers)
                    .HasForeignKey(d => d.EmployeeExitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitAnswers_FK_4");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeExitAnswersEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitAnswers_FK_3");

                entity.HasOne(d => d.ExitQuestion)
                    .WithMany(p => p.EmployeeExitAnswers)
                    .HasForeignKey(d => d.ExitQuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitAnswers_FK_5");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeExitAnswersUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeExitAnswers_FK_2");
            });

            modelBuilder.Entity<EmployeeExitAsset>(entity =>
            {
                entity.Property(e => e.Hodcomments).IsUnicode(false);

                entity.Property(e => e.Status).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeExitAssetAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitAsset_FK_1");

                entity.HasOne(d => d.AssetOwnerNavigation)
                    .WithMany(p => p.EmployeeExitAssetAssetOwnerNavigation)
                    .HasForeignKey(d => d.AssetOwner)
                    .HasConstraintName("EmployeeExitAsset_FK_4");

                entity.HasOne(d => d.EmployeeAsset)
                    .WithMany(p => p.EmployeeExitAsset)
                    .HasForeignKey(d => d.EmployeeAssetId)
                    .HasConstraintName("EmployeeExitAsset_FK_3");

                entity.HasOne(d => d.EmployeeExit)
                    .WithMany(p => p.EmployeeExitAsset)
                    .HasForeignKey(d => d.EmployeeExitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitAsset_FK");

                entity.HasOne(d => d.ManagerNavigation)
                    .WithMany(p => p.EmployeeExitAssetManagerNavigation)
                    .HasForeignKey(d => d.Manager)
                    .HasConstraintName("EmployeeExitAsset_FK_5");

                entity.HasOne(d => d.SeniorManagerNavigation)
                    .WithMany(p => p.EmployeeExitAssetSeniorManagerNavigation)
                    .HasForeignKey(d => d.SeniorManager)
                    .HasConstraintName("EmployeeExitAsset_FK_6");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeExitAssetUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeExitAsset_FK_2");
            });

            modelBuilder.Entity<EmployeeExitForm>(entity =>
            {
                entity.Property(e => e.AccountNo).IsUnicode(false);

                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.AssociateWhom).IsUnicode(false);

                entity.Property(e => e.BankName).IsUnicode(false);

                entity.Property(e => e.Ctc).IsUnicode(false);

                entity.Property(e => e.Designation).IsUnicode(false);

                entity.Property(e => e.DislikeAboutKai).IsUnicode(false);

                entity.Property(e => e.EmailId).IsUnicode(false);

                entity.Property(e => e.Ifsccode).IsUnicode(false);

                entity.Property(e => e.LikeAboutKai).IsUnicode(false);

                entity.Property(e => e.MobileNumber).IsUnicode(false);

                entity.Property(e => e.ReasonForLeavingKai).IsUnicode(false);

                entity.Property(e => e.TenureInKai).IsUnicode(false);

                entity.Property(e => e.ThingsKaiMustChange).IsUnicode(false);

                entity.Property(e => e.ThingsKaiMustContinue).IsUnicode(false);

                entity.Property(e => e.TotalExperience).IsUnicode(false);

                entity.Property(e => e.WhatPromptedToChange).IsUnicode(false);

                entity.Property(e => e.WhichOrganization).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeExitFormAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitForm_FK_1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeExitFormEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitForm_FK_3");

                entity.HasOne(d => d.Exit)
                    .WithMany(p => p.EmployeeExitForm)
                    .HasForeignKey(d => d.ExitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitForm_FK");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeExitFormUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeExitForm_FK_2");
            });

            modelBuilder.Entity<EmployeeExitHodfeedBackForm>(entity =>
            {
                entity.Property(e => e.AttemptsToRetainEmployee).IsUnicode(false);

                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.EligibleToRehire).IsUnicode(false);

                entity.Property(e => e.IntentionToLeaveKai).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeExitHodfeedBackFormAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitHODFeedBackForm_FK_1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeExitHodfeedBackFormEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitHODFeedBackForm_FK_3");

                entity.HasOne(d => d.Exit)
                    .WithMany(p => p.EmployeeExitHodfeedBackForm)
                    .HasForeignKey(d => d.ExitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitHODFeedBackForm_FK");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeExitHodfeedBackFormUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeExitHODFeedBackForm_FK_2");
            });

            modelBuilder.Entity<EmployeeExitHrfeedBackForm>(entity =>
            {
                entity.Property(e => e.Comments).IsUnicode(false);

                entity.Property(e => e.EmployeeLikeToChange).IsUnicode(false);

                entity.Property(e => e.EmployeeRejoinLater).IsUnicode(false);

                entity.Property(e => e.EmployeeThoughtOnKai).IsUnicode(false);

                entity.Property(e => e.SalaryAndDesignationOffered).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeExitHrfeedBackFormAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitHRFeedBackForm_FK_1");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeExitHrfeedBackFormEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitHRFeedBackForm_FK_3");

                entity.HasOne(d => d.Exit)
                    .WithMany(p => p.EmployeeExitHrfeedBackForm)
                    .HasForeignKey(d => d.ExitId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeExitHRFeedBackForm_FK");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeExitHrfeedBackFormUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeExitHRFeedBackForm_FK_2");
            });

            modelBuilder.Entity<EmployeeFamily>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.Column1).IsUnicode(false);

                entity.Property(e => e.Dob).IsUnicode(false);

                entity.Property(e => e.EmailId).IsUnicode(false);

                entity.Property(e => e.Gender).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.Property(e => e.Occupation).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.Relationship).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeFamilyAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeFamily_FK_5");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeFamily)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeFamily_FK_4");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeFamilyEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeFamily_FK_7");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeFamilyUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeFamily_FK_6");
            });

            modelBuilder.Entity<EmployeeLanguage>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.LanguageName).IsUnicode(false);

                entity.Property(e => e.Level).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeLanguageAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeLanguage_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeLanguage)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeLanguage_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeLanguageEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeLanguage_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeLanguageUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeLanguage_FK_2");
            });

            modelBuilder.Entity<EmployeePersonal>(entity =>
            {
                entity.Property(e => e.BloodGroup).IsUnicode(false);

                entity.Property(e => e.Gender).IsUnicode(false);

                entity.Property(e => e.MaritalStatus).IsUnicode(false);

                entity.Property(e => e.Nationality).IsUnicode(false);

                entity.Property(e => e.PhotoLinkUrl).IsUnicode(false);

                entity.Property(e => e.PhotoUrl).IsUnicode(false);

                entity.Property(e => e.SpecializedTraining).IsUnicode(false);

                entity.Property(e => e.Sports).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeePersonalAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeePersonal_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeePersonal)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeePersonal_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeePersonalEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeePersonal_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeePersonalUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeePersonal_FK_2");
            });

            modelBuilder.Entity<EmployeePreviousCompany>(entity =>
            {
                entity.Property(e => e.Department).IsUnicode(false);

                entity.Property(e => e.Designation).IsUnicode(false);

                entity.Property(e => e.Employer).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.ReasonForChange).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeePreviousCompanyAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeePreviousCompany_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeePreviousCompany)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeePreviousCompany_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeePreviousCompanyEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeePreviousCompany_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeePreviousCompanyUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeePreviousCompany_FK_2");
            });

            modelBuilder.Entity<EmployeeReference>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.Company).IsUnicode(false);

                entity.Property(e => e.Designation).IsUnicode(false);

                entity.Property(e => e.EmailId).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.ReferenceName).IsUnicode(false);

                entity.Property(e => e.Remarks).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeReferenceAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeReference_FK_1");

                entity.HasOne(d => d.CompanyNavigation)
                    .WithMany(p => p.EmployeeReference)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeReference_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeReferenceEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .HasConstraintName("EmployeeReference_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeReferenceUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeReference_FK_2");
            });

            modelBuilder.Entity<EmployeeStatutory>(entity =>
            {
                entity.Property(e => e.AadharName).IsUnicode(false);

                entity.Property(e => e.AadharNumber).IsUnicode(false);

                entity.Property(e => e.DrivingLicenseNumber).IsUnicode(false);

                entity.Property(e => e.EsiNumber).IsUnicode(false);

                entity.Property(e => e.LicIdNumber).IsUnicode(false);

                entity.Property(e => e.PanNumber).IsUnicode(false);

                entity.Property(e => e.PassportNumber).IsUnicode(false);

                entity.Property(e => e.PfNumber).IsUnicode(false);

                entity.Property(e => e.PreviousEmpPension).IsUnicode(false);

                entity.Property(e => e.UanNumber).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.EmployeeStatutoryAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeStatutory_FK_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.EmployeeStatutory)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeStatutory_FK");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.EmployeeStatutoryEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("EmployeeStatutory_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.EmployeeStatutoryUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("EmployeeStatutory_FK_2");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Content).IsUnicode(false);

                entity.Property(e => e.NotificationData).IsUnicode(false);

                entity.HasOne(d => d.NotificationToNavigation)
                    .WithMany(p => p.Notification)
                    .HasForeignKey(d => d.NotificationTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PK_Notification_1");
            });

            modelBuilder.Entity<SettingsAnnouncementType>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAnnouncementTypeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Announcem__added__04E4BC85");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsAnnouncementType)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsAnnouncementType_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAnnouncementTypeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Announcem__updat__05D8E0BE");
            });

            modelBuilder.Entity<SettingsAppraisalMode>(entity =>
            {
                entity.Property(e => e.AppraisalMode).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAppraisalModeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsAppraisalMode_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsAppraisalMode)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsAppraisalMode_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAppraisalModeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsAppraisalMode_FK_1");
            });

            modelBuilder.Entity<SettingsAppraisalQuestion>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Question).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAppraisalQuestionAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__SettingsAppraisalQuestion__addedBy__625A9A57");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsAppraisalQuestion)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsAppraisalQuestion_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAppraisalQuestionUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__SettingsAppraisalQuestion__updated__634EBE90");
            });

            modelBuilder.Entity<SettingsAppraisalRatings>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.RatingTag).IsUnicode(false);

                entity.Property(e => e.RatingTitle).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAppraisalRatingsAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsAppraisalRatings_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsAppraisalRatings)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsAppraisalRatings_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAppraisalRatingsUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsAppraisalRatings_FK_1");
            });

            modelBuilder.Entity<SettingsAppraiseeType>(entity =>
            {
                entity.Property(e => e.AppraiseeType).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAppraiseeTypeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsAppraiseeType_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsAppraiseeType)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsAppraiseeType_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAppraiseeTypeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsAppraiseeType_FK_1");
            });

            modelBuilder.Entity<SettingsAssetTypeOwner>(entity =>
            {
                entity.HasOne(d => d.AssetType)
                    .WithMany(p => p.SettingsAssetTypeOwner)
                    .HasForeignKey(d => d.AssetTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SettingsAssetTypeOwner");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.SettingsAssetTypeOwner)
                    .HasForeignKey(d => d.OwnerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SettingsAssetTypeOwner__1");
            });

            modelBuilder.Entity<SettingsAssetTypes>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAssetTypesAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__AssetTypes__added__04E4BC85");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsAssetTypes)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("AssetTypes_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAssetTypesUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__AssetTypes__updat__05D8E0BE");
            });

            modelBuilder.Entity<SettingsAttachmentType>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsAttachmentTypeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AttachmentType_1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsAttachmentTypeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_AttachmentType_2");
            });

            modelBuilder.Entity<SettingsCategory>(entity =>
            {
                entity.Property(e => e.Category).IsUnicode(false);

                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsCategoryAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("Category_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsCategory)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsCategory_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsCategoryUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("Category_FK_1");
            });

            modelBuilder.Entity<SettingsDepartment>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsDepartmentAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__Departmen__added__18EBB532");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsDepartment)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsDepartment_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsDepartmentUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Departmen__updat__19DFD96B");
            });

            modelBuilder.Entity<SettingsDepartmentDesignation>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsDepartmentDesignationAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__Departmen__added__208CD6FA");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsDepartmentDesignation)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsDepartmentDesignation_FK_3");

                entity.HasOne(d => d.Department)
                    .WithMany(p => p.SettingsDepartmentDesignation)
                    .HasForeignKey(d => d.DepartmentId)
                    .HasConstraintName("FK__Departmen__depar__1EA48E88");

                entity.HasOne(d => d.ReportingTo)
                    .WithMany(p => p.InverseReportingTo)
                    .HasForeignKey(d => d.ReportingToId)
                    .HasConstraintName("FK__Departmen__repor__1F98B2C1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsDepartmentDesignationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Departmen__updat__2180FB33");
            });

            modelBuilder.Entity<SettingsDocumentType>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.DocumentType).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsDocumentTypeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsDocumentType_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsDocumentType)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsDocumentType_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsDocumentTypeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsDocumentType_FK_1");
            });

            modelBuilder.Entity<SettingsExitQuestion>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Question).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsExitQuestionAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__SettingsExitQuestion__addedBy__625A9A57");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsExitQuestion)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsExitQuestion_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsExitQuestionUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__SettingsExitQuestion__updated__634EBE90");
            });

            modelBuilder.Entity<SettingsGrade>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Grade).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsGradeAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("Grade_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsGrade)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsGrade_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsGradeUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("Grade_FK_1");
            });

            modelBuilder.Entity<SettingsHoliday>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.HolidayType).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsHolidayAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__Holiday__addedBy__625A9A57");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsHoliday)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsHoliday_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsHolidayUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Holiday__updated__634EBE90");
            });

            modelBuilder.Entity<SettingsHolidayLocation>(entity =>
            {
                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsHolidayLocation)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsHolidayLocation_FK_2");

                entity.HasOne(d => d.Holiday)
                    .WithMany(p => p.SettingsHolidayLocation)
                    .HasForeignKey(d => d.HolidayId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsHolidayLocation_FK");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.SettingsHolidayLocation)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsHolidayLocation_FK_1");
            });

            modelBuilder.Entity<SettingsLocation>(entity =>
            {
                entity.Property(e => e.Address).IsUnicode(false);

                entity.Property(e => e.Country).IsUnicode(false);

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.GstNumber).IsUnicode(false);

                entity.Property(e => e.Phone).IsUnicode(false);

                entity.Property(e => e.State).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsLocationAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__Location__addedB__47A6A41B");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsLocation)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsLocation_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsLocationUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__Location__update__489AC854");
            });

            modelBuilder.Entity<SettingsProductLine>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.ProductLine).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsProductLineAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsProductLine_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsProductLine)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsProductLine_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsProductLineUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsProductLine_FK_1");
            });

            modelBuilder.Entity<SettingsRegion>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsRegionAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__SettingsRegion");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsRegion)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SettingsRegion_2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsRegionUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__SettingsRegion_1");
            });

            modelBuilder.Entity<SettingsReport>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsReportAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsReport_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsReport)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsReport_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsReportUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_SettingsReport_2");
            });

            modelBuilder.Entity<SettingsReportInputs>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsReportInputsAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsReportInputs_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsReportInputs)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsReportInputs_3");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.SettingsReportInputs)
                    .HasForeignKey(d => d.ReportId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsReportInputs_4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsReportInputsUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_SettingsReportInputs_2");
            });

            modelBuilder.Entity<SettingsRole>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.RoleName).IsUnicode(false);

                entity.Property(e => e.RoleSlug).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsRoleAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsRole_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsRole)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsRole_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsRoleUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsRole_FK_1");
            });

            modelBuilder.Entity<SettingsModuleAccess>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.ModuleID).IsUnicode(false);

                entity.Property(e => e.RoleId).IsUnicode(false);
                entity.Property(e => e.Ismanager).IsUnicode(false);
                entity.Property(e => e.EmployeeId).IsUnicode(false);
                entity.Property(e => e.CanAccess).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsModuleAccessAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsModuleAccess_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsModuleAccess)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsModuleAccess_FK_1");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsModuleAccessUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsModuleAccess_FK_2");

            });
            modelBuilder.Entity<SettingsRoleActionAccess>(entity =>
            {
                entity.HasOne(d => d.Action)
                    .WithMany(p => p.SettingsRoleActionAccess)
                    .HasForeignKey(d => d.ActionId)
                    .HasConstraintName("FK__RoleActio__actio__4C6B5938");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsRoleActionAccess)
                    .HasForeignKey(d => d.CompanyId)
                    .HasConstraintName("SettingsRoleActionAccess_FK_3");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.SettingsRoleActionAccess)
                    .HasForeignKey(d => d.Roleid)
                    .HasConstraintName("SettingsRoleActionAccess_FK");
            });


            modelBuilder.Entity<SettingsTeam>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsTeamAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__SettingsTeam");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTeam)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SettingsTeam_2");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsTeamUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__SettingsTeam_1");
            });

            modelBuilder.Entity<SettingsTicketCategory>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsTicketCategoryAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__TicketCat__added__55009F39");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTicketCategory)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketCategory_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsTicketCategoryUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__TicketCat__updat__55F4C372");
            });

            modelBuilder.Entity<SettingsTicketCategoryOwner>(entity =>
            {
                entity.HasOne(d => d.Category)
                    .WithMany(p => p.SettingsTicketCategoryOwner)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketCategoryOwner_FK_3");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTicketCategoryOwner)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketCategoryOwner_FK_2");

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.SettingsTicketCategoryOwner)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SettingsTicketCategoryOwner_1");
            });

            modelBuilder.Entity<SettingsTicketFaq>(entity =>
            {
                entity.Property(e => e.Explanation).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsTicketFaqAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("SettingsFaq_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTicketFaq)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketFaq_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsTicketFaqUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsFaq_FK_1");
            });

            modelBuilder.Entity<SettingsTicketStatus>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Name).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsTicketStatusAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketStatus_5");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTicketStatus)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketStatus_4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsTicketStatusUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("SettingsTicketStatus_6");
            });

            modelBuilder.Entity<SettingsTicketSubCategory>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsTicketSubCategoryAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__TicketSub__added__59C55456");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.SettingsTicketSubCategory)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("FK__TicketSub__categ__58D1301D");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTicketSubCategory)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTicketSubCategory_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsTicketSubCategoryUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__TicketSub__updat__5AB9788F");
            });

            modelBuilder.Entity<SettingsTraining>(entity =>
            {
                entity.Property(e => e.TrainingCode).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.SettingsTrainingAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .HasConstraintName("FK__SettingsTraining_1");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTraining)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__SettingsTraining_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.SettingsTrainingUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK__SettingsTraining_2");
            });

            modelBuilder.Entity<SettingsTrainingFeedbackQuestion>(entity =>
            {
                entity.Property(e => e.Question).IsUnicode(false);
            });

            modelBuilder.Entity<SettingsTrainingGrade>(entity =>
            {
                entity.HasOne(d => d.Company)
                    .WithMany(p => p.SettingsTrainingGrade)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTrainingGrade_2");

                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.SettingsTrainingGrade)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTrainingGrade_3");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.SettingsTrainingGrade)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SettingsTrainingGrade_1");
            });

            modelBuilder.Entity<Task>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Priority).IsUnicode(false);

                entity.Property(e => e.TaskContent).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.TaskAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_FK_5");

                entity.HasOne(d => d.AssignedToNavigation)
                    .WithMany(p => p.TaskAssignedToNavigation)
                    .HasForeignKey(d => d.AssignedTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_FK");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Task)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Task_FK_4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.TaskUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("Task_FK_6");
            });

            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.TaskComment)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TaskComment_FK_5");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TaskComment)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TaskComment_FK_4");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskComment)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TaskComment_FK");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.Property(e => e.Explanation).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.TicketAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Ticket_7");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.CategoryId)
                    .HasConstraintName("Ticket_9");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Ticket_4");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Ticket_5");

                entity.HasOne(d => d.SubCategory)
                    .WithMany(p => p.Ticket)
                    .HasForeignKey(d => d.SubCategoryId)
                    .HasConstraintName("Ticket_10");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.TicketUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("Ticket_8");
            });

            modelBuilder.Entity<TicketAttachment>(entity =>
            {
                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.TicketAttachmentAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketAttachment_1");

                entity.HasOne(d => d.Attachment)
                    .WithMany(p => p.TicketAttachment)
                    .HasForeignKey(d => d.AttachmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketAttachment_2");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketAttachment)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TicketAttachment_4");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.TicketAttachmentUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_TicketAttachment_3");
            });

            modelBuilder.Entity<TicketComment>(entity =>
            {
                entity.Property(e => e.Guid).IsUnicode(false);

                entity.HasOne(d => d.Comment)
                    .WithMany(p => p.TicketComment)
                    .HasForeignKey(d => d.CommentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketComment_FK_5");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TicketComment)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketComment_FK_4");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketComment)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketComment_FK");
            });

            modelBuilder.Entity<TicketStatus>(entity =>
            {
                entity.HasOne(d => d.ChangedByNavigation)
                    .WithMany(p => p.TicketStatus)
                    .HasForeignKey(d => d.ChangedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketStatus_6");

                entity.HasOne(d => d.Company)
                    .WithMany(p => p.TicketStatus)
                    .HasForeignKey(d => d.CompanyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketStatus_4");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TicketStatus)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketStatus_5");

                entity.HasOne(d => d.Ticket)
                    .WithMany(p => p.TicketStatus)
                    .HasForeignKey(d => d.TicketId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TicketStatus_3");
            });

            modelBuilder.Entity<Training>(entity =>
            {
                entity.Property(e => e.Category).IsUnicode(false);

                entity.Property(e => e.Code).IsUnicode(false);

                entity.Property(e => e.OtherLocation).IsUnicode(false);

                entity.Property(e => e.TimeOfDay).IsUnicode(false);

                entity.Property(e => e.Title).IsUnicode(false);

                entity.Property(e => e.TrainerName).IsUnicode(false);

                entity.HasOne(d => d.AddedByNavigation)
                    .WithMany(p => p.TrainingAddedByNavigation)
                    .HasForeignKey(d => d.AddedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Training_FK_1");

                entity.HasOne(d => d.TrainingNavigation)
                    .WithMany(p => p.Training)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Training_FK");

                entity.HasOne(d => d.TrainingLocationNavigation)
                    .WithMany(p => p.Training)
                    .HasForeignKey(d => d.TrainingLocation)
                    .HasConstraintName("Training_FK_3");

                entity.HasOne(d => d.UpdatedByNavigation)
                    .WithMany(p => p.TrainingUpdatedByNavigation)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("Training_FK_2");
            });

            modelBuilder.Entity<TrainingAttendance>(entity =>
            {
                entity.Property(e => e.Remark).IsUnicode(false);

                entity.HasOne(d => d.TrainingDateNavigation)
                    .WithMany(p => p.TrainingAttendance)
                    .HasForeignKey(d => d.TrainingDate)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingAttendance_FK_1");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingAttendance)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingAttendance_FK");

                entity.HasOne(d => d.TrainingNomineeNavigation)
                    .WithMany(p => p.TrainingAttendance)
                    .HasForeignKey(d => d.TrainingNominee)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingAttendance_FK_2");
            });

            modelBuilder.Entity<TrainingDate>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingDate)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingDate_FK");
            });

            modelBuilder.Entity<TrainingDepartment>(entity =>
            {
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.TrainingDepartment)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingDepartment_SettingsDepartment");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingDepartment)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingDepartment_Training");
            });

            modelBuilder.Entity<TrainingDesignation>(entity =>
            {
                entity.HasOne(d => d.Designation)
                    .WithMany(p => p.TrainingDesignation)
                    .HasForeignKey(d => d.DesignationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingDesignation_SettingsDepartmentDesignation");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingDesignation)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingDesignation_Training");
            });

            modelBuilder.Entity<TrainingFeedback>(entity =>
            {
                entity.Property(e => e.Answer).IsUnicode(false);

                entity.HasOne(d => d.Nominee)
                    .WithMany(p => p.TrainingFeedback)
                    .HasForeignKey(d => d.NomineeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingFeedback_FK_1");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.TrainingFeedback)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingFeedback_FK_3");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingFeedback)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingFeedback_FK");
            });

            modelBuilder.Entity<TrainingGrade>(entity =>
            {
                entity.HasOne(d => d.Grade)
                    .WithMany(p => p.TrainingGrade)
                    .HasForeignKey(d => d.GradeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingGrade_SettingsGrade");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingGrade)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingGrade_Training");
            });

            modelBuilder.Entity<TrainingLocation>(entity =>
            {
                entity.HasOne(d => d.Location)
                    .WithMany(p => p.TrainingLocation)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingLocation_SettingsLocation");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingLocation1)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingLocation_Training");
            });

            modelBuilder.Entity<TrainingNominees>(entity =>
            {
                entity.Property(e => e.FeedbackContent).IsUnicode(false);

                entity.Property(e => e.Guid).IsUnicode(false);

                entity.Property(e => e.RejectedReason).IsUnicode(false);

                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.TrainingNomineesEmployee)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingNominees_FK_1");

                entity.HasOne(d => d.Hr)
                    .WithMany(p => p.TrainingNomineesHr)
                    .HasForeignKey(d => d.HrId)
                    .HasConstraintName("TrainingNominees_FK_3");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.TrainingNomineesManager)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("TrainingNominees_FK_2");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingNominees)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("TrainingNominees_FK");
            });

            modelBuilder.Entity<TrainingOrganizer>(entity =>
            {
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.TrainingOrganizer)
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingOrganizer_Employee");

                entity.HasOne(d => d.Training)
                    .WithMany(p => p.TrainingOrganizer)
                    .HasForeignKey(d => d.TrainingId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TrainingOrganizer_Training");
            });
        }
    }
}
