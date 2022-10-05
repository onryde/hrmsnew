using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Microsoft.AspNetCore.Http;

namespace Hrms.Helper.Models.RequestModels
{
    public class UpdateCompanySettingsRequest : BaseRequest
    {
        public string UpdateEntityType { get; set; }
        public List<HolidayDto> Holidays { get; set; }
        public List<LocationDto> Locations { get; set; }
        public List<GradeDto> Grades { get; set; }
        public List<DesignationDto> Designations { get; set; }
        public List<DocumentTypeDto> DocumentTypes { get; set; }
        public List<TicketCategoryDto> TicketCategories { get; set; }
        public List<ProductLineDto> ProductLines { get; set; }
        public List<TicketFaqDto> TicketFaqs { get; set; }
        public List<AnnouncementTypeDto> AnnouncementTypes { get; set; }
        public List<DepartmentDto> Departments { get; set; }
        public List<RegionDto> Regions { get; set; }
        public List<TeamDto> Teams { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<AppraisalQuestionDto> AppraisalQuestions { get; set; }
        public List<AssetTypeDto> AssetTypes { get; set; }
        public CompanyDto Company { get; set; }
    }

    public class UpdateCompanyDetailsRequest : BaseRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string GstNumber { get; set; }
        public string PanNumber { get; set; }
        public string TanNumber { get; set; }
        public string FullLogo { get; set; }
        public string SmallLogo { get; set; }
        public IFormFile FullLogoFile { get; set; }
        public IFormFile SmallLogoFile { get; set; }
        public IFormFile AlternateLogoFile { get; set; }
        public string FileBasePath { get; set; }
    }

    public class DesignationForDropdownRequest : BaseRequest
    {
        public string DepartmentId { get; set; }
    }

    public class ReportingToForDropdownRequest : BaseRequest
    {
        public string DepartmentId { get; set; }
        public string DesignationId { get; set; }
    }

    public class UpdateAppraisalQuestionsRequest : BaseRequest
    {
        public List<AppraisalQuestionDto> AppraisalQuestions { get; set; }
    }
    public class GetTrainingsRequest : BaseRequest
    {
        public List<string> GradeIds { get; set; }
    }

    public class GetHolidaysRequset: BaseRequest
    {
        public string Location { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
    }
}