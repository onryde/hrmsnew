using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface ICompanyRepo : IBaseRepository<AppCompany>
    {
        UpdateCompanySettingsResponse GetAllSettings(BaseRequest request);

        UpdateCompanySettingsResponse UpdateHolidays(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateDepartments(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateLocations(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateTicketCategory(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateGrades(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateDesignations(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateProductLines(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateTicketFaq(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateCategories(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateRegions(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateTeams(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateDocumentTypes(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateAnnouncementTypes(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateAssetTypes(UpdateCompanySettingsRequest request);
        UpdateCompanySettingsResponse UpdateCompany(UpdateCompanyDetailsRequest request);
        
        GetAppraisalQuestionsResponse UpdateAppraisalQuestions(UpdateAppraisalQuestionsRequest request);
        GetAppraisalQuestionsResponse GetAppraisalQuestions(BaseRequest request);
        GetAppraisalRatingsResponse GetAppraisalRatings(BaseRequest request);

        
        GetTicketFaqResponse GetTicketFaqs(BaseRequest request);
        SelectOptionResponse GetLocationsForDropdown(BaseRequest request);
        SelectOptionResponse GetAssetTypesForDropdown(BaseRequest request);
        TicketCategoriesListResponse GetTicketCategoriesForDropdown(BaseRequest request);
        SelectOptionResponse GetRolesForDropdown(BaseRequest request);
        SelectOptionResponse GetTrainingsForDropdown(GetTrainingsRequest request);
        SelectOptionResponse GetTrainingCodeForDropdown(GetTrainingsRequest request);
        SelectOptionResponse GetDepartmentsForDropdown(BaseRequest request);
        SelectOptionResponse GetDesignationsForDropdown(BaseRequest request);
        SelectOptionResponse GetDocumentTypesForDropdown(BaseRequest request);
        SelectOptionResponse GetTeamsForDropdown(BaseRequest request);
        SelectOptionResponse GetCategoriesForDropdown(BaseRequest request);
        SelectOptionResponse GetAnnouncementTypesForDropdown(BaseRequest request);
        SelectOptionResponse GetGradesForDropdown(BaseRequest request);
        SelectOptionResponse GetRegionsForDropdown(BaseRequest request);
        SelectOptionResponse GetReportingToForDropdown(ReportingToForDropdownRequest request);

        UpdateCompanySettingsResponse GetHolidays(GetHolidaysRequset request);


        UploadResponse UploadDataToSettings(UploadDataRequest request);
    }
}