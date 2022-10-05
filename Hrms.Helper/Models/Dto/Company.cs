using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Hrms.Helper.Models.Dto
{
    public class HolidayDto
    {
        public DateTime Date { get; set; }
        public List<string> LocationIds { get; set; }
        public string Reason { get; set; }
        public string HolidayId { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
    }

    public class LocationDto
    {
        public string LocationId { get; set; }
        public string Location { get; set; }
        public string GstNumber { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public bool IsActive { get; set; }

        public int EmployeesCount { get; set; }
    }

    public class GradeDto
    {
        public string GradeId { get; set; }
        public string Grade { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int EmployeesCount { get; set; }
    }

    public class DocumentTypeDto
    {
        public string DocumentTypeId { get; set; }
        public string DocumentType { get; set; }
        public string Description { get; set; }
        public bool IsRestricted { get; set; }
        public bool IsActive { get; set; }

        public int DocumentsCount { get; set; }
    }

    public class TicketCategoryDto
    {
        public string TicketCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<TicketSubCategoryDto> TicketSubCategories { get; set; }
        public List<string> Owners { get; set; }

        public int TicketsCount { get; set; }
    }

    public class TicketSubCategoryDto
    {
        public string TicketSubCategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int TicketsCount { get; set; }
    }

    public class ProductLineDto
    {
        public string ProductLineId { get; set; }
        public string ProductLine { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public int EmployeesCount { get; set; }
    }

    public class TicketFaqDto
    {
        public string TicketFaqId { get; set; }
        public string FaqTitle { get; set; }
        public string Description { get; set; }
        public string TicketSubCategoryId { get; set; }
        public string TicketCategoryId { get; set; }
        public bool IsActive { get; set; }
    }

    public class AnnouncementTypeDto
    {
        public string AnnouncementTypeId { get; set; }
        public string AnnouncementType { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int AnnouncementCount { get; set; }
    }

    public class DepartmentDto
    {
        public string DepartmentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FrontColor { get; set; }
        public string BackColor { get; set; }
        public List<DesignationDto> Designations { get; set; }
        public bool IsActive { get; set; }
        public int EmployeesCount { get; set; }
    }

    public class DesignationDto
    {
        public string DesignationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public string ReportingTo { get; set; }
        public int RefId { get; set; }
        public bool IsActive { get; set; }
        public int EmployeesCount { get; set; }
    }

    public class RegionDto
    {
        public string RegionId { get; set; }
        public string Region { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int EmployeesCount { get; set; }
    }

    public class TeamDto
    {
        public string TeamId { get; set; }
        public string Team { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int EmployeesCount { get; set; }
    }

    public class CategoryDto
    {
        public string CategoryId { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int EmployeesCount { get; set; }
    }

    public class AppraisalQuestionDto
    {
        public string QuestionId { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int AppraisalsCount { get; set; }
    }
    
    public class AppraisalRatingDto
    {
        public string RatingId { get; set; }
        public string Rating { get; set; }
        public string Description { get; set; }
        public string Tag { get; set; }
        public double Score { get; set; }
    }

    public class CompanyDto
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
    }

    public class AssetTypeDto
    {
        public string AssetType { get; set; }
        public string AssetTypeId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int EmployeesActive { get; set; }
        public List<string> SigningAuthorities { get; set; }
    }
}