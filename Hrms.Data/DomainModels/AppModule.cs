using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hrms.Data.DomainModels
{
    public partial class AppModule
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("module")]
        [StringLength(100)]
        public string Module { get; set; }
        [Column("moduleSlug")]
        [StringLength(100)]
        public string ModuleSlug { get; set; }
    }
}
