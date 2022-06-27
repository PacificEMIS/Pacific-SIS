/***********************************************************************************
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

using opensis.data.Models;
using opensis.data.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace opensis.data.Helper
{
    
    public static class Utility
    {
        static string encryptionKey = "oPen$!$b14Ca5898a4e4133b!456k42g";
        /// <summary>
        /// This method returns a int primarykeyId  for an entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cRMContext"></param>
        /// <param name="columnSelector"></param>
        /// <returns></returns>
        public static int? GetMaxPK<TEntity>(CRMContext? cRMContext, Func<TEntity, int> columnSelector) where TEntity : class
        {
            int? GetMaxId = 0;
           
                
                var entityClass = cRMContext?.Set<TEntity>();
           
                if (entityClass?.Any() != true)
                {
                    GetMaxId = 1;
                }
                else
                {
                    GetMaxId = cRMContext?.Set<TEntity>().Max(columnSelector);
                    if (GetMaxId == null || GetMaxId <= 0)
                    {
                        GetMaxId = 1;
                    }
                    else
                    {
                        GetMaxId = GetMaxId + 1;
                    }
                }


            return GetMaxId;
        }


        
        /// <summary>
        /// This method returns a long primarykeyId  for an entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="cRMContext"></param>
        /// <param name="columnSelector"></param>
        /// <returns></returns>
        public static long? GetMaxLongPK<TEntity>(CRMContext? cRMContext, Func<TEntity, long> columnSelector) where TEntity : class
        {
            long? GetMaxId = 0;


            var entityClass = cRMContext?.Set<TEntity>();
            if (entityClass !=null && !entityClass.Any())
            {
                GetMaxId = 1;
            }
            else
            {
                GetMaxId = cRMContext?.Set<TEntity>().Max(columnSelector);
                if (GetMaxId == null || GetMaxId <= 0)
                {
                    GetMaxId = 1;
                }
                else
                {
                    GetMaxId = GetMaxId + 1;
                }
            }


            return GetMaxId;
        }

        /// <summary>
        /// This method returns a decrypt string for a password.
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string Decrypt(string cipherText)
        {
            string passwordKey = "oPen$!$.b14Ca5898a4e4133b!";
            byte[] cipherBytes = Convert.FromBase64String(cipherText); using (Aes encryptor = Aes.Create())
            {
                var salt = cipherBytes.Take(16).ToArray();
                var iv = cipherBytes.Skip(16).Take(16).ToArray();
                var encrypted = cipherBytes.Skip(32).ToArray();
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(passwordKey, salt, 100); encryptor.Key = pdb.GetBytes(32);
                encryptor.Padding = PaddingMode.PKCS7;
                encryptor.Mode = CipherMode.CBC;
                encryptor.IV = iv; using (MemoryStream ms = new MemoryStream(encrypted))
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (var reader = new StreamReader(cs, Encoding.UTF8))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method returns a hashed string for a password.
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string GetHashedPassword(string Input)
        {
            string passwordSecurityKey = "oPenSIsV2.0lOGinS1c0R3t8K61";
            var sha1 = System.Security.Cryptography.SHA256.Create();
            var inputBytes = Encoding.ASCII.GetBytes(Input + passwordSecurityKey);
            var hash = sha1.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// This method returns a soting a coloumn.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public static IQueryable<T> Sort<T>(this IQueryable<T> source, string sortBy, string sortDirection)
        {
            var param = Expression.Parameter(typeof(T), "item");

            var sortExpression = Expression.Lambda<Func<T, object>>
                (Expression.Convert(Expression.Property(param, sortBy), typeof(object)), param);

            switch (sortDirection.ToLower())
            {
                case "asc":
                    return source.AsQueryable<T>().OrderBy<T, object>(sortExpression);
                default:
                    return source.AsQueryable<T>().OrderByDescending<T, object>(sortExpression);
            }
        }

        /// <summary>
        /// This method returns filtered data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterParams"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IEnumerable<T> FilteredData<T>(List<FilterParams> filterParams, IEnumerable<T> data)
        {

            IEnumerable<string> distinctColumns = filterParams.Where(x => !String.IsNullOrEmpty(x.ColumnName)).Select(x => x.ColumnName).Distinct();

            foreach (string colName in distinctColumns)
            {
                var filterColumn = typeof(T).GetProperty(colName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                if (filterColumn != null)
                {
                    IEnumerable<FilterParams> filterValues = filterParams.Where(x => x.ColumnName.Equals(colName)).Distinct();

                    if (filterValues.Count() > 1)
                    {
                        IEnumerable<T> sameColData = Enumerable.Empty<T>();

                        foreach (var val in filterValues)
                        {
                            sameColData = sameColData.Concat(FilterData(val.FilterOption, data, filterColumn, val.FilterValue));
                        }

                        data = data.Intersect(sameColData);
                    }
                    else
                    {
                        data = FilterData(filterValues!.FirstOrDefault()!.FilterOption, data, filterColumn, filterValues!.FirstOrDefault()!.FilterValue);
                    }
                }
            }
            return data;
        }

        /// <summary>
        /// This method returns filtered data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filterOption"></param>
        /// <param name="data"></param>
        /// <param name="filterColumn"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        private static IEnumerable<T> FilterData<T>(FilterOptions filterOption, IEnumerable<T> data, PropertyInfo filterColumn, string filterValue)
        {
            int outValue;
            DateTime dateValue;
            switch (filterOption)
            {
                #region [StringDataType]  

                case FilterOptions.StartsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString()!.ToLower().StartsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.EndsWith:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString()!.ToLower().EndsWith(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.Contains:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString()!.ToLower().Contains(filterValue.ToString().ToLower())).ToList();
                    break;
                case FilterOptions.DoesNotContain:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                     (filterColumn.GetValue(x, null) != null && !filterColumn.GetValue(x, null)!.ToString()!.ToLower().Contains(filterValue.ToString().ToLower()))).ToList();
                    break;
                case FilterOptions.IsEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                     (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString() == string.Empty)).ToList();
                    break;
                case FilterOptions.IsNotEmpty:
                    data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString() != string.Empty).ToList();
                    break;
                #endregion

                #region [Custom]  

                case FilterOptions.IsGreaterThan:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) > outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) > dateValue).ToList();

                    }
                    break;

                case FilterOptions.IsGreaterThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) >= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) >= dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsLessThan:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) < outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) < dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsLessThanOrEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) <= outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) <= dateValue).ToList();
                        break;
                    }
                    break;

                case FilterOptions.IsEqualTo:
                    if (filterValue == string.Empty)
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null
                                        || (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString()!.ToLower() == string.Empty)).ToList();
                    }
                    else
                    {
                        if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                        {
                            data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) == outValue).ToList();
                        }
                        else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                        {
                            data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) == dateValue).ToList();
                            break;
                        }
                        else
                        {
                            data = data.Where(x => filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString()!.ToLower() == filterValue.ToLower()).ToList();
                        }
                    }
                    break;

                case FilterOptions.IsNotEqualTo:
                    if ((filterColumn.PropertyType == typeof(Int32) || filterColumn.PropertyType == typeof(Nullable<Int32>)) && Int32.TryParse(filterValue, out outValue))
                    {
                        data = data.Where(x => Convert.ToInt32(filterColumn.GetValue(x, null)) != outValue).ToList();
                    }
                    else if ((filterColumn.PropertyType == typeof(Nullable<DateTime>)) && DateTime.TryParse(filterValue, out dateValue))
                    {
                        data = data.Where(x => Convert.ToDateTime(filterColumn.GetValue(x, null)) != dateValue).ToList();
                        break;
                    }
                    else
                    {
                        data = data.Where(x => filterColumn.GetValue(x, null) == null ||
                                         (filterColumn.GetValue(x, null) != null && filterColumn.GetValue(x, null)!.ToString()!.ToLower() != filterValue.ToLower())).ToList();
                    }
                    break;
                    #endregion
            }
            return data;
        }

        /// <summary>
        /// This method returns convert data to pivot table.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TColumn"></typeparam>
        /// <typeparam name="TRow"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="source"></param>
        /// <param name="columnSelector"></param>
        /// <param name="rowSelector"></param>
        /// <param name="dataSelector"></param>
        /// <returns></returns>
        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(
         this IEnumerable<T> source,
         Func<T, TColumn> columnSelector,
         Expression<Func<T, TRow>> rowSelector,
         Func<IEnumerable<T>, TData> dataSelector)
        {
            DataTable table = new DataTable();
            var rowsName = ((NewExpression)rowSelector.Body).Members!.Select(s => s).ToList();
            foreach (var row in rowsName)
            {
                var name = row.Name;
                table.Columns.Add(new DataColumn(name));
            }
            var columns = source.Select(columnSelector).Distinct();
            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column!.ToString()));
            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             });

            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                var items = row.Values.Cast<object>().ToList();
                string[] keyRow = row.Key!.ToString()!.Split(',');
                int index = 0;
                foreach (var key in keyRow)
                {
                    string keyValue = key.Replace("}", "").Split('=')[1].Trim();
                    items.Insert(index, keyValue);
                    index++;
                }
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }
            return table;
        }

        public static string CreatedOrUpdatedByForAccessLog(CRMContext? cRMContext,Guid? tenantId, string? UserEmail)
        {
            string createdOrUpdatedByName = string.Empty;

            if (!string.IsNullOrEmpty(UserEmail) && cRMContext != null)
            {
                var StaffData = cRMContext.StaffMaster.FirstOrDefault(e => e.TenantId == tenantId && e.PortalAccess == true && e.LoginEmailAddress == UserEmail);

                if (StaffData != null)
                {
                    createdOrUpdatedByName = $"{StaffData.FirstGivenName} {(StaffData.MiddleName == null ? "" : $"{StaffData.MiddleName} ")}{StaffData.LastFamilyName}";
                }
                else
                {
                    var ParentData = cRMContext.ParentInfo.FirstOrDefault(e => e.TenantId == tenantId && e.IsPortalUser == true && e.LoginEmail == UserEmail);
                    if (ParentData != null)
                    {
                        createdOrUpdatedByName = $"{ParentData.Firstname} { (ParentData.Middlename == null ? "" : $"{ParentData.Middlename} ")}{ParentData.Lastname}";
                    }
                    else
                    {
                        var StudentData = cRMContext.StudentMaster.FirstOrDefault(e => e.TenantId == tenantId && e.StudentPortalId != null && e.StudentPortalId == UserEmail);
                        if (StudentData != null)
                        {
                            createdOrUpdatedByName = $"{StudentData.FirstGivenName} { (StudentData.MiddleName == null ? "" : $"{StudentData.MiddleName} ")}{StudentData.LastFamilyName}";
                        }
                    }
                }
            }
            return createdOrUpdatedByName;
        }

        public static int? checkDuplicate(CRMContext? cRMContext,Guid? tenantId,int? schoolId,string? salutation,string? firstGivenName,string? middleName, string? lastFamilyName,string? suffix, DateTime? dob, string? emailAddress,string? ssn,string module,Guid? guid)
        {
            int? check=null;
            if (module== "student")
            {
                if (string.IsNullOrEmpty(emailAddress) && string.IsNullOrEmpty(ssn))
                {
                    var StudentData = cRMContext?.StudentMaster.FirstOrDefault(c => c.TenantId == tenantId && c.SchoolId == schoolId && (string.IsNullOrEmpty(salutation) || c.Salutation == salutation) && (string.IsNullOrEmpty(firstGivenName) || (c.FirstGivenName??"").ToLower() == firstGivenName.ToLower()) && (string.IsNullOrEmpty(middleName) || (c.MiddleName??"").ToLower() == middleName.ToLower()) && (string.IsNullOrEmpty(lastFamilyName) || (c.LastFamilyName??"").ToLower() == lastFamilyName.ToLower()) && (string.IsNullOrEmpty(suffix) || c.Suffix == suffix) && (dob == null || c.Dob!.Value.Date == dob.Value.Date) && (guid == null || c.StudentGuid != guid));


                    if (StudentData != null)
                    {
                        check = 1;
                    }
                }
                else
                {
                    var studentData = cRMContext?.StudentMaster.FirstOrDefault(c => c.TenantId == tenantId && c.SchoolId == schoolId && (salutation == null || c.Salutation == salutation) && (firstGivenName == null || c.FirstGivenName!.ToLower() == firstGivenName.ToLower()) && (middleName == null || c.MiddleName!.ToLower() == middleName.ToLower()) && (lastFamilyName == null || c.LastFamilyName!.ToLower() == lastFamilyName.ToLower()) && (suffix == null || c.Suffix == suffix) && (dob == null || c.Dob!.Value.Date == dob.Value.Date) && (emailAddress == null || c.PersonalEmail == emailAddress) && (ssn == null || c.SocialSecurityNumber!.ToLower() == ssn.ToLower()) && (guid == null || c.StudentGuid != guid));

                    if (studentData != null)
                    {
                        check = 0;
                    }
                }
            }
            else if (module == "parent")
            {
                var Data = cRMContext?.ParentInfo.Where(c => c.TenantId == tenantId && (salutation == null || c.Salutation == salutation) && (firstGivenName == null || (c.Firstname ?? "").ToLower() == firstGivenName.ToLower()) && (middleName == null || (c.Middlename ?? "").ToLower() == middleName.ToLower()) && (lastFamilyName == null || (c.Lastname ?? "").ToLower() == lastFamilyName.ToLower()) && (suffix == null || c.Suffix == suffix) && (emailAddress == null || c.PersonalEmail == emailAddress) && (ssn == null || (c.Mobile ?? "").ToLower() == ssn.ToLower()) && (guid == null || c.ParentGuid != guid));

                if (Data != null)
                {
                    int parentInfoData = Data.Select(x => x.ParentId).Distinct().Count();
                    if (parentInfoData > 0)
                    {
                        check = 0;
                    }
                }
            }
            return check;
        }

        public static string CreatedOrUpdatedBy(CRMContext? cRMContext, Guid? tenantId, string? userGuid)
        {
            string createdOrUpdatedByName =string.Empty;

            if (!string.IsNullOrEmpty(userGuid) && cRMContext!=null)
            {
                var StaffData = cRMContext.StaffMaster.FirstOrDefault(e => e.TenantId == tenantId && e.PortalAccess == true && e.StaffGuid.ToString() == userGuid);

                if (StaffData != null)
                {
                    createdOrUpdatedByName = $"{StaffData.FirstGivenName} {(StaffData.MiddleName == null ? "" : $"{StaffData.MiddleName} ")}{StaffData.LastFamilyName}";
                }
                else
                {
                    var ParentData = cRMContext.ParentInfo.FirstOrDefault(e => e.TenantId == tenantId && e.IsPortalUser == true && e.ParentGuid.ToString() == userGuid);
                    if (ParentData != null)
                    {
                        createdOrUpdatedByName = $"{ParentData.Firstname} { (ParentData.Middlename == null ? "" : $"{ParentData.Middlename} ")}{ParentData.Lastname}";
                    }
                    else
                    {
                        var StudentData = cRMContext.StudentMaster.FirstOrDefault(e => e.TenantId == tenantId && e.StudentPortalId != null && e.StudentGuid.ToString() == userGuid);
                        if (StudentData != null)
                        {
                            createdOrUpdatedByName = $"{StudentData.FirstGivenName} { (StudentData.MiddleName == null ? "" : $"{StudentData.MiddleName} ")}{StudentData.LastFamilyName}";
                        }
                    }
                }
            }

            return createdOrUpdatedByName;
        }

        public static decimal? GetAcademicYear(CRMContext cRMContext, Guid? tenantId, int? schoolId)
        {
            decimal? AcademicYear = null;

            var schoolYearData = cRMContext.SchoolYears.Where(e => e.TenantId == tenantId && e.SchoolId == schoolId).ToList();
            if (schoolYearData.Count > 0)
            {
                AcademicYear = schoolYearData.Max(x => x.AcademicYear);
            }

            return AcademicYear;
        }

        public static string EncryptString(string plainText)
        {
            byte[] iv = new byte[16];
            byte[] array;
            string key = encryptionKey;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }



        public static string DecryptString(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            string key = encryptionKey;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static decimal? GetCurrentAcademicYear(CRMContext cRMContext, Guid? tenantId, int? schoolId)
        {
            decimal? AcademicYear = null;

            var calendarData = cRMContext.SchoolCalendars.FirstOrDefault(x => x.TenantId == tenantId && x.SchoolId == schoolId && x.StartDate!.Value.Date <= DateTime.UtcNow.Date && x.EndDate!.Value.Date >= DateTime.UtcNow.Date && x.SessionCalendar == true);

            if (calendarData != null)
            {
                AcademicYear = calendarData.AcademicYear;
            }
            else
            {
                var lastAcademicData = cRMContext.SchoolCalendars.Where(x => x.TenantId == tenantId && x.SchoolId == schoolId && x.SessionCalendar == true).OrderByDescending(x => x.StartDate).FirstOrDefault();
                AcademicYear = lastAcademicData != null ? lastAcademicData.AcademicYear : null;
            }

            return AcademicYear;
        }

        public static List<Guid>? MedicalAdvancedSearch(CRMContext cRMContext, List<FilterParams> filterParams, Guid? tenantId, int? schoolId, List<Guid>? studentGuids)
        {
            List<Guid>? filterStudentGuids = null;

            var medicalInfoList = cRMContext.StudentMedicalListViews.Where(x => x.TenantId == tenantId && (schoolId == null || x.SchoolId == schoolId) && studentGuids!.Contains(x.StudentGuid));

            var medicalFilterData = Utility.FilteredData(filterParams, medicalInfoList).AsQueryable();

            if (medicalFilterData?.Any() == true)
            {
                filterStudentGuids = medicalFilterData.Select(s => s.StudentGuid).Distinct().ToList();
            }
            return filterStudentGuids;
        }
    }
}

