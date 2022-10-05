using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Data.Core;
using Hrms.Data.DomainModels;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.RequestModels;
using Hrms.Helper.Models.ResponseModels;

namespace Hrms.Data.Interfaces
{
    public interface IReportRepo: IBaseRepository<SettingsReport>
    {
        ReportListResponse GetAllAvailableReports(BaseRequest request);
        void DownloadReport(DownloadReportRequest request);
    }
}
