using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class ChangePasswordResponse : BaseResponse
    {
        public bool IsPasswordChanged { get; set; }
    }

    public class NotificationListResponse : BaseResponse
    {
        public List<NotificationDto> Notifications { get; set; }
    }

    public class EmployeeListResponse : BaseResponse
    {
        public List<EmployeeDto> Employees { get; set; }
    }

    public class EmployeeRptBdayResponse : BaseResponse
    {
        public List<EmployeeRptBdayDto> EmployeeRptBday { get; set; }
    }

    public class EmployeeRptWdayResponse : BaseResponse
    {
        public List<EmployeeRptWdayDto> EmployeeRptWday { get; set; }
    }

    public class EmployeeRptHeadCountResponse : BaseResponse
    {
        public List<HeadCountrptDto> LocationWiseRptHeadCount { get; set; }
        public List<HeadCountrptDto> DepartmentWiseRptHeadCount { get; set; }
        public List<HeadCountrptDto> GradeWiseRptHeadCount { get; set; }
        public List<HeadCountrptDto> DivisionWiseRptHeadCount { get; set; }
    }

    public class EmployeeRptAddandExitResponse : BaseResponse
    {
        public List<AddandExitrptDto> AdditionDetails { get; set; }
        public List<AddandExitrptDto> ExitDetails { get; set; }
    }

    public class EmployeeRptProbationResponse : BaseResponse
    {
        public List<ProbationrptDto> ProbationDetails { get; set; }
    }
    public class EmployeeRptBasicResponse : BaseResponse
    {
        public List<BasicrptDto> EmployeebasicDetails { get; set; }
    }
    public class EmployeeRptResignedResponse : BaseResponse
    {
        public List<ResignedrptDto> EmployeeresignedDetails { get; set; }
    }
    public class EmployeeRptObjectiveResponse : BaseResponse
    {
        public List<ObjectiverptDto> EmployeeObjectiveDetails { get; set; }
        public List<ObjectiverptDto> EmployeeVariableBonusDetails { get; set; }
        public List<ObjectiverptDto> EmployeeAppraisalDetails { get; set; }
    }
    public class EmployeeRptCTCResponse : BaseResponse
    {
        public List<CTCrptDto> EmployeeCTCDetails { get; set; }

    }
    public class EmployeeRptCompensationResponse : BaseResponse
    {
        public List<CompensationrptDto> EmployeeCompensationDetails { get; set; }

    }
    public class EmployeeRptMasterResponse : BaseResponse
    {
        public List<EmpMasterrptDto> EmployeeRptMasterDetails { get; set; }

    }

    public class EmployeeRehireResponse : BaseResponse
    {
        public List<EmployeeRehireDto> EmployeeRehireDetails { get; set; }
    }
    public class CreateEmployeeResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public bool IsCreated { get; set; }
    }

    public class EmployeeExistResponse : BaseResponse
    {
        public bool IsExist { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Role { get; set; }
        public bool HasReportees { get; set; }
        public bool HasAssetSignings { get; set; }
    }

    public class EmployeeBaseInfoResponse : BaseResponse
    {
        public List<EmployeeBaseInfoDto> Employees { get; set; }
    }

    public class GetEmployeeAccountResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string UniqueCode { get; set; }
        public string LoginEmail { get; set; }
        public string Name { get; set; }
        public string RoleId { get; set; }
        public bool CanLogin { get; set; }
        public string Status { get; set; }
        public string AddressingName { get; set; }
        public string OffRoleCode { get; set; }
        public string Grade { get; set; }
    }

    public class GetEmployeePersonalResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public DateTime? RecordDob { get; set; }
        public DateTime? ActualDob { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string Sports { get; set; }
        public string SpecializedTraining { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public string PhotoUrl { get; set; }
        public string PhotoLinkUrl { get; set; }
        public bool HideBirthday { get; set; }
    }

    public class GetEmployeeCompanyResponse : BaseResponse
    {
        public bool IsCircularManager { get; set; }
        public List<EmployeeCardDto> ManagerList { get; set; }
        public string EmployeeId { get; set; }
        public string Status { get; set; }
        public string StatusCategory { get; set; }
        public string UniqueCode { get; set; }
        public string EmployeeCode { get; set; }
        public string OffRoleCode { get; set; }
        public string AddressingName { get; set; }
        public string RegionId { get; set; }
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
        public string TeamId { get; set; }
        public string LocationId { get; set; }
        public string CategoryId { get; set; }
        public string GradeId { get; set; }
        public DateTime? Doj { get; set; }
        public DateTime? ProbationStartDate { get; set; }
        public DateTime? ProbationEndDate { get; set; }
        public int ProbationExtraDays { get; set; }
        public string ConfirmationRemarks { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? ConfirmedOn { get; set; }
        public string Vendor { get; set; }
        public string ReportingToId { get; set; }
        public string ReportingToName { get; set; }
        public string Division { get; set; }

        public string LocationBifurcation { get; set; }
        public string LocationForField { get; set; }
    }

    public class GetEmployeeStatutoryResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public string PanNumber { get; set; }
        public string PfNumber { get; set; }
        public string UanNumber { get; set; }
        public string PreviousEmployeePensionNumber { get; set; }
        public string AadharNumber { get; set; }
        public string AadharName { get; set; }
        public string DrivingLicenseNumber { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? DrivingLicenseValidity { get; set; }
        public DateTime? PassportValidity { get; set; }
        public string EsiNumber { get; set; }
        public string LicIdNumber { get; set; }
    }

    public class GetEmployeeContactResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public EmployeeAddressDto PresentAddress { get; set; }
        public EmployeeAddressDto PermanentAddress { get; set; }
        public string ContactNumber { get; set; }
        public string AlternateContactNumber { get; set; }
        public string PersonalEmail { get; set; }
        public string OfficialEmail { get; set; }
        public string OfficialContactNumber { get; set; }
        public bool PermanentAddressSame { get; set; }
    }

    public class GetEmployeeBankResponse : BaseResponse
    {
        public string EmployeeId { get; set; }

        public List<EmployeeBankDto> EmployeeBanks { get; set; }
    }

    public class GetEmployeeCareerResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public List<EmployeeCareerDto> EmployeeCareers { get; set; }
    }

    public class GetEmployeeEducationResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public List<EmployeeEducationDto> EmployeeEducation { get; set; }
    }

    public class GetEmployeeFamilyResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public List<EmployeeFamilyDto> EmployeeFamily { get; set; }
    }

    public class GetEmployeeLanguageResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public List<EmployeeLanguageDto> EmployeeLanguage { get; set; }
    }

    public class GetEmployeePreviousCompanyResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public DateTime? CurrentDateOfJoin { get; set; }
        public List<EmployeePreviousCompanyDto> PreviousCompanies { get; set; }
    }

    public class GetEmployeeReferenceResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public bool CanAdd { get; set; }
        public List<EmployeeReferenceDto> EmployeeReference { get; set; }
    }

    public class GetEmployeeDocumentsResponse : BaseResponse
    {
        public bool IsAllowed { get; set; }
        public string EmployeeId { get; set; }
        public List<EmployeeDocumentDto> EmployeeDocuments { get; set; }
    }

    public class GetEmployeeAppraisalResponse : BaseResponse
    {
        public List<EmployeeAppraisalDto> Appraisals { get; set; }

        public List<EmployeeDocumentDto> AppraisalObjectiveDocuments { get; set; }
        public bool IsAllowed { get; set; }
    }

    public class GetEmployeeAppraisalDetailsResponse : BaseResponse
    {
        public EmployeeAppraisalDto Appraisal { get; set; }
    }

    public class GetEmployeeAppraisalFeedbackResponse : BaseResponse
    {
        public string EmployeeAppraisalId { get; set; }
        public List<EmployeeAppraisalFeedbackDto> Feedbacks { get; set; }
    }

    public class GetEmployeeAppraisalQuestionsResponse : BaseResponse
    {
        public string EmployeeAppraisalId { get; set; }
        public List<EmployeeAppraisalSelfAnswerDto> SelfAnswers { get; set; }
        public List<EmployeeAppraisalQuestionAnswerDto> Questions { get; set; }
    }
    public class EmployeeReportingToResponse : BaseResponse
    {
        public List<EmployeesReportingDto> Employees { get; set; }
    }

    public class EmployeeDataVerificationResponse : BaseResponse
    {
        public List<EmployeeDataVerificationDto> Verifications { get; set; }
    }

    public class EmployeeCardResponse : BaseResponse
    {
        public List<EmployeeCardDto> Employees { get; set; }
    }

    public class GetEmployeeAssetResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public bool AssetCodeTaken { get; set; }
        public string AssetUniqueId { get; set; }
        public DateTime Doj { get; set; }
        public List<EmployeeAssetDto> Assets { get; set; }
    }
    public class GetEmployeeSigningsResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public DateTime Doj { get; set; }
        public bool AssetCodeTaken { get; set; }
        public string AssetUniqueId { get; set; }
        public List<EmployeeSigningAssetDto> Assets { get; set; }
    }
    public class EmployeeCompensationResponse : BaseResponse
    {
        public bool IsAllowed { get; set; }
        public bool IsOnRoll { get; set; }
        public string EmployeeId { get; set; }
        public List<EmployeeCompensationDto> EmployeeCompensation { get; set; }
    }

    public class EmployeeTrainingResponse : BaseResponse
    {
        public string EmployeeId { get; set; }
        public List<EmployeeTrainingDto> MyTrainings { get; set; }
        public List<EmployeeTrainingDto> TrainingsForMe { get; set; }
        public List<EmployeeTrainingDto> TrainingForReportees { get; set; }
    }

    public class EmployeeExitDto
    {
        public long ExitId { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public string EmployeeResignationReason { get; set; }
        public DateTime ResignationDate { get; set; }
        public DateTime? RelievingDate { get; set; }
        public string Feedback { get; set; }
        public long? ManangerId { get; set; }
        public string ManangerName { get; set; }
        public string ManagerEmailId { get; set; }
        public long? SeniorManangerId { get; set; }
        public string SeniorManangerName { get; set; }
        public string SeniorManangerEmailId { get; set; }
        public long? HrId { get; set; }
        public string HRName { get; set; }
        public bool IsRevoked { get; set; }
        public string Status { get; set; }
        public bool EmployeeHasCompanyAsset { get; set; }
        public bool IsEmployeeExitFormSubmitted { get; set; }
        public bool IsHODFeedBackFormSubmitted { get; set; }
        public bool IsHRFeedBackFormSubmitted { get; set; }
        public bool IsAssetHandOverCompleted { get; set; }
        public DateTime? PreferredRelievingDate { get; set; }
        public string L1ApprovalFeedback { get; set; }
        public string L2ApprovalFeedback { get; set; }
        public string HRApprovalFeedback { get; set; }
        public string L1ApprovalFeedbackForOthers { get; set; }
        public string L2ApprovalFeedbackForOthers { get; set; }
        public string HRApprovalFeedbackForOthers { get; set; }
        public string FeedbackForOthers { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public string IFSCCode { get; set; }
        public DateTime? RelievingDateAsPerPolicy { get; set; }
        public string Address { get; set; }
        public double ShortfallDays { get; set; }

        public DateTime AddedOn { get; set; }
        public string AddedCode { get; set; }
        public string AddedName { get; set; }

        public DateTime? UpdatedOn { get; set; }
        public string UpdatedCode { get; set; }
        public string UpdatedName { get; set; }

        public DateTime? UpdatedOnRM { get; set; }
        public string UpdatedCodeRM { get; set; }
        public string UpdatedNameRM { get; set; }

        public DateTime? UpdatedOnSM { get; set; }
        public string UpdatedCodeSM { get; set; }
        public string UpdatedNameSM { get; set; }

        public DateTime? UpdatedOnHR { get; set; }
        public string UpdatedCodeHR { get; set; }
        public string UpdatedNameHR { get; set; }

    }

    public class EmployeeExitResponse : BaseResponse
    {
        public List<EmployeeExitDto> EmployeeExits { get; set; }
    }


    public class EmployeeExitFormReponseDto
    {
        public long Id { get; set; }
        public long ExitId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
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
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedCode { get; set; }
        public string UpdatedName { get; set; }
    }

    public class EmployeeExitFormResponse : BaseResponse
    {
        public EmployeeExitFormReponseDto EmployeeExitForm { get; set; }
    }


    public class HODFeedBackFormResponseDto
    {
        public long Id { get; set; }
        public long ExitId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public bool IsDesiredAttrition { get; set; }
        public string IntentionToLeaveKai { get; set; }
        public string AttemptsToRetainEmployee { get; set; }
        public string EligibleToRehire { get; set; }
        public string Comments { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedCode { get; set; }
        public string UpdatedName { get; set; }
    }

    public class HODFeedBackFormResponse : BaseResponse
    {
        public HODFeedBackFormResponseDto HODFeedBackForm { get; set; }
    }

    public class HRFeedBackFormResponseDto
    {
        public long Id { get; set; }
        public long ExitId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public DateTime DateOfResignation { get; set; }
        public DateTime? DateOfRelieving { get; set; }
        public string EmployeeThoughtOnKai { get; set; }
        public string EmployeeLikeToChange { get; set; }
        public string EmployeeRejoinLater { get; set; }
        public string SalaryAndDesignationOffered { get; set; }
        public string Comments { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedCode { get; set; }
        public string UpdatedName { get; set; }
    }

    public class HRFeedBackFormResponse : BaseResponse
    {
        public HRFeedBackFormResponseDto HRFeedBackForm { get; set; }
    }

    public class EmployeeExitAssetDto
    {
        public long EmpoyeeExitId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public DateTime RelievingDate { get; set; }
    }

    public class EmployeeExitAssetResponse : BaseResponse
    {
        public List<EmployeeExitAssetDto> EmployeeExitAssets { get; set; }
    }


    public class EmployeeExitAssetDetailsDto
    {
        public long EmployeeAssetId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string AssetType { get; set; }

        public string Manager { get; set; }

        public string SeniorManager { get; set; }

        public bool LoggedInUserAssetOwner { get; set; }

        public string AssetUniqueId { get; set; }
        public decimal? AssetBreakageFee { get; set; }
        public string Status { get; set; }
        public bool IsDefaultRMHODAssets { get; set; }
        public string Comments { get; set; }
        public string HodComments { get; set; }
        public long? AssetOwner { get; set; }
        public long AddedBy { get; set; }
        public DateTime AddedOn { get; set; }
        public long? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string UpdatedCode { get; set; }
        public string UpdatedName { get; set; }
    }

    public class EmployeeExitAssetDetailsResponse : BaseResponse
    {
        public List<EmployeeExitAssetDetailsDto> EmployeeExitAssetDetails { get; set; }
    }
}