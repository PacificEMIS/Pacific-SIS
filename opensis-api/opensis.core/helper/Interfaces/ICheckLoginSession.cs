using opensis.data.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.helper.Interfaces
{
    public interface ICheckLoginSession
    {
        public bool CheckToken(string user_name, string token);
        public string RefreshToken(string token, string user_name);
        public string CheckTokenInLogin(string email, Guid? tenentId,string userName);
    }
}
