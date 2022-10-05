using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class EmployeeAddress
    {
        public EmployeeAddress()
        {
            EmployeeContactPermanentAddress = new HashSet<EmployeeContact>();
            EmployeeContactPresentAddress = new HashSet<EmployeeContact>();
        }

        [Column("id")]
        public long Id { get; set; }
        [Column("companyId")]
        public long CompanyId { get; set; }
        [Column("employeeId")]
        public long EmployeeId { get; set; }
        [Column("doorNo")]
        [StringLength(100)]
        public string DoorNo { get; set; }
        [Column("streetName")]
        [StringLength(500)]
        public string StreetName { get; set; }
        [Column("landmark")]
        [StringLength(100)]
        public string Landmark { get; set; }
        [Column("village")]
        [StringLength(100)]
        public string Village { get; set; }
        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }
        [Column("district")]
        [StringLength(100)]
        public string District { get; set; }
        [Required]
        [Column("state")]
        [StringLength(100)]
        public string State { get; set; }
        [Required]
        [Column("country")]
        [StringLength(100)]
        public string Country { get; set; }
        [Required]
        [Column("pincode")]
        [StringLength(100)]
        public string Pincode { get; set; }

        [InverseProperty("PermanentAddress")]
        public virtual ICollection<EmployeeContact> EmployeeContactPermanentAddress { get; set; }
        [InverseProperty("PresentAddress")]
        public virtual ICollection<EmployeeContact> EmployeeContactPresentAddress { get; set; }
    }
}
