using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.RequestModels
{
    public class DownloadReportRequest: BaseRequest
    {
        public string ReportId { get; set; }
        public List<ReportInputDto> ReportInputs { get; set; }
    }
}
