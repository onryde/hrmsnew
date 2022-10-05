using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsExitGrade
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("grade")]
        [StringLength(5)]
        public string Grade { get; set; }
        [Column("preConfirmDays")]
        public int? PreConfirmDays { get; set; }
        [Column("postConfirmDays")]
        public int? PostConfirmDays { get; set; }
        [Column("companyId")]
        public int CompanyId { get; set; }
    }
}
