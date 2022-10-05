using System.Collections.Generic;
using Hrms.Helper.Models.Contracts;
using Hrms.Helper.Models.Dto;

namespace Hrms.Helper.Models.ResponseModels
{
    public class SelectOptionResponse : BaseResponse
    {
        public List<SelectOptionDto> Options { get; set; }
    }
}