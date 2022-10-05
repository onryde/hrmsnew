using System;
using System.Linq;
using System.Net.Http;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Data.Interfaces;
using Hrms.Helper.EmailHelper;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Hrms.Helper.Settings;
using Hrms.Helper.StaticClasses;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hrms.Data.Repositories
{
    public class AuthRepo : BaseRepository<Employee>, IAuthRepo
    {
        private IEventLogRepo _eventLogRepo;
        private readonly HttpClient client;
        public AuthRepo(HrmsContext context, IEventLogRepo eventLogRepo) : base(context)
        {
            _eventLogRepo = eventLogRepo;
        }

        public LoginResponse LoginEmployee(LoginRequest request, ApplicationSettings appSettings)
        {
            //var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
            //var secretKey = Cryptography.Decrypt(appSettings.CaptchaSecretKey, appSettings.Key);
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var response = client.PostAsync($"{googleVerificationUrl}?secret={secretKey}&response={request.CaptchaKey}", null).Result;
            //        String jsonResponse = response.Content.ReadAsStringAsync().Result;
            //        dynamic jsonData = JObject.Parse(jsonResponse);
            //        if(jsonData.success != true.ToString().ToLower())
            //        {
            //            return new LoginResponse
            //            {
            //                IsSuccess = true,
            //                IsLoginSuccess = false,
            //                IsCaptchaSuccess = false
            //            };
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    return new LoginResponse
            //    {
            //        IsSuccess = true,
            //        IsLoginSuccess = false,
            //        IsCaptchaSuccess = false
            //    };
            //}

            var employee =
                GetAll()
                    .Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyReportingTo)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                    .Include(var => var.EmployeePersonalEmployee)
                    .FirstOrDefault(var =>
                        var.IsActive && var.RoleId != 0 &&
                        (request.EmailId.Equals("admin")
                         || request.EmailId.Equals(var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode)));
            if (employee == null)
            {
                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false
                };
            }

            if (!(employee.CanLogin ?? false))
            {
                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false,
                    IsAccountLocked = true
                };
            }

            if(request.MaxInvalidAttempted.HasValue && request.MaxInvalidAttempted.Value)
            {
                employee.CanLogin = false;
                Save();

                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false,
                    IsAccountLocked = true
                };
            }

            var encryptedPassword = Cryptography.Encrypt(request.Password, employee.PasswordSalt);
            if (!encryptedPassword.Equals(employee.Password))
            {
                if (!((employee.IsTemporaryPasswordSet ?? false) &&
                    encryptedPassword.Equals(employee.TemporaryPassword)))
                {
                    return new LoginResponse
                    {
                        IsSuccess = true,
                        IsLoginSuccess = false
                    };
                }
            }

            var encryptedOTPPassword = Cryptography.Encrypt(request.OTPPassword, employee.PasswordSalt);
            if (!encryptedOTPPassword.Equals(employee.OTPPassword))
            {
                    return new LoginResponse
                    {
                        IsSuccess = true,
                        IsLoginSuccess = false
                    };
            }

            var jwtData = new EmployeeTokenDto
            {
                Guid = employee.Guid,
                Name = employee.Name,
                Role = employee.Role.RoleName,
                EmployeeId = employee.Id,
                CompanyId = 1
            };
            var token = JwtHelper.GetJwtToken(appSettings.JwtSettings, jwtData);

            var photoUrl = employee.EmployeePersonalEmployee.Any()
                ? employee.EmployeePersonalEmployee.FirstOrDefault().PhotoLinkUrl
                : string.Empty;

            var company = employee.EmployeeCompanyEmployee.Any()
                ? employee.EmployeeCompanyEmployee.FirstOrDefault()
                : null;

            var code = employee.Role.RoleName.Equals("Admin")
                ? string.Empty
                : company.Status.Equals("on-roll")
                    ? company.EmployeeCode
                    : company.OffRoleCode;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.Login.Template, employee.Name, employee.Guid),
                PerformedBy = employee.Id,
                ActionId = EventLogActions.Login.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = employee.Guid,
                    userName = employee.Name
                })
            });

            var isManager = employee.EmployeeCompanyReportingTo != null && employee.EmployeeCompanyReportingTo.Any();

            Save();

            return new LoginResponse
            {
                IsSuccess = true,
                Name = employee.Name,
                Role = employee.Role.RoleName,
                Token = token,
                IsLoginSuccess = true,
                EmployeeId = employee.Guid,
                Department = company != null && company.Department != null ? company.Department.Name : string.Empty,
                Location = company != null && company.Location != null ? company.Location.Name : string.Empty,
                Grade = company != null && company.Grade != null ? company.Grade.Grade : string.Empty,
                Designation = company != null && company.Designation != null ? company.Designation.Name : string.Empty,
                PhotoUrl = photoUrl,
                Code = code,
                EmailId = employee.EmailId,
                LocationId = company != null && company.Location != null ? company.Location.Guid : string.Empty,
                IsManager = isManager,
                IsAccountLocked = false,
                IsCaptchaSuccess = true
            };
        }
        
        public LoginResponse LoginEmployeeOTP(LoginRequest request, ApplicationSettings appSettings)
        {
            //var googleVerificationUrl = "https://www.google.com/recaptcha/api/siteverify";
            //var secretKey = Cryptography.Decrypt(appSettings.CaptchaSecretKey, appSettings.Key);
            //try
            //{
            //    using (var client = new HttpClient())
            //    {
            //        var response = client.PostAsync($"{googleVerificationUrl}?secret={secretKey}&response={request.CaptchaKey}", null).Result;
            //        String jsonResponse = response.Content.ReadAsStringAsync().Result;
            //        dynamic jsonData = JObject.Parse(jsonResponse);
            //        if(jsonData.success != true.ToString().ToLower())
            //        {
            //            return new LoginResponse
            //            {
            //                IsSuccess = true,
            //                IsLoginSuccess = false,
            //                IsCaptchaSuccess = false
            //            };
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    return new LoginResponse
            //    {
            //        IsSuccess = true,
            //        IsLoginSuccess = false,
            //        IsCaptchaSuccess = false
            //    };
            //}

            var employee =
                GetAll()
                    .Include(var => var.EmployeeCompanyEmployee)
                    .FirstOrDefault(var =>
                        var.IsActive && var.RoleId != 0 &&
                        (request.EmailId.Equals("admin")
                         || request.EmailId.Equals(var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode)));
            if (employee == null)
            {
                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false
                };
            }

            if (!(employee.CanLogin ?? false))
            {
                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false,
                    IsAccountLocked = true
                };
            }

            if (request.MaxInvalidAttempted.HasValue && request.MaxInvalidAttempted.Value)
            {
                employee.CanLogin = false;
                Save();

                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false,
                    IsAccountLocked = true
                };
            }

            var encryptedPassword = Cryptography.Encrypt(request.Password, employee.PasswordSalt);
            if (!encryptedPassword.Equals(employee.Password))
            {
                if (!((employee.IsTemporaryPasswordSet ?? false) &&
                    encryptedPassword.Equals(employee.TemporaryPassword)))
                {
                    return new LoginResponse
                    {
                        IsSuccess = true,
                        IsLoginSuccess = false
                    };
                }
            }

            var tempPassword = RandomString.GetRandomString(8);
            var password = Cryptography.Encrypt(tempPassword, employee.PasswordSalt);
            //employee.IsTemporaryPasswordSet = true;
            employee.OTPPassword = password;

            var empCompanyResigned = employee.EmployeeCompanyEmployee.Any() ? employee.EmployeeCompanyEmployee.FirstOrDefault().IsResigned : null;
            if (empCompanyResigned == null || !empCompanyResigned.Value)
            {
                employee.CanLogin = true;
            }

            Save();
            if (request.EmailId == "admin")
            {
                EmailSender.SendOTPPasswordEmail(employee.Name, "duraimurugan.n@kubota.com", tempPassword, appSettings);
            }
            else { 
            EmailSender.SendOTPPasswordEmail(employee.Name, employee.EmailId, tempPassword, appSettings);
            }

            return new LoginResponse
            {
                IsSuccess = true,
                IsLoginSuccess = true,
                IsAccountLocked = false,
                IsCaptchaSuccess = true
            };
        }

        public LoginResponse LoginOthers(LoginRequest request, ApplicationSettings appSettings)

        {

            var employee =GetAll()
                    .Include(var => var.Role)
                    .Include(var => var.EmployeeCompanyReportingTo)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Category)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Department)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Designation)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Location)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Region)
                    .Include(var => var.EmployeeCompanyEmployee).ThenInclude(var => var.Grade)
                    .Include(var => var.EmployeePersonalEmployee)
                    .FirstOrDefault(var =>
                        var.IsActive && var.RoleId != 0 &&
                        request.EmailId.Equals(var.EmployeeCompanyEmployee.FirstOrDefault().EmployeeCode));
            if (employee == null)
            {
                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false
                };
            }

            if (!(employee.CanLogin ?? false))
            {
                return new LoginResponse
                {
                    IsSuccess = true,
                    IsLoginSuccess = false
                };
            }
            var encryptedPassword = Cryptography.Encrypt(request.Password, employee.PasswordSalt);

            if (!encryptedPassword.Equals(employee.Password))
            {
                if (!((employee.IsTemporaryPasswordSet ?? false) &&
                    encryptedPassword.Equals(employee.TemporaryPassword)))
                {
                    return new LoginResponse
                    {
                        IsSuccess = true,
                        IsLoginSuccess = false
                    };
                }
            }
            var jwtData = new EmployeeTokenDto
            {
                Guid = employee.Guid,
                Name = employee.Name,
                Role = employee.Role.RoleName,
                EmployeeId = employee.Id,
                CompanyId = 1
            };

            var token = JwtHelper.GetJwtToken(appSettings.JwtSettings, jwtData);

            var company = employee.EmployeeCompanyEmployee.Any()
                ? employee.EmployeeCompanyEmployee.FirstOrDefault()
                : null;
            var code = employee.Role.RoleName.Equals("Admin")
                ? string.Empty
                : company.Status.Equals("on-roll")
                    ? company.EmployeeCode
                    : company.OffRoleCode;

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.Login.Template, employee.Name, employee.Guid),
                PerformedBy = employee.Id,
                ActionId = EventLogActions.Login.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = employee.Guid,
                    userName = employee.Name
                })

            });

            Save();
            return new LoginResponse
            {
                IsSuccess = true,
                Name = employee.Name,
                Role = employee.Role.RoleName,
                Token = token,
                IsLoginSuccess = true,
                Department = company != null && company.Department != null ? company.Department.Name : string.Empty,
                Location = company != null && company.Location != null ? company.Location.Name : string.Empty,
                Grade = company != null && company.Grade != null ? company.Grade.Grade : string.Empty,
                Designation = company != null && company.Designation != null ? company.Designation.Name : string.Empty,
                Code = code,
                EmailId = employee.EmailId
            };

        }
        public BaseResponse LogoutEmployee(BaseRequest request)
        {
            throw new System.NotImplementedException();
        }

        public BaseResponse ForgotPassword(ForgotPasswordRequest request, ApplicationSettings appSettings)
        {
            var employee =
                GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.EmailId.Equals(request.EmailId) && var.IsActive);
            if (employee == null)
            {
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            var tempPassword = RandomString.GetRandomString(8);
            var password = Cryptography.Encrypt(tempPassword, employee.PasswordSalt);
            employee.IsTemporaryPasswordSet = true;
            employee.TemporaryPassword = password;

            var empCompanyResigned = employee.EmployeeCompanyEmployee.Any() ? employee.EmployeeCompanyEmployee.FirstOrDefault().IsResigned : null;
            if(empCompanyResigned == null || !empCompanyResigned.Value)
            {
                employee.CanLogin = true;
            }
            
            

            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.ForgotPassword.Template, employee.Name, employee.Guid),
                PerformedBy = employee.Id,
                ActionId = EventLogActions.ForgotPassword.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = employee.Guid,
                    userName = employee.Name
                })
            });

            Save();

            EmailSender.SendForgotPasswordEmail(employee.Name, employee.EmailId, tempPassword, appSettings);

            return new BaseResponse
            {
                IsSuccess = true
            };
        }

        public BaseResponse OTPPassword(OTPPasswordRequest request, ApplicationSettings appSettings)
        {
            var employee =
                GetAll()
                .Include(var => var.EmployeeCompanyEmployee)
                .FirstOrDefault(var => var.EmailId.Equals(request.EmailId) && var.IsActive);
            if (employee == null)
            {
                return new BaseResponse
                {
                    IsSuccess = true
                };
            }

            var tempPassword = RandomString.GetRandomString(8);
            var password = Cryptography.Encrypt(tempPassword, employee.PasswordSalt);
            //employee.IsTemporaryPasswordSet = true;
            employee.OTPPassword = password;

            var empCompanyResigned = employee.EmployeeCompanyEmployee.Any() ? employee.EmployeeCompanyEmployee.FirstOrDefault().IsResigned : null;
            if (empCompanyResigned == null || !empCompanyResigned.Value)
            {
                employee.CanLogin = true;
            }



            _eventLogRepo.AddAuditLog(new AuditInfo
            {
                AuditText = string.Format(EventLogActions.ForgotPassword.Template, employee.Name, employee.Guid),
                PerformedBy = employee.Id,
                ActionId = EventLogActions.ForgotPassword.ActionId,
                Data = JsonConvert.SerializeObject(new
                {
                    userId = employee.Guid,
                    userName = employee.Name
                })
            });

            Save();

            EmailSender.SendForgotPasswordEmail(employee.Name, employee.EmailId, tempPassword, appSettings);

            return new BaseResponse
            {
                IsSuccess = true,
            };
        }
    }
}