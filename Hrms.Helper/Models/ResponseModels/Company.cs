using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class UpdateCompanySettingsResponse : BaseResponse
    {
        public List<HolidayDto> Holidays { get; set; }
        public List<LocationDto> Locations { get; set; }
        public List<GradeDto> Grades { get; set; }
        public List<DocumentTypeDto> DocumentTypes { get; set; }
        public List<TicketCategoryDto> TicketCategories { get; set; }
        public List<ProductLineDto> ProductLines { get; set; }
        public List<TicketFaqDto> TicketFaqs { get; set; }
        public List<AnnouncementTypeDto> AnnouncementTypes { get; set; }
        public List<DepartmentDto> Departments { get; set; }
        public List<DesignationDto> Designations { get; set; }
        public List<RegionDto> Regions { get; set; }
        public List<TeamDto> Teams { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public List<AppraisalQuestionDto> AppraisalQuestions { get; set; }
        public List<AssetTypeDto> AssetTypes { get; set; }
        public CompanyDto Company { get; set; }
    }

    public class TicketCategoriesListResponse : BaseResponse
    {
        public List<TicketCategoryDto> TicketCategories { get; set; }
    }

    public class GetAppraisalQuestionsResponse : BaseResponse
    {
        public List<AppraisalQuestionDto> AppraisalQuestions { get; set; }
    }

    public class GetAppraisalRatingsResponse : BaseResponse
    {
        public List<AppraisalRatingDto> AppraisalRatings { get; set; }
    }
    
    public class GetTicketFaqResponse : BaseResponse
    {
        public List<TicketFaqDto> TicketFaqs { get; set; }
    }
}