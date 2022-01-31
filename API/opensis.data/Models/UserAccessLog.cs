using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace opensis.data.Models
{
    public partial class UserAccessLog
    {
        
        public long Id { get; set; }
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }
        [ValidateNever]
        public string Emailaddress { get; set; } = null!;
        public string? UserName { get; set; }
        public int? MembershipId { get; set; }
        public string? Profile { get; set; }
        public DateTime LoginAttemptDate { get; set; }
        public int? LoginFailureCount { get; set; }
        public bool? LoginStatus { get; set; }
        public string? Ipaddress { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
