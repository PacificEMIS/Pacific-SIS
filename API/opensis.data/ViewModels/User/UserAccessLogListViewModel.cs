using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.User
{
    public class UserAccessLogListViewModel : CommonFields
    {
        public UserAccessLogListViewModel()
        {
            userAccessLogList = new List<UserAccessLog>();
            UserAccessLogIds = new List<long>();
        }
        public List<UserAccessLog> userAccessLogList { get; set; }
        public List<long> UserAccessLogIds { get; set; }
        public Guid? TenantId { get; set; }
        public int? TotalCount { get; set; }
        public int? PageNumber { get; set; }
        public int? _pageSize { get; set; }
    }
}
