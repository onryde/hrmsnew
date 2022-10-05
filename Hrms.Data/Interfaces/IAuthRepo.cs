using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;

namespace Hrms.Data.Interfaces
{
    public interface IAuthRepo : IBaseRepository<Employee>
    {
        LoginResponse LoginEmployee(LoginRequest request, ApplicationSettings appsettings);
        LoginResponse LoginEmployeeOTP(LoginRequest request, ApplicationSettings appsettings);
        LoginResponse LoginOthers(LoginRequest request, ApplicationSettings appsettings);
        BaseResponse LogoutEmployee(BaseRequest request);
        BaseResponse ForgotPassword(ForgotPasswordRequest request, ApplicationSettings appsettings);
        BaseResponse OTPPassword(OTPPasswordRequest request, ApplicationSettings appsettings);
        //LoginResponse GetTasks(string employeeid, int year);
    }
}