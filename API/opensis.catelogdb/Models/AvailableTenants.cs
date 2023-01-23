using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace opensis.catelogdb.Models
{
    [Serializable, Table("available_tenants")]
    public class AvailableTenants
    {
        [Key]
        public long? Id { get; set; }
        [Column("tenant_id")]
        public Guid? TenantId { get; set; }
        [Column("tenant_name")]
        public string TenantName { get; set; }
        [Column("tenant_logo")]
        public byte[] TenantLogo { get; set; }
        [Column("tenant_logo_icon")]
        public byte[] TenantLogoIcon { get; set; }
        [Column("tenant_sidenav_logo")]
        public byte[] TenantSidenavLogo { get; set; }
        [Column("tenant_fav_icon")]
        public byte[] TenantFavIcon { get; set; }
        [Column("tenant_footer")]
        public string TenantFooter { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; }
    }
}
