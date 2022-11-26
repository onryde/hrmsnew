using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;

namespace Hrms.Data.Interfaces
{
    public interface IEmployeeRepo : IBaseRepository<Employee>
    {
        // ACCOUNT
        ChangePasswordResponse ChangePassword(ChangePasswordRequest request);


        // EMPLOYEE ACTIONS
        CreateEmployeeResponse CreateEmployee(CreateEmployeeRequest request, ApplicationSettings appSettings, bool sendEmail = true);
        CreateEmployeeResponse CopyEmployee(CopyEmployeeRequest request, ApplicationSettings appSettings, bool sendEmail = true);
        GetEmployeeAccountResponse UpdateEmployeeAccount(UpdateEmployeeAccountRequest request, bool removeExisting = false);
        GetEmployeePersonalResponse UpdateEmployeePersonal(UpdateEmployeePersonalRequest request, bool removeExisting = false);
        GetEmployeeCompanyResponse UpdateEmployeeCompany(UpdateEmployeeCompanyRequest request, bool removeExisting = false);
        GetEmployeeStatutoryResponse UpdateEmployeeStatutory(UpdateEmployeeStatutoryRequest request, bool removeExisting = false);
        GetEmployeePreviousCompanyResponse UpdateEmployeePreviousCompany(UpdateEmployeePreviousCompanyRequest request, bool removeExisting = false);
        GetEmployeeDocumentsResponse UpdateEmployeeDocument(UpdateEmployeeDocumentRequest request);
        GetEmployeeDocumentsResponse DeleteEmployeeDocument(DeleteEmployeeDocumentRequest request);
        GetEmployeeBankResponse UpdateEmployeeBank(UpdateEmployeeBankRequest request, bool removeExisting = false);
        GetEmployeeCareerResponse UpdateEmployeeCareer(UpdateEmployeeCareerRequest request, bool removeExisting = false);
        GetEmployeeContactResponse UpdateEmployeeContact(UpdateEmployeeContactRequest request);
        GetEmployeeFamilyResponse UpdateEmployeeFamily(UpdateEmployeeFamilyRequest request, bool removeExisting = false);
        GetEmployeeEducationResponse UpdateEmployeeEducation(UpdateEmployeeEducationRequest request, bool removeExisting = false);
        GetEmployeeLanguageResponse UpdateEmployeeLanguage(UpdateEmployeeLanguageRequest request, bool removeExisting = false);
        GetEmployeeReferenceResponse UpdateEmployeeReference(UpdateEmployeeReferenceRequest request, bool removeExisting = false);
        GetEmployeeAssetResponse UpdateEmployeeAssets(UpdateEmployeeAssetRequest request, bool removeExisting = false);

        BaseResponse ConvertEmployeeToOnRoll(EmployeeActionRequest request);
        BaseResponse DeleteEmployee(EmployeeActionRequest request);
        BaseResponse ToggleEmployeeLogin(EmployeeActionRequest request);

        //Report
        EmployeeListResponse GetAllEmployees(EmployeeListFilterRequest request);
        EmployeeRptBdayResponse GetEmployeeRptBday(EmployeeReportFilterRequest request);
        EmployeeRptWdayResponse GetEmployeeRptWday(EmployeeReportFilterRequest request);
        EmployeeRptHeadCountResponse GetEmployeeRptHeadCount(EmployeeReportFilterRequest request);
        EmployeeRptAddandExitResponse GetEmployeeRptAddandExit(EmployeeReportFilterRequest request);
        EmployeeRptProbationResponse GetEmployeeRptProbation(EmployeeReportFilterRequest request);
        EmployeeRptBasicResponse GetEmployeeRptBasic(EmployeeReportFilterRequest request);
        EmployeeRptResignedResponse GetEmployeeRptResigned(EmployeeReportFilterRequest request);
        EmployeeRptObjectiveResponse GetEmployeeRptObjective(EmployeeReportFilterRequest request);
        EmployeeRptCTCResponse GetEmployeeRptCTC(EmployeeReportFilterRequest request);
        EmployeeRptCompensationResponse GetEmployeeRptCompensation(EmployeeReportFilterRequest request);
        EmployeeRptMasterResponse GetEmployeeRptMaster(EmployeeReportFilterRequest request);
        EmployeeSSOResponse ssoInsuranceLink(EmployeeListFilterRequest request);
        EmployeeRehireResponse GetEmployeeRehire(EmployeeListFilterRequest request);
        GetEmployeeAccountResponse GetEmployeeAccount(EmployeeActionRequest request);
        GetEmployeePersonalResponse GetEmployeePersonal(EmployeeActionRequest request);
        GetEmployeeCompanyResponse GetEmployeeCompany(EmployeeActionRequest request);
        GetEmployeeStatutoryResponse GetEmployeeStatutory(EmployeeActionRequest request);
        GetEmployeePreviousCompanyResponse GetPreviousCompany(EmployeeActionRequest request);
        GetEmployeeDocumentsResponse GetEmployeeDocuments(EmployeeActionRequest request);
        GetEmployeeBankResponse GetEmployeeBanks(EmployeeActionRequest request);
        GetEmployeeCareerResponse GetEmployeeCareers(EmployeeActionRequest request);
        GetEmployeeContactResponse GetEmployeeContacts(EmployeeActionRequest request);
        GetEmployeeFamilyResponse GetEmployeeFamily(EmployeeActionRequest request);
        GetEmployeeEducationResponse GetEmployeeEducation(EmployeeActionRequest request);
        GetEmployeeLanguageResponse GetEmployeeLanguage(EmployeeActionRequest request);
        GetEmployeeReferenceResponse GetEmployeeReference(EmployeeActionRequest request);
        EmployeeExistResponse CheckIfEmployeeExist(EmployeeActionRequest request);
        GetTasksResponse GetAllEmployeeTasks(EmployeeTaskFilterRequest request);
        EmployeeBaseInfoResponse GetAllEmployeesBaseInfo(EmployeeInfoRequest request);
        EmployeeBaseInfoResponse GetReportingEmployeesBaseInfo(EmployeeActionRequest request);
        GetTicketsResponse GetEmployeeTickets(EmployeeActionRequest request);
        EmployeeCardResponse GetEmployeeOrgChart(EmployeeActionRequest request);
        GetEmployeeAssetResponse GetEmployeeAssets(EmployeeActionRequest request);
        EmployeeListResponse GetEmployeeReportees(EmployeeActionRequest request);
        GetEmployeeSigningsResponse GetEmployeeAssetSignings(EmployeeActionRequest request);
        GetEmployeeSigningsResponse UpdateEmployeeAssetSignings(UpdateEmployeeAssetSigningRequest request);


        // EMPLOYEE APPRAISAL ACTIONS
        GetEmployeeAppraisalResponse GetEmployeeAppraisalDetails(EmployeeActionRequest request);
        GetEmployeeAppraisalResponse GetEmployeeAppraisalReport(EmployeeAppraisalActionRequest request);
        GetEmployeeAppraisalResponse GetEmployeeSingleAppraisalDetails(EmployeeAppraisalActionRequest request);
        GetEmployeeAppraisalQuestionsResponse SaveAppraisalAnswers(SaveEmployeeAnswersRequest request);
        GetEmployeeAppraisalQuestionsResponse SaveRecommendedFitmentOrPromotion(SaveEmployeeAnswersRequest request);
        BaseResponse SaveAppraisalTrainings(SaveAppraisalTrainingsRequest request);
        GetEmployeeAppraisalQuestionsResponse SubmitAppraisalAnswers(EmployeeAppraisalActionRequest request, ApplicationSettings settings);
        GetEmployeeAppraisalFeedbackResponse UpdateFeedback(EmployeeAppraisalFeedbackRequest request);
        GetEmployeeAppraisalDetailsResponse SaveAppraisalRating(SaveEmployeeAppraisalRatingRequest request, ApplicationSettings settings);
        EmployeeReportingToResponse GetEmployeeAppraisalsAsManager(EmployeeActionRequest request);
        BaseResponse SaveAppraisalInternalAnswers(SaveEmployeeInternalAnswersRequest request);
        EmployeeReportingToResponse GetAllAppraisalsPendingWithHr(EmployeeActionRequest request);

        //EMPLOYEE OBJECITVE ACTIONS
        GetEmployeeAppraisalQuestionsResponse SubmitObjective(EmployeeAppraisalActionRequest request, ApplicationSettings settings);


        // EMPLOYEE ANNOUNCEMENT
        GetAnnouncementsResponse GetEmployeeAnnouncements(EmployeeActionRequest request);

        // EMPLOYEE DATA VERIFICATION
        void UpdateEmployeeDataVerification(EmployeeDataVerificationRequest request);
        EmployeeDataVerificationResponse GetEmployeeDataVerification(EmployeeActionRequest request);

        // EMPLOYEE INFO
        EmployeeCardResponse GetEmployeesBirthday(BaseRequest request);

        // EXIT PROCESS
        BaseResponse HRRaisedResignation(HRResignationRequest request, ApplicationSettings appSettings);
        BaseResponse InitiateEmployeeResignation(EmployeeResignationRequest request, ApplicationSettings appSettings);
        BaseResponse UpdateEmployeeResignation(UpdateEmployeeExit request, ApplicationSettings appSettings);
        EmployeeExitResponse GetEmployeeExits(BaseRequest request);
        EmployeeExitResponse GetAllEmployeeExitRequest(BaseRequest request);
        BaseResponse CreateEmployeeExitForm(EmployeeExitFormRequest request);
        BaseResponse CreateHODFeedBackForm(HODFeedBackFormRequest request);
        BaseResponse CreateHRFeedBackForm(HRFeedBackFormRequest request);
        EmployeeExitFormResponse GetEmployeeExitForm(ExitFormRequest request);
        HODFeedBackFormResponse GetHODFeedBackForm(ExitFormRequest request);
        HRFeedBackFormResponse GetHRFeedBackForm(ExitFormRequest request);      
        EmployeeExitAssetResponse GetAllEmployeeExitWithAssets(BaseRequest request);   
        EmployeeExitAssetDetailsResponse GetEmployeeExitAssetDetails(ExitFormRequest request);
        BaseResponse UpdateEmployeeExitAssetDetails(UpdateEmployeeExitAsset request);
        
        // DASHBOARD
        ManagerDashboardStatResponse GetManagerStats(BaseRequest request);
        HrDashboardStatResponse GetHrStats(BaseRequest request);


        BaseResponse UploadEmployeeData(UploadDataRequest request, ApplicationSettings appSettings);


        // COMPENSATION
        EmployeeCompensationResponse UpdateEmployeeCompensation(EmployeeCompensationRequest request, bool removeExisting = false);
        EmployeeCompensationResponse GetEmployeeCompensation(EmployeeActionRequest request);

        EmployeeTrainingResponse GetEmployeeTrainings(EmployeeActionRequest request);

        //TASK

        GetTaskFilterResponse GetEmployeeTaskFilter(BaseRequest request);
    }
}