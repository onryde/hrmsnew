using System;
using System.Collections.Generic;
using System.Text;

namespace Hrms.Helper.Models.Dto
{
    public class ReportInputDto
    {
        public string ReportInputId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ReportDto
    {
        public string ReportId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ReportInputDto> ReportInputs { get; set; }
    }
}
