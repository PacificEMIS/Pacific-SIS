using opensis.core.helper;
using opensis.core.helper.Interfaces;
using opensis.core.User.Interfaces;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace opensis.core.helper.Services
{
    public class CheckLoginSession : ICheckLoginSession
    {
       // public IUserRepository userRepository;
        public CRMContext context;
        public CheckLoginSession( IDbContextFactory dbContextFactory)
        {
           // this.userRepository = userRepository;
            this.context = dbContextFactory.Create();
        }

        public bool CheckToken(string user_name, string token)
        {
            if (user_name != null && token != null)
            {
                string tokenUserName = TokenManager.ValidateToken(token);
                if (tokenUserName != null)
                {
                    var tokenDetails = tokenUserName.Split("|");
                    var userName = tokenDetails.First();
                    var email = tokenDetails.ElementAt(1);
                    var tenentId = tokenDetails.Last();

                    if (userName.Equals(user_name))
                    {
                        var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId.ToString() == tenentId && x.EmailAddress == email && x.Token == token && x.IsExpired != true);

                        if (loginSessionData != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string RefreshToken(string token, string user_name)
        {
            string newToken = "";
            try
            {
                newToken = RefreshTokenWithExpiry(token, user_name).Token;
            }
            catch (Exception)
            {
                throw;
            }
            return newToken;
        }

        public (string Token, DateTimeOffset Expiry) RefreshTokenWithExpiry(string token, string user_name)
        {
            try
            {
                if (user_name != null && token != null)
                {              
                    //var tokenDetails = user_name.Split("|");
                    //var userName = tokenDetails.First();
                    //var email = tokenDetails.ElementAt(1);
                    //var tenentId = tokenDetails.Last();

                    if (CheckToken(user_name, token))
                    {
                        var loginSessionData = this.context?.LoginSession.FirstOrDefault(x =>x.Token == token && x.IsExpired != true);

                        if (loginSessionData != null)
                        {
                           var userName = user_name + "|" + loginSessionData.EmailAddress + "|" + loginSessionData.TenantId;
                            var tokenInfo = TokenManager.GenerateTokenWithExpiry(userName);

                            if (UpdateLoginSessionForUser(loginSessionData.EmailAddress, loginSessionData.TenantId, tokenInfo.Token, token))
                            {
                                return tokenInfo;
                            }
                        }
                    
                  
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return default((string, DateTimeOffset));
        }

        public bool UpdateLoginSessionForUser(string email, Guid? tenentId, string newToken, string oldToken)
        {
            try
            {
                if (email != null && tenentId != null && newToken != null && oldToken != null)
                {
                    var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId == tenentId && x.EmailAddress == email && x.Token == oldToken && x.IsExpired != true);

                    if (loginSessionData != null)
                    {
                        loginSessionData.IsExpired = true;
                    }

                    var loginSession = new LoginSession();
                    var ide = this.context?.LoginSession.Max(x => x.Id);
                    loginSession.Id = (int)ide + 1;
                    loginSession.TenantId = (Guid)tenentId;
                    loginSession.SchoolId = loginSessionData.SchoolId;
                    loginSession.EmailAddress = email;
                    loginSession.Token = newToken;
                    loginSession.IsExpired = false;
                    loginSession.LoginTime = DateTime.UtcNow;
                    this.context?.LoginSession.Add(loginSession);
                    this.context?.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception es)
            {
                return false;
            }
        }

        public string CheckTokenInLogin(string email,Guid? tenentId, string userName)
        {
            string token = null;
            var loginSessionData = this.context?.LoginSession.FirstOrDefault(x => x.TenantId == tenentId && x.EmailAddress == email && x.IsExpired != true);

            if (loginSessionData != null)
            {
                if(CheckToken(userName, loginSessionData.Token))
                {
                    token= loginSessionData.Token;
                }
            }
            return token;
        }
    }
}
