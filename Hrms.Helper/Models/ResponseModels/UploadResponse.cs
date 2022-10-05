using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Models.Contracts;

namespace Hrms.Helper.Models.ResponseModels
{
    public class UploadResponse : BaseResponse
    {
        public string Message { get; set; }
        public int RowsInserted { get; set; }
    }
}
