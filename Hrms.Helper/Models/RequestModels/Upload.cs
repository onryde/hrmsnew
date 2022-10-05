using System;
using System.Collections.Generic;
using System.Text;
using Hrms.Helper.Models.Contracts;

namespace Hrms.Helper.Models.RequestModels
{
    public class UploadDataRequest : BaseRequest
    {
        public bool IsCompany { get; set; }
        public string Type { get; set; }
        public string FileContent { get; set; }
        public int SkipLines { get; set; }
    }
}
