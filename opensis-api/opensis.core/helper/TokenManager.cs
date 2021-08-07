﻿/***********************************************************************************
openSIS is a free student information system for public and non-public
schools from Open Solutions for Education, Inc.Website: www.os4ed.com.

Visit the openSIS product website at https://opensis.com to learn more.
If you have question regarding this software or the license, please contact
via the website.

The software is released under the terms of the GNU Affero General Public License as
published by the Free Software Foundation, version 3 of the License.
See https://www.gnu.org/licenses/agpl-3.0.en.html.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Copyright (c) Open Solutions for Education, Inc.

All rights reserved.
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace opensis.core.helper
{
    public class TokenManager
    {
        private const string Secret = "ERMN05OPLoDvbTTa/QkqLNMI7cPLguaRyHzyg7n5qNBVjQmtBhz4SzYh4NBVCXi3KJHlSXKP+oi2+bXr6CUYTR==";

        /// <summary>
        /// Generate Token
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string GenerateToken(string username)
        {
            //byte[] key = Convert.FromBase64String(Secret);
            //SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            //SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new[] {
            //          new Claim(ClaimTypes.Name, username)}),
            //    Expires = DateTime.UtcNow.AddMinutes(30),
            //    SigningCredentials = new SigningCredentials(securityKey,
            //        SecurityAlgorithms.HmacSha256Signature)
            //};

            //JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            //JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            //return handler.WriteToken(token);

            return GenerateTokenWithExpiry(username).Token;
        }

        /// <summary>
        /// Generate Token With Expiry
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static (string Token, DateTimeOffset Expiry) GenerateTokenWithExpiry(string username)
        {
            DateTimeOffset expiry = DateTimeOffset.UtcNow.AddMinutes(30);
            byte[] key = Convert.FromBase64String(Secret);
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(key);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                      new Claim(ClaimTypes.Name, username)}),
                Expires = expiry.UtcDateTime,
                SigningCredentials = new SigningCredentials(securityKey,
                    SecurityAlgorithms.HmacSha256Signature)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = handler.CreateJwtSecurityToken(descriptor);
            var jwtToken = handler.WriteToken(token);
            return (jwtToken, expiry);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static string RefreshToken(string token, string user_name)
        {
            string newToken = "";
            try
            {
                newToken= RefreshTokenWithExpiry(token, user_name).Token;

                //if (user_name != null && token != null)
                //{
                //    string tokenusername = TokenManager.ValidateToken(token);
                //    if (tokenusername != null)
                //    {
                //        if (tokenusername.Equals(user_name))
                //        {
                //            newToken = GenerateToken(user_name);
                //        }
                //    }
                //}
            }
            catch (Exception)
            {
                throw;
            }
            return newToken;
        }

        /// <summary>
        /// Refresh Token With Expiry
        /// </summary>
        /// <param name="token"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static (string Token, DateTimeOffset Expiry) RefreshTokenWithExpiry(string token, string user_name)
        {
            try
            {
                if (CheckToken(user_name, token))
                    return GenerateTokenWithExpiry(user_name);
            }
            catch (Exception)
            {
                throw;
            }
            return default((string, DateTimeOffset));
        }

        /// <summary>
        /// Get Principal
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                    return null;
                byte[] key = Convert.FromBase64String(Secret);
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
                SecurityToken securityToken;
                if (jwtToken.ValidTo <= DateTime.UtcNow)
                {
                    return null;
                }
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token,
                      parameters, out securityToken);
                return principal;
            }
            catch (Exception)
            {
                throw;
                //return null;
            }
        }

        /// <summary>
        /// Validate Token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string ValidateToken(string token)
        {
            string username = null;
            ClaimsPrincipal principal = GetPrincipal(token);
            if (principal == null)
                return null;
            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            Claim usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim.Value;
            return username;
        }

        /// <summary>
        /// Check Token
        /// </summary>
        /// <param name="user_name"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CheckToken(string user_name, string token)
        {
            try
            {
                if (user_name != null && token != null)
                {
                    string tokenusername = TokenManager.ValidateToken(token);
                    if (tokenusername != null)
                    {
                        if (tokenusername.Equals(user_name))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}