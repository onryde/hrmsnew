using Hrms.Helper.Models.Contracts;

namespace Hrms.Helper.Models.ResponseModels
{
    public class LoginResponse: BaseResponse
    {
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }
        public string Grade { get; set; }
        public string Designation { get; set; }
        public string EmployeeId { get; set; }
        public bool IsLoginSuccess { get; set; }
        public string PhotoUrl { get; set; }
        public string Code { get; set; }
        public string LocationId { get; set; }
        public bool IsManager { get; set; }
        public bool IsAccountLocked { get; set; }
        public bool IsCaptchaSuccess { get; set; }
        public string NodeToken { get; set; }
    }
    
    
}