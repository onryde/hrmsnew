using System;
using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class EventLogResponse : BaseResponse
    {
        public List<EventLogDto> EventLogs { get; set; }
    }
}
