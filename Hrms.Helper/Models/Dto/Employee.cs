using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Hrms.Helper.Models.Dto
{

    public class EmployeeDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public bool CanLogin { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public bool IsSelfFilled { get; set; }
        public DateTime SelfFilledOn { get; set; }
        public bool IsVerificationPending { get; set; }
    }

    public class EmployeeRptBdayDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? BdayDate { get; set; }
    }

    public class EmployeeRptWdayDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? WdayDate { get; set; }
    }
    public class HeadCountrptDto
    {
        public string Type { get; set; }
        public int OnRollCount { get; set; }
        public int OffRollCount { get; set; }
        public int Trainee { get; set; }
        public int CasualorTemp { get; set; }
        public int Expatriate { get; set; }
    }

    public class AddandExitrptDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public DateTime? addExitDate { get; set; }
    }

    public class ProbationrptDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? DateofJoing { get; set; }
        public DateTime? ConfirmationDueDate { get; set; }
        public int? ExtenstionPeriod { get; set; }
        public DateTime? ConfirmationDueDateExtended { get; set; }
        public string ConfirmationRemarks { get; set; }
    }

    public class BasicrptDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? DateofJoing { get; set; }
        public string RMCode { get; set; }
        public string RMEmailId { get; set; }
        public string RMName { get; set; }
        public string SMCode { get; set; }
        public string SMEmailId { get; set; }
        public string SMName { get; set; }
        public string PerEmailId { get; set; }
        public string OffPhone { get; set; }
        public string PerPhone { get; set; }
        public string PresentAddr { get; set; }
        public string PermenantAddr { get; set; }

    }

    public class ResignedrptDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? DateofResignation { get; set; }
        public DateTime? DateofConfirmation { get; set; }
        public string Desired { get; set; }
        public string ExitStatus { get; set; }
        public string ExitClearanceStatus { get; set; }
    }

    public class ObjectiverptDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? SelfSubmitted { get; set; }
        public DateTime? RMSubmitted { get; set; }
        public DateTime? HODSubmitted { get; set; }
    }
    public class EmployeeRehireDto {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? DateofJoing { get; set; }
        public DateTime? DateofRelieving { get; set; }
        public string Desired { get; set; }
        public  string Rehired { get; set; }

    }
    public class CTCrptDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string LocationBifurcation { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public DateTime? DateofJoing { get; set; }
        public string EducationDetails { get; set; }
        public int TotalyrExp { get; set; }
        public double CTC { get; set; }
    }

    public class EmployeeBankDto
    {
        public string EmployeeBankId { get; set; }
        public string BankName { get; set; }
        public string Branch { get; set; }
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string IfscCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }

    public class EmployeeCareerDto
    {
        public string EmployeeCareerId { get; set; }
        public long AppraisalYear { get; set; }
        public string AppraisalType { get; set; }
        public double Rating { get; set; }
        public string Grade { get; set; }
        public string Description { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public DateTime? DateofChange { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public string ReasonForChange { get; set; }
        public string RnR { get; set; }
        public string Remarks { get; set; }
        public string MovementStatus { get; set; }
        public bool IsActive { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PreLocation { get; set; }
        public string PreDepartment { get; set; }
    }

    public class EmployeeDocumentDto
    {
        public string EmployeeDocumentId { get; set; }
        public bool IsActive { get; set; }
        public string DocumentTypeId { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public string FileLocation { get; set; }
        public string FileUrl { get; set; }
    }

    public class EmployeeEducationDto
    {
        public string EmployeeEducationId { get; set; }
        public bool IsActive { get; set; }
        public string Institute { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CourseName { get; set; }
        public int? StartedYear { get; set; }
        public int? CompletedYear { get; set; }
        public int? CourseDuration { get; set; }
        public string MajorSubject { get; set; }
        public string Grade { get; set; }
        public string CourseType { get; set; }
        public double? Percentage { get; set; }
    }

    public class EmployeePreviousCompanyDto
    {
        public string PreviousCompanyId { get; set; }
        public string Employer { get; set; }
        public string Designation { get; set; }
        public int Duration { get; set; }
        public string Department { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public DateTime? DateOfExit { get; set; }
        public string ReasonForChange { get; set; }
        public double Ctc { get; set; }
        public bool IsActive { get; set; }
    }

    public class EmployeeFamilyDto
    {
        public string EmployeeFamilyId { get; set; }
        public bool IsActive { get; set; }
        public string Relation { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Dob { get; set; }
        public string Occupation { get; set; }
        public bool IsEmergencyContact { get; set; }
        public bool IsDependant { get; set; }
        public bool IsAlive { get; set; }
        public string Gender { get; set; }
        public bool IsOptedForMediclaim { get; set; }
    }

    public class EmployeeReferenceDto
    {
        public string EmployeeReferenceId { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Remarks { get; set; }
        public string Address { get; set; }
    }

    public class EmployeeLanguageDto
    {
        public string EmployeeLanguageId { get; set; }
        public bool IsActive { get; set; }
        public string Language { get; set; }
        public bool? CanSpeak { get; set; }
        public bool? CanWrite { get; set; }
        public bool? CanRead { get; set; }
        public string Level { get; set; }
    }

    public class EmployeeAppraisalDto
    {
        public string EmployeeAppraisalId { get; set; }
        public string Title { get; set; }
        public bool IsSelf { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsManager { get; set; }
        public bool IsL2Manager { get; set; }
        public DateTime? SelfAppraisalDoneOn { get; set; }
        public DateTime? AppraisalClosedOn { get; set; }
        public DateTime? RmSubmittedOn { get; set; }
        public DateTime? L2SubmittedOn { get; set; }
        public string Rating { get; set; }
        public double InternalSelf { get; set; }
        public double InternalMgmt { get; set; }
        public double InternalL2 { get; set; }
        public bool ShowCalculation { get; set; }
        public string Category { get; set; }
        public string Mode { get; set; }
        public string CalculationMethod { get; set; }
        public DateTime? HrSubmittedOn { get; set; }
        public DateTime? SelfObjectiveSubmittedOn { get; set; }
        public DateTime? RmObjectiveSubmittedOn { get; set; }
        public DateTime? L2ObjectiveSubmittedOn { get; set; }
        public DateTime? HrObjectiveSubmittedOn { get; set; }
        public List<EmployeeAppraisalSelfAnswerDto> SelfAnswers { get; set; }
        public List<EmployeeAppraisalBusinessNeedAnswerDto> BusinessNeeds { get; set; }
        public List<EmployeeAppraisalQuestionAnswerDto> Questions { get; set; }
        public List<EmployeeAppraisalFeedbackDto> Feedbacks { get; set; }
        public List<AppraisalTrainingDto> Trainings { get; set; }
        public string Grade { get; set; }
        public string EmployeeCateogry { get; set; }
        public bool IsPromotionRecommended { get; set; }
        public bool IsFitmentRecommended { get; set; }
        public string TrainingComments { get; set; }
    }

    public class AppraisalTrainingDto
    {
        public string TrainingId { get; set; }
        public string AddedBy { get; set; }
        public bool IsSelf { get; set; }
    }

    public class EmployeeAppraisalSelfAnswerDto
    {
        public string SelfAppraisalAnswerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Weightage { get; set; }
        public double SelfWeightage { get; set; }
        public double ManagementWeightage { get; set; }
        public double L2Weightage { get; set; }
        public bool IsActive { get; set; }
        public long? AppraisalMode { get; set; }
    }

    public class EmployeeAppraisalBusinessNeedAnswerDto
    {
        public string BusinessNeedAnswerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Weightage { get; set; }
        public double SelfWeightage { get; set; }
        public double ManagementWeightage { get; set; }
        public bool IsActive { get; set; }
        public double L2Weightage { get; set; }
        public long? AppraisalMode { get; set; }
    }

    public class EmployeeAppraisalQuestionAnswerDto
    {
        public string EmployeeAnswerId { get; set; }
        public string EmployeeQuestionId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Weightage { get; set; }
        public double SelfWeightage { get; set; }
        public double ManagementWeightage { get; set; }
        public string Answer { get; set; }
        public double L2Weightage { get; set; }
    }

    public class EmployeeAppraisalFeedbackDto
    {
        public string EmployeeFeedbackId { get; set; }
        public string GivenBy { get; set; }
        public string GivenByName { get; set; }
        public DateTime? GivenOn { get; set; }
        public string Feedback { get; set; }
        public string AppraiseeType { get; set; }
        public long? AppraisalMode { get; set; }
    }

    public class EmployeeAddressDto
    {
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Pincode { get; set; }
        public string DoorNo { get; set; }
        public string Street { get; set; }
        public string Village { get; set; }
        public string Landmark { get; set; }
        public string District { get; set; }
    }

    public class EmployeeBaseInfoDto
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
    }

    public class EmployeeDataVerificationDto
    {
        public string Section { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? VerifiedOn { get; set; }
        public string VerifiedBy { get; set; }
    }

    public class EmployeesReportingDto
    {
        public string EmployeeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Grade { get; set; }
        public string Region { get; set; }
        public string Team { get; set; }
        public DateTime? SelfFilledOn { get; set; }
        public DateTime? ManagerFilledOn { get; set; }
        public DateTime? HrFilledOn { get; set; }
        public DateTime? L2FilledOn { get; set; }
        public string Rating { get; set; }
        public string AppraisalName { get; set; }
        public string ManagerName { get; set; }
        public string L2ManagerName { get; set; }
        public bool IsReportingToMe { get; set; }

        public DateTime? SelfObjectiveFilledOn { get; set; }
        public DateTime? ManagerObjectiveFilledOn { get; set; }
        public DateTime? L2ObjectiveFilledOn { get; set; }
        public DateTime? HrObjectiveFilledOn { get; set; }
        public DateTime? SelfVariableFilledOn { get; set; }
        public DateTime? ManagerVariableFilledOn { get; set; }
        public DateTime? L2VariableFilledOn { get; set; }
        public DateTime? HrVariableFilledOn { get; set; }
        public long? AppraisalMode { get; set; }
    }

    public class EmployeeCardDto
    {
        public string EmployeeId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Code { get; set; }
        public string Manager { get; set; }
        public string BirthDate { get; set; }
        public List<EmployeeCardDto> Children { get; set; }
    }

    public class EmployeeAssetDto
    {
        public string EmployeeAssetId { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string AssetUniqueId { get; set; }
        public DateTime? GivenOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }

    public class EmployeeExitQuestionDto
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class EmployeeSigningAssetDto
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeAssetId { get; set; }
        public string AssetId { get; set; }
        public string AssetName { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string AssetUniqueId { get; set; }
        public DateTime? GivenOn { get; set; }
        public DateTime? Doj { get; set; }
        public string LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }

    public class EmployeeCompensationDto
    {
        public bool IsOnRoll { get; set; }
        public string EmployeeCompensationId { get; set; }
        public int Year { get; set; }
        public double AnnualBasic { get; set; }
        public double AnnualHra { get; set; }
        public double AnnualConvAllow { get; set; }
        public double AnnualSplAllow { get; set; }
        public double AnnualMedAllow { get; set; }
        public double AnnualLta { get; set; }
        public double AnnualWashing { get; set; }
        public double AnnualChildEdu { get; set; }
        public double AnnualGross { get; set; }
        public double StatutoryBonus { get; set; }
        public double AnnualVarBonus { get; set; }
        public double AnnualVarBonusPaid1 { get; set; }
        public double AnnualVarBonusPaid2 { get; set; }
        public double AnnualAccidIns { get; set; }
        public double AnnualHealthIns { get; set; }
        public double AnnualGratuity { get; set; }
        public double AnnualPf { get; set; }
        public double AnnualEsi { get; set; }
        public double AnnualCtc { get; set; }
        public double VendorCharges { get; set; }
        public double OtherBenefits { get; set; }
        public double OffrollCtc { get; set; }
        public bool IsActive { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string GradeId { get; set; }
    }

    public class EmployeeTrainingDto
    {
        public string Attendance { get; set; }
        public string Effectiveness { get; set; }
        public string NomineeId { get; set; }
        public string EmployeeName { get; set; }
        public string TrainingId { get; set; }
        public string TrainingName { get; set; }
        public string Organizers { get; set; }
        public string TrainerName { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }
        public bool IsFeedbackClosed { get; set; }
        public bool? IsSelfAccepted { get; set; }
        public bool? IsMangerAccepted { get; set; }
        public bool? IsHrAccepted { get; set; }
        public bool IsRejected { get; set; }
        public bool IsFeedbackCompleted { get; set; }
        public DateTime? SelfUpdatedOn { get; set; }
        public DateTime? ManagerUpdatedOn { get; set; }
        public DateTime? HrUpdatedOn { get; set; }
        public string ManagerName { get; set; }
        public string HrName { get; set; }
    }

    public class EmployeeTrainingDateDto
    {
        public DateTime? Date { get; set; }
        public bool IsAttended { get; set; }
    }


    public class EmployeeExitFormDto
    {
        public string TenureInKai { get; set; }
        public string TotalExperience { get; set; }
        public string LikeAboutKai { get; set; }
        public string DislikeAboutKai { get; set; }
        public string ThingsKaiMustChange { get; set; }
        public string ThingsKaiMustContinue { get; set; }
        public string WhatPromptedToChange { get; set; }
        public string ReasonForLeavingKai { get; set; }
        public bool? RejoinKaiLater { get; set; }
        public string AssociateWhom { get; set; }
        public string WhichOrganization { get; set; }
        public string Designation { get; set; }
        public string Ctc { get; set; }
        public string EmailId { get; set; }
        public string MobileNumber { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public string Address { get; set; }
    }

    public class HODFeedBackFormDto
    {
        public bool IsDesiredAttrition { get; set; }
        public string IntentionToLeaveKai { get; set; }
        public string AttemptsToRetainEmployee { get; set; }
        public string EligibleToRehire { get; set; }
        public string Comments { get; set; }
    }

    public class HRFeedBackFormDto
    {
        public string EmployeeThoughtOnKai { get; set; }
        public string EmployeeLikeToChange { get; set; }
        public string EmployeeRejoinLater { get; set; }
        public string SalaryAndDesignationOffered { get; set; }
        public string Comments { get; set; }
    }
}