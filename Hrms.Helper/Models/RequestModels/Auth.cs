using Hrms.Helper.Models.Contracts;

namespace Hrms.Helper.Models.RequestModels
{
    public class LoginRequest : BaseRequest
    {
        public string EmailId { get; set; }
        public string Password { get; set; }
        public string OTPPassword { get; set; }
        public string CaptchaKey { get; set; }
        public bool? MaxInvalidAttempted { get; set; }
    }

    public class ForgotPasswordRequest : BaseRequest
    {
        public string EmailId { get; set; }
        
        public string EmployeeCode { get; set; }
    }

    public class OTPPasswordRequest : BaseRequest
    {
        public string EmailId { get; set; }
    }
}