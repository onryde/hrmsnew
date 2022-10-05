using System;
using System.Collections.Generic;
using System.Text;

namespace Hrms.Helper.Models.Dto
{
    public class LocationWiseHeadCountDto
    {
        public string Location { get; set; }
        public int OnRollCount { get; set; }
        public int OffRollCount { get; set; }
    }
}
