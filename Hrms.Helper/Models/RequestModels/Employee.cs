using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace Hrms.Helper.Models.RequestModels
{
    public class ChangePasswordRequest : BaseRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class GetNotificationRequest : BaseRequest
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class EmployeeListFilterRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Grades { get; set; }
        public List<string> Departments { get; set; }
        public List<string> Designations { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Status { get; set; }
    }

    public class EmployeeReportFilterRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Code { get; set; }
        public List<string> Locations { get; set; }
        public List<string> Grades { get; set; }
        public List<string> Departments { get; set; }
        public List<string> Designations { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<string> EmployeeStatus { get; set; }
        public List<string> ProbationStatus { get; set; }
    }

    public class CreateEmployeeRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string OfficialEmail { get; set; }
        public string RoleId { get; set; }
        public bool CanLogin { get; set; }
        public string EmployeCode { get; set; }
    }

    public class CopyEmployeeRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string OfficialEmail { get; set; }
        public string OldEmail { get; set; }
        public string EmployeeCode { get; set; }
        public string RoleId { get; set; }
        public bool CanLogin { get; set; }
    }

    public class EmployeeActionRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
    }

    public class UpdateEmployeeAccountRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string RoleId { get; set; }
        public bool CanLogin { get; set; }
        public string EmployeeCode { get; set; }
        public string Status { get; set; }
        public string AddressingName { get; set; }
        public string OffRoleCode { get; set; }
    }

    public class UpdateEmployeeBankRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeBankDto> EmployeeBanks { get; set; }
    }

    public class UpdateEmployeeCareerRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeCareerDto> EmployeeCareers { get; set; }
    }

    public class UpdateEmployeeCompanyRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string StatusCategory { get; set; }
        public string UniqueCode { get; set; }
        public string Status { get; set; }
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
        public DateTime Doj { get; set; }
        public DateTime ProbationStartDate { get; set; }
        public DateTime ProbationEndDate { get; set; }
        public int ProbationExtraDays { get; set; }
        public string ConfirmationRemarks { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime OnRollDate { get; set; }
        public string ReportingToId { get; set; }
        public string LocationBifurcation { get; set; }
        public string Vendor { get; set; }
        public string LocationForField { get; set; }
        public string Division { get; set; }
    }

    public class UpdateEmployeeContactRequest : BaseRequest
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

    public class UpdateEmployeeFamilyRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeFamilyDto> EmployeeFamily { get; set; }
    }

    public class UpdateEmployeeEducationRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeEducationDto> EmployeeEducation { get; set; }
    }

    public class UpdateEmployeeLanguageRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeLanguageDto> EmployeeLanguage { get; set; }
    }

    public class UpdateEmployeePersonalRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string Nationality { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public DateTime RecordDob { get; set; }
        public DateTime ActualDob { get; set; }
        public string MaritalStatus { get; set; }
        public DateTime? MarriageDate { get; set; }
        public string Sports { get; set; }
        public string SpecializedTraining { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public int Age { get; set; }
        public IFormFile Photo { get; set; }
        public string FileBasePath { get; set; }
        public bool HideBirthday { get; set; }
    }

    public class UpdateEmployeePreviousCompanyRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeePreviousCompanyDto> PreviousCompanies { get; set; }
    }

    public class UpdateEmployeeReferenceRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeReferenceDto> EmployeeReference { get; set; }
    }

    public class UpdateEmployeeStatutoryRequest : BaseRequest
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

    public class UpdateEmployeeDocumentRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentTypeId { get; set; }
        public IFormFile File { get; set; }
        public string FileBasePath { get; set; }
    }

    public class DeleteEmployeeDocumentRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string DocumentId { get; set; }
        public string FileBasePath { get; set; }
    }

    public class EmployeeAppraisalActionRequest : BaseRequest
    {
        public string EmployeeAppraisalId { get; set; }
        public string EmployeeId { get; set; }
        public bool IsSelf { get; set; }
        public string RatingId { get; set; }
        public long? AppraisalMode { get; set; }
    }

    public class EmployeeAppraisalFeedbackRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string AppraiseeType { get; set; }
        public string EmployeeAppraisalId { get; set; }
        public string EmployeeFeedbackId { get; set; }
        public string Feedback { get; set; }
        public long AppraisalMode { get; set; }
    }

    public class SaveEmployeeAnswersRequest : BaseRequest
    {
        public bool IsSelf { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeAppraisalId { get; set; }
        public List<EmployeeAppraisalSelfAnswerDto> SelfAnswers { get; set; }
        public List<EmployeeAppraisalBusinessNeedAnswerDto> BusinessNeeds { get; set; }
        public List<EmployeeAppraisalQuestionAnswerDto> Questions { get; set; }
        public bool? IsFitmentRecommended { get; set; }
        public bool? IsPromotionRecommended { get; set; }

    }
    public class SaveAppraisalTrainingsRequest : BaseRequest
    {
        public bool IsSelf { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeAppraisalId { get; set; }
        public List<string> Trainings { get; set; }
        public string Comments { get; set; }
    }

    public class SaveEmployeeInternalAnswersRequest : BaseRequest
    {
        public bool IsSelf { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeAppraisalId { get; set; }
        public double Weightage { get; set; }
    }

    public class SaveEmployeeAppraisalRatingRequest : BaseRequest
    {
        public string EmployeeAppraisalId { get; set; }
        public string EmployeeId { get; set; }
        public string RatingId { get; set; }
    }

    public class EmployeeDataVerificationRequest : BaseRequest
    {
        public string Section { get; set; }
        public string EmployeeId { get; set; }
    }

    public class EmployeeTaskFilterRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<string> AssignedTo { get; set; }
        public List<string> AssignedBy { get; set; }
        public List<bool> Started { get; set; }
        public List<bool> Completed { get; set; }
        public List<bool> Verified { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Title { get; set; }
    }

    public class EmployeeInfoRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string Role { get; set; }
        public bool IncludeSelf { get; set; }
    }

    public class UpdateEmployeeAssetRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeAssetDto> Assets { get; set; }
    }
    public class UpdateEmployeeAssetSigningRequest : BaseRequest
    {
        public List<EmployeeSigningAssetDto> Assets { get; set; }
    }

    public class EmployeeAssetSignoffRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public string EmployeeAssetId { get; set; }
        public string Comment { get; set; }
    }

    public class EmployeeCompensationRequest : BaseRequest
    {
        public string EmployeeId { get; set; }
        public List<EmployeeCompensationDto> EmployeeCompensation { get; set; }
    }

    public class EmployeeExitInterviewRequest : BaseResponse
    {
        public string EmployeeId { get; set; }
        public List<EmployeeExitQuestionDto> Questions { get; set; }
    }

    public class EmployeeResignationRequest : BaseRequest
    {
        public string ResignationReason { get; set; }
        public DateTime PreferredRelievingDate { get; set; }
        public DateTime RelievingDateAsPerPolicy { get; set; }
    }

    public class HRResignationRequest : BaseRequest
    {
        public string ResignationReason { get; set; }
        public string ResignationType { get; set; }
        public string EmployeeId { get; set; }
        public DateTime PreferredRelievingDate { get; set; }
        public DateTime RelievingDateAsPerPolicy { get; set; }
        public DateTime ResignedOn { get; set; }
        public string Status { get; set; }
        
    }

    public class UpdateEmployeeExit : BaseRequest
    {
        public long EmployeeExitId { get; set; }
        public string EmployeeId { get; set; }
        public string Feedback { get; set; }
        public string Status { get; set; }
        public DateTime? RelievingDate { get; set; }
        public bool? IsRevoked { get; set; }
        public string ClearanceComments { get; set; }
        public bool EligibleForRehire { get; set; }
        public string FeedbackForOthers { get; set; }
    }

    public class EmployeeExitFormRequest : BaseRequest
    {
        public long EmployeeExitId { get; set; }
        public EmployeeExitFormDto EmployeeExitForm { get; set; }
    }

    public class HODFeedBackFormRequest : BaseRequest
    {
        public long EmployeeExitId { get; set; }
        public HODFeedBackFormDto HodFeedBackForm { get; set; }
    }

    public class HRFeedBackFormRequest : BaseRequest
    {
        public long EmployeeExitId { get; set; }
        public HRFeedBackFormDto HrFeedBackForm { get; set; }
    }

    public class ExitFormRequest : BaseRequest
    {
        public long EmployeeExitId { get; set; }
      
    }   

    public class UpdateEmployeeExitAsset : BaseRequest
    {
        public long EmployeeExitAssetId { get; set; }
        public string Status { get; set; }
        public decimal? BreakageFee { get; set; }
        public string comments { get; set; }
        public string hodComments { get; set; }
    }
}