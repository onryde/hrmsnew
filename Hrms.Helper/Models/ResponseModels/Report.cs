using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class ReportListResponse : BaseResponse
    {
        public List<ReportDto> Reports { get; set; }
    }
}
