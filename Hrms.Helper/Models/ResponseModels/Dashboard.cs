using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hrms.Helper.Models.ResponseModels
{
    public class ManagerDashboardStatResponse: BaseResponse
    {
        public int OnRollPercent { get; set; }
        public int OffRollPercent { get; set; }
        public List<LocationWiseHeadCountDto> LocationWiseHeadCount { get; set; }
    }
    public class HrDashboardStatResponse : BaseResponse
    {
        public int OnRollPercent { get; set; }
        public int OffRollPercent { get; set; }
        public int AverageAge { get; set; }
        public List<LocationWiseHeadCountDto> LocationWiseHeadCount { get; set; }
        public List<LocationWiseHeadCountDto> DepartmentWiseHeadCount { get; set; }
    }
}
