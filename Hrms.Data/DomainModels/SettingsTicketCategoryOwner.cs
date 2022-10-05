using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class SettingsTicketCategoryOwner
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("categoryId")]
        public long CategoryId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("SettingsTicketCategoryOwner")]
        public virtual SettingsTicketCategory Category { get; set; }
        [ForeignKey("CompanyId")]
        [InverseProperty("SettingsTicketCategoryOwner")]
        public virtual AppCompany Company { get; set; }
        [ForeignKey("EmployeeId")]
        [InverseProperty("SettingsTicketCategoryOwner")]
        public virtual Employee Employee { get; set; }
    }
}
