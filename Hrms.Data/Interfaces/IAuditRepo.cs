using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;

namespace Hrms.Data.Interfaces
{
    public interface IAuditRepo: IBaseRepository<EmployeeAudit>
    {
        SelectOptionResponse GetAllAuditModules(BaseRequest request);
        AuditListResponse GetAllAudit(AuditFilterRequest request);
    }
}
