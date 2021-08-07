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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.data.Helper;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.ViewModels.CommonModel;
using opensis.data.ViewModels.Membership;
using opensis.data.ViewModels.StaffSchedule;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace opensis.data.Repository
{
    public class CommonRepository : ICommonRepository
    {
        private CRMContext context;
        private static readonly string NORECORDFOUND = "No Record Found";
        public CommonRepository(IDbContextFactory dbContextFactory)
        {
            this.context = dbContextFactory.Create();
        }

        /// <summary>
        /// Get All Cities By State
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        public CityListModel GetAllCitiesByState(CityListModel city)
        {
            CityListModel cityListModel = new CityListModel();
            try
            {
                var CityList = this.context?.City.Where(x => x.StateId == city.StateId).Select(e=> new City()
                { 
                    Id=e.Id,
                    Name=e.Name,
                    StateId=e.StateId,
                    CreatedBy= (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == city.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    CreatedOn=e.CreatedOn,
                    UpdatedBy= (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == city.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    UpdatedOn=e.UpdatedOn
                }).ToList();
                
                if (CityList.Count > 0)
                {
                    cityListModel.TableCity = CityList;
                }
                cityListModel._tenantName = city._tenantName;
                cityListModel._token = city._token;
                cityListModel._failure = false;
            }
            catch (Exception es)
            {
                cityListModel._message = es.Message;
                cityListModel._failure = true;
                cityListModel._tenantName = city._tenantName;
                cityListModel._token = city._token;
            }
            return cityListModel;

        }

        /// <summary>
        /// Get All States By Country
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public StateListModel GetAllStatesByCountry(StateListModel state)
        {
            StateListModel stateListModel = new StateListModel();
            try
            {
                stateListModel.TableState = null;
                var StateList = this.context?.State.Where(x => x.CountryId == state.CountryId).Select(e=> new State()
                { 
                   Id=e.Id,
                   Name=e.Name,
                   CountryId=e.CountryId,
                   CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == state.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                   CreatedOn = e.CreatedOn,
                   UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == state.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                   UpdatedOn = e.UpdatedOn
                }).ToList();
                
                if (StateList.Count > 0)
                {
                    stateListModel.TableState = StateList;
                }
                stateListModel._tenantName = state._tenantName;
                stateListModel._token = state._token;
                stateListModel._failure = false;
            }
            catch (Exception es)
            {
                stateListModel._message = es.Message;
                stateListModel._failure = true;
                stateListModel._tenantName = state._tenantName;
                stateListModel._token = state._token;
            }
            return stateListModel;

        }
        
        /// <summary>
        /// Get All Countries
        /// </summary>
        /// <param name="country"></param>
        /// <returns></returns>
        public CountryListModel GetAllCountries(CountryListModel country)
        {
            CountryListModel countryListModel = new CountryListModel();
            try
            {
                var CountryList = this.context?.Country.Select(e=> new Country()
                {
                    Id = e.Id,
                    Name = e.Name,
                    CountryCode = e.CountryCode,
                    CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == country.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    CreatedOn = e.CreatedOn,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == country.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    UpdatedOn = e.UpdatedOn
                }).ToList();

                countryListModel.TableCountry = CountryList;

                if (CountryList.Count > 0)
                {
                    int? StateCount = this.context?.State.Count();
                    countryListModel.StateCount = StateCount;
                    countryListModel._failure = false;
                }
                else
                {
                    countryListModel._failure = true;
                    countryListModel._message = NORECORDFOUND;
                }
                countryListModel._tenantName = country._tenantName;
                countryListModel._token = country._token;

            }
            catch (Exception es)
            {
                countryListModel._message = es.Message;
                countryListModel._failure = true;
                countryListModel._tenantName = country._tenantName;
                countryListModel._token = country._token;
            }
            return countryListModel;

        }

        /// <summary>
        /// Add Language 
        /// </summary>
        /// <param name="languageAdd"></param>
        /// <returns></returns>
        public LanguageAddModel AddLanguage(LanguageAddModel languageAdd)
        {
            try
            {
                var getLanguage = this.context?.Language.FirstOrDefault(x => x.Locale.ToLower() == languageAdd.Language.Locale.ToLower());

                if (getLanguage != null)
                {
                    languageAdd._message = "This Language Name Already Exist";
                    languageAdd._failure = true;
                }
                else
                {
                    int? languageId = Utility.GetMaxPK(this.context, new Func<Language, int>(x => x.LangId));
                    languageAdd.Language.LangId = (int)languageId;
                    languageAdd.Language.CreatedOn = DateTime.UtcNow;
                    this.context?.Language.Add(languageAdd.Language);
                    this.context?.SaveChanges();
                    languageAdd._failure = false;
                    languageAdd._message = "Language Added Successfully";
                }
            }
            catch (Exception es)
            {
                languageAdd._message = es.Message;
                languageAdd._failure = true;
                languageAdd._tenantName = languageAdd._tenantName;
                languageAdd._token = languageAdd._token;
            }
            return languageAdd;
        }

        /// <summary>
        /// Get Language By Id 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LanguageAddModel ViewLanguage(LanguageAddModel language)
        {
            try
            {
                LanguageAddModel languageAddModel = new LanguageAddModel();
                var getLanguageValue = this.context?.Language.FirstOrDefault(x => x.LangId == language.Language.LangId);
                if (getLanguageValue != null)
                {
                    languageAddModel.Language = getLanguageValue;
                    languageAddModel._failure = false;
                    return languageAddModel;
                }
                else
                {
                    languageAddModel._failure = true;
                    languageAddModel._message = NORECORDFOUND;
                    return languageAddModel;
                }
            }
            catch (Exception es)
            {

                throw;
            }
        }

        /// <summary>
        /// Update Language
        /// </summary>
        /// <param name="languageUpdate"></param>
        /// <returns></returns>
        public LanguageAddModel UpdateLanguage(LanguageAddModel languageUpdate)
        {
            var getLanguage = this.context?.Language.Where(x => x.Locale.ToLower() == languageUpdate.Language.Locale.ToLower() && x.LangId != languageUpdate.Language.LangId).ToList();

            if (getLanguage.Count > 0)
            {
                languageUpdate._message = "This Language Name Already Exist";
                languageUpdate._failure = true;
            }
            else
            {
                var getLanguageData = this.context?.Language.FirstOrDefault(x => x.LangId == languageUpdate.Language.LangId);
                if (getLanguageData != null)
                {
                    if (getLanguageData.Lcid != null)
                    {
                        if (getLanguageData.Lcid.ToLower() == "en-us".ToLower() || getLanguageData.Lcid.ToLower() == "fr-fr".ToLower() || getLanguageData.Lcid.ToLower() == "es-es".ToLower())
                        {
                            languageUpdate._message = "This Language is not editable";
                            languageUpdate._failure = true;
                            return languageUpdate;
                        }
                    }
                    languageUpdate.Language.CreatedBy = getLanguageData.CreatedBy;
                    languageUpdate.Language.CreatedOn = getLanguageData.CreatedOn;
                    languageUpdate.Language.UpdatedOn = DateTime.Now;
                    languageUpdate.Language.Lcid = getLanguageData.Lcid;
                    this.context.Entry(getLanguageData).CurrentValues.SetValues(languageUpdate.Language);
                    this.context?.SaveChanges();
                    languageUpdate._failure = false;
                    languageUpdate._message = "Language Updated Successfully";

                }
            }
            return languageUpdate;
        }

        /// <summary>
        /// Get All Language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LanguageListModel GetAllLanguage(LanguageListModel language)
        {
            LanguageListModel languageListModel = new LanguageListModel();
            try
            {
                var languages = this.context?.Language.Select(e=> new Language()
                { 
                    LangId=e.LangId,
                    Lcid=e.Lcid,
                    Locale=e.Locale,
                    LanguageCode=e.LanguageCode,
                    CreatedBy = (e.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == language.TenantId && u.EmailAddress == e.CreatedBy).Name : null,
                    CreatedOn = e.CreatedOn,
                    UpdatedBy = (e.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == language.TenantId && u.EmailAddress == e.UpdatedBy).Name : null,
                    UpdatedOn = e.UpdatedOn
                }).ToList();

                languageListModel.TableLanguage = languages;

                if (languages.Count > 0)
                {
                    languageListModel._failure = false;
                }
                else
                {
                    languageListModel._failure = true;
                    languageListModel._message = NORECORDFOUND;
                }
                languageListModel._tenantName = language._tenantName;
                languageListModel._token = language._token;

            }
            catch (Exception es)
            {
                languageListModel._message = es.Message;
                languageListModel._failure = true;
                languageListModel._tenantName = language._tenantName;
                languageListModel._token = language._token;
            }
            return languageListModel;

        }

        /// <summary>
        /// Delete Language Value By Id
        /// </summary>
        /// <param name="languageAddModel"></param>
        /// <returns></returns>
        public LanguageAddModel DeleteLanguage(LanguageAddModel languageAddModel)
        {
            LanguageAddModel deleteLanguageModel = new LanguageAddModel();
            try
            {
                var getLanguageValue = this.context?.Language.FirstOrDefault(x => x.LangId == languageAddModel.Language.LangId);

                if (getLanguageValue != null)
                {

                    var getStudentLanguageData = this.context?.StudentMaster.Where(x => x.FirstLanguageId == languageAddModel.Language.LangId || x.SecondLanguageId == languageAddModel.Language.LangId || x.ThirdLanguageId == languageAddModel.Language.LangId).ToList();

                    if (getLanguageValue.Lcid != null && (getLanguageValue.Lcid.ToLower() == "en-us".ToLower() || getLanguageValue.Lcid.ToLower() == "fr-fr".ToLower() || getLanguageValue.Lcid.ToLower() == "es-es".ToLower()))
                    {
                        deleteLanguageModel._message = "This Language is not deletable";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }
                    if (getStudentLanguageData.Count > 0)
                    {
                        deleteLanguageModel._message = "Language cannot be deleted because it has its association";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }
                    var getStaffLanguageData = this.context?.StaffMaster.Where(x => x.FirstLanguage == languageAddModel.Language.LangId || x.SecondLanguage == languageAddModel.Language.LangId || x.ThirdLanguage == languageAddModel.Language.LangId).ToList();
                    if (getStaffLanguageData.Count > 0)
                    {
                        deleteLanguageModel._message = "Language cannot be deleted because it has its association";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }
                    var getUserLanguageData = this.context?.UserMaster.Where(x => x.LangId == languageAddModel.Language.LangId).ToList();
                    if (getUserLanguageData.Count > 0)
                    {
                        deleteLanguageModel._message = "Language cannot be deleted because it has its association";
                        deleteLanguageModel._failure = true;
                        return deleteLanguageModel;
                    }

                    this.context?.Language.Remove(getLanguageValue);
                    this.context?.SaveChanges();
                    deleteLanguageModel._failure = false;
                    deleteLanguageModel._message = "Language Deleted Successfully";
                }
            }
            catch (Exception ex)
            {
                deleteLanguageModel._failure = true;
                deleteLanguageModel._message = ex.Message;
            }
            return deleteLanguageModel;
        }

        /// <summary>
        /// Add Dropdown Value
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel AddDropdownValue(DropdownValueAddModel dpdownValue)
        {
            var getDpvalue = this.context?.DpdownValuelist.FirstOrDefault(x => x.LovColumnValue.ToLower() == dpdownValue.DropdownValue.LovColumnValue.ToLower() && x.LovName.ToLower() == dpdownValue.DropdownValue.LovName.ToLower() && x.SchoolId == dpdownValue.DropdownValue.SchoolId);

            if (getDpvalue != null)
            {
                dpdownValue._message = "This Title Already Exist";
                dpdownValue._failure = true;
            }
            else
            {
                long? dpdownValueId = Utility.GetMaxLongPK(this.context, new Func<DpdownValuelist, long>(x => x.Id));
                dpdownValue.DropdownValue.Id = (long)dpdownValueId;
                dpdownValue.DropdownValue.CreatedOn = DateTime.UtcNow;
                this.context?.DpdownValuelist.Add(dpdownValue.DropdownValue);
                this.context?.SaveChanges();
                dpdownValue._failure = false;
                dpdownValue._message = "Data Added Successfully";
            }
            return dpdownValue;
        }

        /// <summary>
        /// Update Dropdown Value
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel UpdateDropdownValue(DropdownValueAddModel dpdownValue)
        {
            using (var transaction = this.context.Database.BeginTransaction())
            {
                try
                {
                    var getDpdownData = this.context?.DpdownValuelist.Where(x => x.LovColumnValue.ToLower() == dpdownValue.DropdownValue.LovColumnValue.ToLower() && x.LovName.ToLower() == dpdownValue.DropdownValue.LovName.ToLower() && x.Id != dpdownValue.DropdownValue.Id && x.SchoolId == dpdownValue.DropdownValue.SchoolId).ToList();

                    if (getDpdownData.Count > 0)
                    {
                        dpdownValue._message = "This Title Already Exist";
                        dpdownValue._failure = true;
                    }
                    else
                    {

                        var getDpdownValue = this.context?.DpdownValuelist.FirstOrDefault(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Id == dpdownValue.DropdownValue.Id);

                        if (getDpdownValue != null)
                        {

                            switch (getDpdownValue.LovName)
                            {
                                case "Race":
                                    var raceValueStaff = this.context?.StaffMaster.Where(x => x.Race.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();
                                    var raceValueStudent = this.context?.StudentMaster.Where(x => x.Race.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (raceValueStaff.Count > 0)
                                    {
                                        foreach (var staff in raceValueStaff)
                                        {
                                            staff.Race = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StaffMaster.UpdateRange(raceValueStaff);
                                    }
                                    if (raceValueStudent.Count > 0)
                                    {
                                        foreach (var student in raceValueStudent)
                                        {
                                            student.Race = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StudentMaster.UpdateRange(raceValueStudent);
                                    }
                                    break;
                                case "School Level":
                                    var schoolLevelValue = this.context?.SchoolMaster.Where(x => x.SchoolLevel.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (schoolLevelValue.Count > 0)
                                    {
                                        foreach (var schoolLevel in schoolLevelValue)
                                        {
                                            schoolLevel.SchoolLevel = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolMaster.UpdateRange(schoolLevelValue);

                                    }

                                    break;
                                case "School Classification":
                                    var schoolClassificationValue = this.context?.SchoolMaster.Where(x => x.SchoolClassification.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (schoolClassificationValue.Count > 0)
                                    {
                                        foreach (var schoolClassification in schoolClassificationValue)
                                        {
                                            schoolClassification.SchoolClassification = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolMaster.UpdateRange(schoolClassificationValue);
                                    }

                                    break;
                                case "Female Toilet Type":
                                    var femaleToiletTypeValue = this.context?.SchoolDetail.Where(x => x.FemaleToiletType.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (femaleToiletTypeValue.Count > 0)
                                    {
                                        foreach (var femaleToilet in femaleToiletTypeValue)
                                        {
                                            femaleToilet.FemaleToiletType = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolDetail.UpdateRange(femaleToiletTypeValue);
                                    }

                                    break;
                                case "Male Toilet Type":
                                    var maleToiletTypeValue = this.context?.SchoolDetail.Where(x => x.MaleToiletType.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (maleToiletTypeValue.Count > 0)
                                    {
                                        foreach (var maleToilet in maleToiletTypeValue)
                                        {
                                            maleToilet.MaleToiletType = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolDetail.UpdateRange(maleToiletTypeValue);
                                    }

                                    break;
                                case "Common Toilet Type":
                                    var commonToiletTypeValue = this.context?.SchoolDetail.Where(x => x.ComonToiletType.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (commonToiletTypeValue.Count > 0)
                                    {
                                        foreach (var ComonToilet in commonToiletTypeValue)
                                        {
                                            ComonToilet.ComonToiletType = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.SchoolDetail.UpdateRange(commonToiletTypeValue);
                                    }

                                    break;
                                case "Ethnicity":
                                    var ethnicityValueStaff = this.context?.StaffMaster.Where(x => x.Ethnicity.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();
                                    var ethnicityValueStudent = this.context?.StudentMaster.Where(x => x.Ethnicity.ToLower() == getDpdownValue.LovColumnValue.ToLower()).ToList();

                                    if (ethnicityValueStaff.Count > 0)
                                    {
                                        foreach (var staff in ethnicityValueStaff)
                                        {
                                            staff.Ethnicity = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StaffMaster.UpdateRange(ethnicityValueStaff);
                                    }
                                    if (ethnicityValueStudent.Count > 0)
                                    {
                                        foreach (var student in ethnicityValueStudent)
                                        {
                                            student.Ethnicity = dpdownValue.DropdownValue.LovColumnValue;
                                        }
                                        this.context?.StudentMaster.UpdateRange(ethnicityValueStudent);
                                    }
                                    break;
                            }
                        }

                        getDpdownValue.LovColumnValue = dpdownValue.DropdownValue.LovColumnValue;
                        getDpdownValue.UpdatedOn = DateTime.UtcNow;
                        getDpdownValue.UpdatedBy = dpdownValue.DropdownValue.UpdatedBy;
                        this.context?.SaveChanges();
                        dpdownValue._failure = false;
                        dpdownValue._message = "Data Updated Successfully";
                        transaction.Commit();
                    }
                    return dpdownValue;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    dpdownValue.DropdownValue = null;
                    dpdownValue._failure = true;
                    dpdownValue._message = ex.Message;
                    return dpdownValue;
                }
            }
        }


        /// <summary>
        /// Get Dropdown Value By Id
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel ViewDropdownValue(DropdownValueAddModel dpdownValue)
        {
            try
            {
                DropdownValueAddModel dropdownValueAddModel = new DropdownValueAddModel();
                var getDpdownValue = this.context?.DpdownValuelist.FirstOrDefault(x => x.TenantId == dpdownValue.DropdownValue.TenantId && x.SchoolId == dpdownValue.DropdownValue.SchoolId && x.Id == dpdownValue.DropdownValue.Id);
                if (getDpdownValue != null)
                {
                    dropdownValueAddModel.DropdownValue = getDpdownValue;
                    dropdownValueAddModel._tenantName = dpdownValue._tenantName;
                    dropdownValueAddModel._failure = false;
                    return dropdownValueAddModel;
                }
                else
                {
                    dropdownValueAddModel._failure = true;
                    dropdownValueAddModel._message = NORECORDFOUND;
                    return dropdownValueAddModel;
                }
            }
            catch (Exception es)
            {

                throw;
            }
        }


        /// <summary>
        /// Get All Dropdown Value
        /// </summary>
        /// <param name="dpdownList"></param>
        /// <returns></returns>
        public DropdownValueListModel GetAllDropdownValues(DropdownValueListModel dpdownList)
        {
            DropdownValueListModel dpdownListModel = new DropdownValueListModel();
            try
            {

                var dropdownList = this.context?.DpdownValuelist.Where(x => x.TenantId == dpdownList.TenantId && x.SchoolId == dpdownList.SchoolId && x.LovName.ToLower() == dpdownList.LovName.ToLower()).Select(c=>new DpdownValuelist()
                {
                    TenantId=c.TenantId,
                    SchoolId=c.SchoolId,
                    LovName=c.LovName,
                    LovColumnValue=c.LovColumnValue,
                    CreatedOn=c.CreatedOn,
                    UpdatedOn=c.UpdatedOn,
                    CreatedBy= (c.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == dpdownList.TenantId && u.EmailAddress == c.CreatedBy).Name : null,
                    UpdatedBy= (c.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == dpdownList.TenantId && u.EmailAddress == c.UpdatedBy).Name : null,
                }).ToList();

                dpdownListModel.dropdownList = dropdownList;

                if (dropdownList.Count > 0)
                {
                    dpdownListModel._failure = false;
                }
                else
                {
                    dpdownListModel._failure = true;
                    dpdownListModel._message = NORECORDFOUND;
                }
                dpdownListModel._tenantName = dpdownList._tenantName;
                dpdownListModel._token = dpdownList._token;

            }
            catch (Exception es)
            {
                dpdownListModel._message = es.Message;
                dpdownListModel._failure = true;
                dpdownListModel._tenantName = dpdownList._tenantName;
                dpdownListModel._token = dpdownList._token;
            }
            return dpdownListModel;

        }

        /// <summary>
        /// Add Country
        /// </summary>
        /// <param name="countryAddModel"></param>
        /// <returns></returns>
        public CountryAddModel AddCountry(CountryAddModel countryAddModel)
        {
            try
            {
                var getCountry = this.context?.Country.FirstOrDefault(x => x.Name.ToLower() == countryAddModel.country.Name.ToLower());

                if (getCountry != null)
                {
                    countryAddModel._message = "This Country Name Already Exist";
                    countryAddModel._failure = true;
                }
                else
                {
                    int? CountryId = Utility.GetMaxPK(this.context, new Func<Country, int>(x => x.Id));
                    countryAddModel.country.Id = (int)CountryId;
                    countryAddModel.country.CreatedOn = DateTime.UtcNow;
                    this.context?.Country.Add(countryAddModel.country);
                    this.context?.SaveChanges();
                    countryAddModel._failure = false;
                    countryAddModel._message = "Country Added Successfully";
                }
            }
            catch (Exception es)
            {
                countryAddModel._message = es.Message;
                countryAddModel._failure = true;
                countryAddModel._tenantName = countryAddModel._tenantName;
                countryAddModel._token = countryAddModel._token;
            }
            return countryAddModel;
        }

        /// <summary>
        /// Update Country
        /// </summary>
        /// <param name="countryAddModel"></param>
        /// <returns></returns>
        public CountryAddModel UpdateCountry(CountryAddModel countryAddModel)
        {
            try
            {
                var getCountry = this.context?.Country.Where(x => x.Name.ToLower() == countryAddModel.country.Name.ToLower() && x.Id != countryAddModel.country.Id).ToList();

                if (getCountry.Count > 0)
                {
                    countryAddModel._message = "This Country Name Already Exist";
                    countryAddModel._failure = true;
                }
                else
                {
                    var getCountryData = this.context?.Country.FirstOrDefault(x => x.Id == countryAddModel.country.Id);

                    countryAddModel.country.CreatedBy = getCountryData.CreatedBy;
                    countryAddModel.country.CreatedOn = getCountryData.CreatedOn;
                    countryAddModel.country.UpdatedOn = DateTime.Now;
                    this.context.Entry(getCountryData).CurrentValues.SetValues(countryAddModel.country);
                    this.context?.SaveChanges();
                    countryAddModel._failure = false;
                    countryAddModel._message = "Country Updated Successfully";
                }
            }
            catch (Exception es)
            {
                countryAddModel._message = es.Message;
                countryAddModel._failure = true;
                countryAddModel._tenantName = countryAddModel._tenantName;
                countryAddModel._token = countryAddModel._token;
            }
            return countryAddModel;
        }

        /// <summary>
        /// Delete Country
        /// </summary>
        /// <param name="countryDeleteModel"></param>
        /// <returns></returns>
        public CountryAddModel DeleteCountry(CountryAddModel countryDeleteModel)
        {
            CountryAddModel deleteCountryModel = new CountryAddModel();
            try
            {
                var getCountryValue = this.context?.Country.FirstOrDefault(x => x.Id == countryDeleteModel.country.Id);

                if (getCountryValue != null)
                {
                    var getPatrentCountryData = this.context?.ParentAddress.Where(x => x.Country == countryDeleteModel.country.Id.ToString()).ToList();
                    if (getPatrentCountryData.Count > 0)
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getStudentCountryData = this.context?.StudentMaster.Where(x => x.CountryOfBirth == countryDeleteModel.country.Id || x.HomeAddressCountry == countryDeleteModel.country.Id.ToString() || x.MailingAddressCountry == countryDeleteModel.country.Id.ToString()).ToList();
                    if (getStudentCountryData.Count > 0)
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getStaffCountryData = this.context?.StaffMaster.Where(x => x.CountryOfBirth == countryDeleteModel.country.Id || x.HomeAddressCountry == countryDeleteModel.country.Id.ToString() || x.MailingAddressCountry == countryDeleteModel.country.Id.ToString()).ToList();
                    if (getStaffCountryData.Count > 0)
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getSchoolCountryData = this.context?.SchoolMaster.Where(x => x.Country == countryDeleteModel.country.Id.ToString()).ToList();
                    if (getSchoolCountryData.Count > 0)
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }
                    var getSateData = this.context?.State.Where(x => x.CountryId == countryDeleteModel.country.Id).ToList();
                    if (getSateData.Count > 0)
                    {
                        deleteCountryModel._message = "Country cannot be deleted because it has its association";
                        deleteCountryModel._failure = true;
                        return deleteCountryModel;
                    }

                    this.context?.Country.Remove(getCountryValue);
                    this.context?.SaveChanges();
                    deleteCountryModel._failure = false;
                    deleteCountryModel._message = "Country Deleted Successfully";

                }

            }
            catch (Exception ex)
            {
                deleteCountryModel._failure = true;
                deleteCountryModel._message = ex.Message;
            }
            return deleteCountryModel;
        }

        /// <summary>
        /// Delete Dropdown Value
        /// </summary>
        /// <param name="dpdownValue"></param>
        /// <returns></returns>
        public DropdownValueAddModel DeleteDropdownValue(DropdownValueAddModel dpdownValue)
        {
            DropdownValueAddModel dropdownValueAddModel = new DropdownValueAddModel();
            try
            {
                var getDpValue = this.context?.DpdownValuelist.FirstOrDefault(x => x.Id == dpdownValue.DropdownValue.Id);

                if (getDpValue != null)
                {

                    switch (getDpValue.LovName)
                    {
                        case "Race":

                            var raceValueStaff = this.context?.StaffMaster.Where(x => x.Race.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();
                            if (raceValueStaff.Count > 0)
                            {
                                dropdownValueAddModel._message = "Race cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            var raceValueStudent = this.context?.StudentMaster.Where(x => x.Race.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (raceValueStudent.Count > 0)
                            {
                                dropdownValueAddModel._message = "Race cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }


                            break;
                        case "School Level":
                            var schoolLevelValue = this.context?.SchoolMaster.Where(x => x.SchoolLevel.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (schoolLevelValue.Count > 0)
                            {
                                dropdownValueAddModel._message = "School Level cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "School Classification":
                            var schoolClassificationValue = this.context?.SchoolMaster.Where(x => x.SchoolClassification.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (schoolClassificationValue.Count > 0)
                            {
                                dropdownValueAddModel._message = "School Classification cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Female Toilet Type":
                            var femaleToiletTypeValue = this.context?.SchoolDetail.Where(x => x.FemaleToiletType.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (femaleToiletTypeValue.Count > 0)
                            {
                                dropdownValueAddModel._message = "Female Toilet Type cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Male Toilet Type":
                            var maleToiletTypeValue = this.context?.SchoolDetail.Where(x => x.MaleToiletType.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (maleToiletTypeValue.Count > 0)
                            {
                                dropdownValueAddModel._message = "Male Toilet Type cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Common Toilet Type":
                            var commonToiletTypeValue = this.context?.SchoolDetail.Where(x => x.ComonToiletType.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (commonToiletTypeValue.Count > 0)
                            {
                                dropdownValueAddModel._message = "Common Toilet Type cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                        case "Ethnicity":
                            var ethnicityValueStaff = this.context?.StaffMaster.Where(x => x.Ethnicity.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();
                            if (ethnicityValueStaff.Count > 0)
                            {
                                dropdownValueAddModel._message = "Ethnicity cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }
                            var ethnicityValueStudent = this.context?.StudentMaster.Where(x => x.Ethnicity.ToLower() == getDpValue.LovColumnValue.ToLower() && x.SchoolId == getDpValue.SchoolId).ToList();

                            if (ethnicityValueStudent.Count > 0)
                            {
                                dropdownValueAddModel._message = "Ethnicity cannot be deleted because it has its association";
                                dropdownValueAddModel._failure = true;
                                return dropdownValueAddModel;
                            }

                            break;
                    }
                    this.context?.DpdownValuelist.Remove(getDpValue);
                    this.context?.SaveChanges();
                    dropdownValueAddModel._failure = false;
                    dropdownValueAddModel._message = "Data Deleted Successfully";
                }

            }
            catch (Exception es)
            {
                dropdownValueAddModel._failure = true;
                dropdownValueAddModel._message = es.Message;
            }
            return dropdownValueAddModel;

        }

        /// <summary>
        /// Get All Language For Login 
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public LanguageListModel GetAllLanguageForLogin(LanguageListModel language)
        {
            LanguageListModel languageListModel = new LanguageListModel();
            try
            {
                languageListModel.TableLanguage = null;
                var languages = this.context?.Language.Where(x => x.Lcid.ToLower() == "en-us".ToLower() || x.Lcid.ToLower() == "fr-fr".ToLower() || x.Lcid.ToLower() == "es-es".ToLower()).Select(n=> new Language()
                {
                    LangId=n.LangId,
                    Lcid=n.Lcid,
                    Locale=n.Locale,
                    LanguageCode=n.LanguageCode,
                    CreatedOn=n.CreatedOn,
                    UpdatedOn=n.UpdatedOn,
                    CreatedBy = (n.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == language.TenantId && u.EmailAddress == n.CreatedBy).Name : null,
                    UpdatedBy = (n.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == language.TenantId && u.EmailAddress == n.UpdatedBy).Name : null,
                }).ToList();
                
                if (languages.Count > 0)
                {
                    languageListModel.TableLanguage = languages;
                    languageListModel._failure = false;
                }
                else
                {
                    languageListModel._failure = true;
                    languageListModel._message = NORECORDFOUND;
                }
                languageListModel._tenantName = language._tenantName;
                languageListModel._token = language._token;

            }
            catch (Exception es)
            {
                languageListModel._message = es.Message;
                languageListModel._failure = true;
                languageListModel._tenantName = language._tenantName;
                languageListModel._token = language._token;
            }
            return languageListModel;

        }

        /// <summary>
        /// Dashboard View
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <returns></returns>
        public DashboardViewModel GetDashboardView(DashboardViewModel dashboardViewModel)
        {
            DashboardViewModel dashboardView = new DashboardViewModel();
            try
            {
                var todayDate = DateTime.Today;
                dashboardView.TenantId = dashboardViewModel.TenantId;
                dashboardView.SchoolId = dashboardViewModel.SchoolId;
                dashboardView.AcademicYear = dashboardViewModel.AcademicYear;

                dashboardView.SuperAdministratorName = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.MembershipId == dashboardViewModel.MembershipId)?.Name;

                dashboardView.SchoolName = this.context?.SchoolMaster.FirstOrDefault(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId)?.SchoolName;

                dashboardView.TotalStudent = this.context?.StudentMaster.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.IsActive == true).Count();

                //dashboardView.TotalParent = this.context?.ParentAssociationship.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.Associationship == true).Select(s => new { s.TenantId, s.SchoolId, s.ParentId }).Distinct().ToList().Count();

                //dashboardView.TotalStaff = this.context?.StaffMaster.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId).Count();

                dashboardView.TotalStaff = this.context?.StaffSchoolInfo.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolAttachedId == dashboardViewModel.SchoolId && (x.EndDate == null || x.EndDate >= DateTime.UtcNow.Date)).Count();
                
                dashboardView.TotalParent = this.context?.ParentAssociationship.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.Associationship == true).Select(x => x.ParentId).Distinct().Count();

                var notice = this.context?.Notice.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.Isactive == true && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).FirstOrDefault();
                if (notice != null)
                {
                    dashboardView.NoticeTitle = notice.Title;
                    dashboardView.NoticeBody = notice.Body;
                }

                dashboardView._tenantName = dashboardViewModel._tenantName;
                dashboardView._token = dashboardViewModel._token;
            }
            catch (Exception es)
            {
                dashboardView._failure = true;
                dashboardView._message = es.Message;
            }
            return dashboardView;
        }

        /// <summary>
        /// Get Release Number
        /// </summary>
        /// <param name="releaseNumberAddViewModel"></param>
        /// <returns></returns>
        public ReleaseNumberAddViewModel GetReleaseNumber(ReleaseNumberAddViewModel releaseNumberAddViewModel)
        {
            ReleaseNumberAddViewModel ReleaseNumberViewModel = new ReleaseNumberAddViewModel();
            try
            {
                var releaseNumber = this.context?.ReleaseNumber.OrderByDescending(c => c.ReleaseDate).FirstOrDefault(c => c.SchoolId == releaseNumberAddViewModel.releaseNumber.SchoolId && c.TenantId == releaseNumberAddViewModel.releaseNumber.TenantId);

                ReleaseNumberViewModel.releaseNumber = releaseNumber;

                if (releaseNumber != null)
                {
                    ReleaseNumberViewModel._failure = false;
                }
                else
                {
                    ReleaseNumberViewModel._message = NORECORDFOUND;
                    ReleaseNumberViewModel._failure = true;
                }
                ReleaseNumberViewModel._tenantName = releaseNumberAddViewModel._tenantName;
                ReleaseNumberViewModel._token = releaseNumberAddViewModel._token;
            }
            catch (Exception es)
            {
                ReleaseNumberViewModel._message = es.Message;
                ReleaseNumberViewModel._failure = true;
                ReleaseNumberViewModel._tenantName = releaseNumberAddViewModel._tenantName;
                ReleaseNumberViewModel._token = releaseNumberAddViewModel._token;
            }
            return ReleaseNumberViewModel;
        }

        /// <summary>
        /// Add Search Filter
        /// </summary>
        /// <param name="searchFilterAddViewModel"></param>
        /// <returns></returns>
        public SearchFilterAddViewModel AddSearchFilter(SearchFilterAddViewModel searchFilterAddViewModel)
        {
            try
            {
                var checkFilterName = this.context?.SearchFilter.Where(x => x.SchoolId == searchFilterAddViewModel.searchFilter.SchoolId && x.TenantId == searchFilterAddViewModel.searchFilter.TenantId && x.FilterName.ToLower() == searchFilterAddViewModel.searchFilter.FilterName.ToLower() && x.Module.ToLower() == searchFilterAddViewModel.searchFilter.Module.ToLower()).FirstOrDefault();

                if (checkFilterName != null)
                {
                    searchFilterAddViewModel._failure = true;
                    searchFilterAddViewModel._message = "Filter Name Already Exists";
                }
                else
                {
                    int? FilterId = 1;

                    var searchFilterData = this.context?.SearchFilter.Where(x => x.SchoolId == searchFilterAddViewModel.searchFilter.SchoolId && x.TenantId == searchFilterAddViewModel.searchFilter.TenantId).OrderByDescending(x => x.FilterId).FirstOrDefault();

                    if (searchFilterData != null)
                    {
                        FilterId = searchFilterData.FilterId + 1;
                    }

                    searchFilterAddViewModel.searchFilter.FilterId = (int)FilterId;
                    searchFilterAddViewModel.searchFilter.CreatedOn = DateTime.UtcNow;
                    this.context?.SearchFilter.Add(searchFilterAddViewModel.searchFilter);
                    this.context?.SaveChanges();
                    searchFilterAddViewModel._failure = false;
                    searchFilterAddViewModel._message = "Search Filter Added Successfully";
                }
            }
            catch (Exception es)
            {
                searchFilterAddViewModel._failure = true;
                searchFilterAddViewModel._message = es.Message;
            }
            return searchFilterAddViewModel;
        }

        /// <summary>
        /// Update Search Filter
        /// </summary>
        /// <param name="searchFilterAddViewModel"></param>
        /// <returns></returns>
        public SearchFilterAddViewModel UpdateSearchFilter(SearchFilterAddViewModel searchFilterAddViewModel)
        {
            try
            {
                var searchFilterUpdate = this.context?.SearchFilter.FirstOrDefault(x => x.TenantId == searchFilterAddViewModel.searchFilter.TenantId && x.SchoolId == searchFilterAddViewModel.searchFilter.SchoolId && x.FilterId == searchFilterAddViewModel.searchFilter.FilterId);

                if (searchFilterUpdate != null)
                {
                    var checkFilterName = this.context?.SearchFilter.Where(x => x.SchoolId == searchFilterAddViewModel.searchFilter.SchoolId && x.TenantId == searchFilterAddViewModel.searchFilter.TenantId && x.FilterId != searchFilterAddViewModel.searchFilter.FilterId && x.FilterName.ToLower() == searchFilterAddViewModel.searchFilter.FilterName.ToLower() && x.Module.ToLower() == searchFilterAddViewModel.searchFilter.Module.ToLower()).FirstOrDefault();

                    if (checkFilterName != null)
                    {
                        searchFilterAddViewModel._failure = true;
                        searchFilterAddViewModel._message = "Filter Name Already Exists";
                    }
                    else
                    {
                        searchFilterAddViewModel.searchFilter.FilterName = searchFilterUpdate.FilterName;
                        searchFilterAddViewModel.searchFilter.CreatedBy = searchFilterUpdate.CreatedBy;
                        searchFilterAddViewModel.searchFilter.CreatedOn = searchFilterUpdate.CreatedOn;
                        searchFilterAddViewModel.searchFilter.UpdatedOn = DateTime.Now;
                        this.context.Entry(searchFilterUpdate).CurrentValues.SetValues(searchFilterAddViewModel.searchFilter);
                        this.context?.SaveChanges();
                        searchFilterAddViewModel._failure = false;
                        searchFilterAddViewModel._message = "Search Filter Updated Successfully";
                    }
                }
                else
                {
                    searchFilterAddViewModel._failure = true;
                    searchFilterAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                searchFilterAddViewModel._failure = true;
                searchFilterAddViewModel._message = es.Message;
            }
            return searchFilterAddViewModel;
        }

        /// <summary>
        /// Delete Search Filter
        /// </summary>
        /// <param name="searchFilterAddViewModel"></param>
        /// <returns></returns>
        public SearchFilterAddViewModel DeleteSearchFilter(SearchFilterAddViewModel searchFilterAddViewModel)
        {
            try
            {
                var searchFilterDelete = this.context?.SearchFilter.FirstOrDefault(x => x.TenantId == searchFilterAddViewModel.searchFilter.TenantId && x.SchoolId == searchFilterAddViewModel.searchFilter.SchoolId && x.FilterId == searchFilterAddViewModel.searchFilter.FilterId);

                if (searchFilterDelete != null)
                {
                    this.context?.SearchFilter.Remove(searchFilterDelete);
                    this.context?.SaveChanges();
                    searchFilterAddViewModel._failure = false;
                    searchFilterAddViewModel._message = "Search Filter Deleted Successfully";

                }
                else
                {
                    searchFilterAddViewModel._failure = true;
                    searchFilterAddViewModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {

                searchFilterAddViewModel._failure = true;
                searchFilterAddViewModel._message = es.Message;
            }
            return searchFilterAddViewModel;
        }

        /// <summary>
        /// Get All Search Filter
        /// </summary>
        /// <param name="searchFilterListViewModel"></param>
        /// <returns></returns>
        public SearchFilterListViewModel GetAllSearchFilter(SearchFilterListViewModel searchFilterListViewModel)
        {
            SearchFilterListViewModel searchFilterListModel = new SearchFilterListViewModel();
            try
            {

                var searchFilterList = this.context?.SearchFilter.Where(x => x.TenantId == searchFilterListViewModel.TenantId && x.SchoolId == searchFilterListViewModel.SchoolId && x.Module.ToLower() == searchFilterListViewModel.Module.ToLower()).Select(c=> new SearchFilter()
                {
                    TenantId=c.TenantId,
                    SchoolId=c.SchoolId,
                    Module=c.Module,
                    FilterId=c.FilterId,
                    FilterName=c.FilterName,
                    Emailaddress=c.Emailaddress,
                    JsonList=c.JsonList,
                    CreatedOn=c.CreatedOn,
                    UpdatedOn=c.UpdatedOn,
                    CreatedBy = (c.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == searchFilterListViewModel.TenantId && u.EmailAddress == c.CreatedBy).Name : null,
                    UpdatedBy = (c.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == searchFilterListViewModel.TenantId && u.EmailAddress == c.UpdatedBy).Name : null,
                }).ToList();

                searchFilterListModel.searchFilterList = searchFilterList;

                if (searchFilterList.Count > 0)
                {
                    searchFilterListModel._tenantName = searchFilterListViewModel._tenantName;
                    searchFilterListModel._token = searchFilterListViewModel._token;
                    searchFilterListModel._failure = false;
                }
                else
                {
                    searchFilterListModel._tenantName = searchFilterListViewModel._tenantName;
                    searchFilterListModel._token = searchFilterListViewModel._token;
                    searchFilterListModel._failure = true;
                    searchFilterListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                searchFilterListModel._message = es.Message;
                searchFilterListModel._failure = true;
                searchFilterListModel._tenantName = searchFilterListViewModel._tenantName;
                searchFilterListModel._token = searchFilterListViewModel._token;
            }
            return searchFilterListModel;
        }

        /// <summary>
        /// Get All Grade Age Range
        /// </summary>
        /// <param name="gradeAgeRangeListViewModel"></param>
        /// <returns></returns>
        public GradeAgeRangeListViewModel GetAllGradeAgeRange(GradeAgeRangeListViewModel gradeAgeRangeListViewModel)
        {
            GradeAgeRangeListViewModel gradeAgeRangeListModel = new GradeAgeRangeListViewModel();
            try
            {
                var gradeAgeRangeList = this.context?.GradeAgeRange.Select(s=> new GradeAgeRange()
                {
                    AgeRangeId=s.AgeRangeId,
                    AgeRange=s.AgeRange,
                    CreatedOn=s.CreatedOn,
                    UpdatedOn=s.UpdatedOn,
                    CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == gradeAgeRangeListViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null,
                    UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == gradeAgeRangeListViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null,
                }).ToList();
                gradeAgeRangeListModel.gradeAgeRangeList = gradeAgeRangeList;
                gradeAgeRangeListModel._tenantName = gradeAgeRangeListViewModel._tenantName;
                gradeAgeRangeListModel._token = gradeAgeRangeListViewModel._token;

                if (gradeAgeRangeList.Count > 0)
                {
                    gradeAgeRangeListModel._failure = false;
                }
                else
                {
                    gradeAgeRangeListModel._failure = true;
                    gradeAgeRangeListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradeAgeRangeListModel._message = es.Message;
                gradeAgeRangeListModel._failure = true;
                gradeAgeRangeListModel._tenantName = gradeAgeRangeListViewModel._tenantName;
                gradeAgeRangeListModel._token = gradeAgeRangeListViewModel._token;
            }
            return gradeAgeRangeListModel;
        }

        /// <summary>
        /// Get All Grade Educational Stage
        /// </summary>
        /// <param name="gradeEducationalStageListViewModel"></param>
        /// <returns></returns>
        public GradeEducationalStageListViewModel GetAllGradeEducationalStage(GradeEducationalStageListViewModel gradeEducationalStageListViewModel)
        {
            GradeEducationalStageListViewModel gradeEducationalStageListModel = new GradeEducationalStageListViewModel();
            try
            {
                var gradeEducationalStageList = this.context?.GradeEducationalStage.Select(s=> new GradeEducationalStage()
                {
                    IscedCode=s.IscedCode,
                    EducationalStage=s.EducationalStage,
                    CreatedOn = s.CreatedOn,
                    UpdatedOn = s.UpdatedOn,
                    CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == gradeEducationalStageListViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null,
                    UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == gradeEducationalStageListViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null,
                }).ToList();
                gradeEducationalStageListModel.gradeEducationalStageList = gradeEducationalStageList;
                gradeEducationalStageListModel._tenantName = gradeEducationalStageListViewModel._tenantName;
                gradeEducationalStageListModel._token = gradeEducationalStageListViewModel._token;

                if (gradeEducationalStageList.Count > 0)
                {
                    gradeEducationalStageListModel._failure = false;
                }
                else
                {
                    gradeEducationalStageListModel._failure = true;
                    gradeEducationalStageListModel._message = NORECORDFOUND;
                }
            }
            catch (Exception es)
            {
                gradeEducationalStageListModel._message = es.Message;
                gradeEducationalStageListModel._failure = true;
                gradeEducationalStageListModel._tenantName = gradeEducationalStageListViewModel._tenantName;
                gradeEducationalStageListModel._token = gradeEducationalStageListViewModel._token;
            }
            return gradeEducationalStageListModel;
        }

        /// <summary>
        /// Get Dashboard View For Staff
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetDashboardViewForStaff(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            List<StaffCoursesectionSchedule> staffCoursesectionScheduleList = new List<StaffCoursesectionSchedule>();
            List<AllCourseSectionView> allCourseSectionVewList = new List<AllCourseSectionView>();

            CourseFixedSchedule fixedData = null;
            List<CourseVariableSchedule> variableData = new List<CourseVariableSchedule>();
            List<CourseCalendarSchedule> calenderData = new List<CourseCalendarSchedule>();
            List<CourseBlockSchedule> blockData = new List<CourseBlockSchedule>();

            try
            {
                var todayDate = DateTime.Today;

                scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                scheduledCourseSectionView.StaffId = scheduledCourseSectionViewModel.StaffId;
                scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                scheduledCourseSectionView._userName = scheduledCourseSectionViewModel._userName;
                scheduledCourseSectionView.AllCourse = scheduledCourseSectionViewModel.AllCourse;

                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Include(x => x.CourseSection.Course).Include(x => x.CourseSection.SchoolCalendars).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.StaffId == scheduledCourseSectionViewModel.StaffId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData.Count > 0)
                {
                    if (scheduledCourseSectionViewModel.AllCourse != true)
                    {
                        staffCoursesectionScheduleList = scheduledCourseSectionData.Where(x => x.MeetingDays.ToLower().Contains(todayDate.DayOfWeek.ToString().ToLower()) || x.MeetingDays == "Calendar Days").ToList();
                    }
                    else
                    {
                        staffCoursesectionScheduleList = scheduledCourseSectionData;
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                    return scheduledCourseSectionView;
                }


                if (staffCoursesectionScheduleList.Count > 0)
                {
                    foreach (var scheduledCourseSection in staffCoursesectionScheduleList)
                    {
                        if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)" || scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)"|| scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                        {
                            CourseSectionViewList CourseSections = new CourseSectionViewList();

                            CourseSections.CourseTitle = scheduledCourseSection.CourseSection.Course.CourseTitle;

                            if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                            {
                                CourseSections.ScheduleType = "Fixed Schedule";

                                fixedData = this.context?.CourseFixedSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).FirstOrDefault(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId);

                                if (fixedData != null)
                                {
                                    fixedData.BlockPeriod.CourseFixedSchedule = null;
                                    fixedData.BlockPeriod.CourseVariableSchedule = null;
                                    fixedData.BlockPeriod.CourseCalendarSchedule = null;
                                    fixedData.BlockPeriod.CourseBlockSchedule = null;
                                    fixedData.BlockPeriod.StudentAttendance = null;
                                    fixedData.Rooms.CourseFixedSchedule = null;
                                    fixedData.Rooms.CourseVariableSchedule = null;
                                    fixedData.Rooms.CourseCalendarSchedule = null;
                                    fixedData.Rooms.CourseBlockSchedule = null;
                                    CourseSections.courseFixedSchedule = fixedData;
                                }
                            }
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                CourseSections.ScheduleType = "Variable Schedule";

                                variableData = this.context?.CourseVariableSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (variableData.Count > 0)
                                {
                                    variableData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                                    CourseSections.courseVariableSchedule = variableData;
                                }
                            }
                            //if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            //{
                            //    CourseSections.ScheduleType = "Calendar Schedule";

                            //    calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                            //    if (calenderData.Count > 0)
                            //    {
                            //        calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                            //        CourseSections.courseCalendarSchedule = calenderData;
                            //    }
                            //    else
                            //    {
                            //        break;
                            //    }
                            //}
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                            {
                                CourseSections.ScheduleType = "Block Schedule";

                                blockData = this.context?.CourseBlockSchedule.Include(v=>v.Block).Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (blockData.Count > 0)
                                {
                                    blockData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                                    CourseSections.courseBlockSchedule = blockData;
                                }
                            }
                            CourseSections.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                            CourseSections.CourseId = scheduledCourseSection.CourseId;
                            CourseSections.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
                            CourseSections.CourseSectionId = scheduledCourseSection.CourseSectionId;
                            CourseSections.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                            CourseSections.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                            CourseSections.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                            CourseSections.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                            CourseSections.CourseSectionName = scheduledCourseSection.CourseSectionName;
                            CourseSections.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                            CourseSections.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                            CourseSections.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                            CourseSections.DurationStartDate = scheduledCourseSection.DurationStartDate;
                            CourseSections.DurationEndDate = scheduledCourseSection.DurationEndDate;
                            CourseSections.MeetingDays = scheduledCourseSection.MeetingDays;
                            CourseSections.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
                            CourseSections.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars.Days;

                            scheduledCourseSectionView.courseSectionViewList.Add(CourseSections);
                        }
                        else
                        {
                            if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                            {
                                CourseSectionViewList CourseSection = new CourseSectionViewList();
                                CourseSection.ScheduleType = "Calendar Schedule";

                                calenderData = this.context?.CourseCalendarSchedule.Include(v => v.BlockPeriod).Include(f => f.Rooms).Where(c => (scheduledCourseSectionViewModel.AllCourse != true) ? c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId && c.Date.Value.Date == todayDate.Date : c.SchoolId == scheduledCourseSection.SchoolId && c.TenantId == scheduledCourseSection.TenantId && c.CourseSectionId == scheduledCourseSection.CourseSectionId).ToList();

                                if (calenderData.Count > 0)
                                {
                                    calenderData.ForEach(x => { x.BlockPeriod.CourseFixedSchedule = null; x.BlockPeriod.CourseVariableSchedule = null; x.BlockPeriod.CourseCalendarSchedule = null; x.BlockPeriod.CourseBlockSchedule = null; x.BlockPeriod.StudentAttendance = null; x.Rooms.CourseFixedSchedule = null; x.Rooms.CourseVariableSchedule = null; x.Rooms.CourseCalendarSchedule = null; x.Rooms.CourseBlockSchedule = null; });

                                    CourseSection.courseCalendarSchedule = calenderData;

                                    CourseSection.CalendarId = scheduledCourseSection.CourseSection.CalendarId;
                                    CourseSection.CourseId = scheduledCourseSection.CourseId;
                                    CourseSection.CourseGradeLevel = scheduledCourseSection.CourseSection.Course.CourseGradeLevel;
                                    CourseSection.CourseSectionId = scheduledCourseSection.CourseSectionId;
                                    CourseSection.GradeScaleType = scheduledCourseSection.CourseSection.GradeScaleType;
                                    CourseSection.AttendanceCategoryId = scheduledCourseSection.CourseSection.AttendanceCategoryId;
                                    CourseSection.GradeScaleId = scheduledCourseSection.CourseSection.GradeScaleId;
                                    CourseSection.StandardGradeScaleId = scheduledCourseSection.CourseSection.StandardGradeScaleId;
                                    CourseSection.CourseSectionName = scheduledCourseSection.CourseSectionName;
                                    CourseSection.YrMarkingPeriodId = scheduledCourseSection.YrMarkingPeriodId;
                                    CourseSection.SmstrMarkingPeriodId = scheduledCourseSection.SmstrMarkingPeriodId;
                                    CourseSection.QtrMarkingPeriodId = scheduledCourseSection.QtrMarkingPeriodId;
                                    CourseSection.DurationStartDate = scheduledCourseSection.DurationStartDate;
                                    CourseSection.DurationEndDate = scheduledCourseSection.DurationEndDate;
                                    CourseSection.MeetingDays = scheduledCourseSection.MeetingDays;
                                    CourseSection.AttendanceTaken = scheduledCourseSection.CourseSection.AttendanceTaken;
                                    CourseSection.WeekDays = scheduledCourseSection.CourseSection.SchoolCalendars.Days;

                                    scheduledCourseSectionView.courseSectionViewList.Add(CourseSection);
                                }
                            }
                        }
                    }
                }
                else
                {
                    scheduledCourseSectionView._failure = true;
                    scheduledCourseSectionView._message = NORECORDFOUND;
                    return scheduledCourseSectionView;
                }

                int count = 0;

                if (scheduledCourseSectionData.Count > 0)
                {
                    allCourseSectionVewList = this.context?.AllCourseSectionView.Where(c => c.TenantId == scheduledCourseSectionViewModel.TenantId && c.SchoolId == scheduledCourseSectionViewModel.SchoolId).ToList();                    

                    foreach (var StaffScheduleData in scheduledCourseSectionData)
                    {
                        var allCourseSectionViewData = allCourseSectionVewList.Where(v => v.CourseSectionId == StaffScheduleData.CourseSectionId && v.CourseId == StaffScheduleData.CourseId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                        if (allCourseSectionViewData.Count>0)
                        {
                            if ((StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)" && StaffScheduleData.CourseSection.AttendanceTaken == true) || StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                            {
                                List<DateTime> dateList = new List<DateTime>();
                                List<string> list = new List<string>();
                                string[] meetingDays = { };


                                DateTime start = (DateTime)StaffScheduleData.CourseSection.DurationStartDate;
                                DateTime end = (DateTime)StaffScheduleData.CourseSection.DurationEndDate;

                                //if (StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                //{
                                //    list = StaffScheduleData.CourseSection.MeetingDays.Split("|").ToList();

                                //    if (list.Count > 0)
                                //    {
                                //        meetingDays = list.ToArray();
                                //    }
                                //}
                                //if (StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                //{
                                //    meetingDays = variableData.Select(c => c.Day).ToArray();
                                //}

                                meetingDays = StaffScheduleData.MeetingDays.ToLower().Split("|");

                                bool allDays = meetingDays == null || !meetingDays.Any();

                                dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                      .Select(offset => start.AddDays(offset))
                                                      .Where(d => allDays || meetingDays.Contains(d.DayOfWeek.ToString().ToLower()))
                                                      .ToList();

                                if (dateList.Count > 0)
                                {
                                    dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                }

                                foreach (var date in dateList)
                                {

                                    if (StaffScheduleData.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                    {
                                        var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == allCourseSectionViewData.FirstOrDefault().FixedPeriodId);

                                        if (staffAttendanceData.Count() == 0)
                                        {
                                            count = count + 1;
                                        }
                                    }

                                    if (StaffScheduleData.CourseSection.ScheduleType == "Variable Schedule (2)")
                                    {
                                        var courseVariableScheduleData = allCourseSectionViewData.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                        if (courseVariableScheduleData != null)
                                        {
                                            foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                            {
                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                if (staffAttendanceData.Count() == 0)
                                                {
                                                    count = count + 1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var calendarDataList = allCourseSectionViewData.Where(c => c.CalDate <= DateTime.Today.Date);

                                if (calendarDataList.Count() > 0)
                                {
                                    foreach (var calenderScheduleData in calendarDataList)
                                    {
                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == StaffScheduleData.SchoolId && b.TenantId == StaffScheduleData.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == calenderScheduleData.CalDate && b.CourseSectionId == StaffScheduleData.CourseSectionId && b.PeriodId == calenderScheduleData.CalPeriodId);

                                            if (staffAttendanceData.Count() == 0)
                                            {
                                                count = count + 1;
                                            }
                                        
                                    }
                                }
                            }
                        }
                        
                        //scheduledCourseSectionView.MissingAttendanceCount = count;
                    }
                    scheduledCourseSectionView.MissingAttendanceCount = count;


                    //var membership = this.context?.Membership.FirstOrDefault(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.Profile == "Teacher");

                    if (scheduledCourseSectionViewModel.MembershipId != null)
                    {
                        var noticeList = this.context?.Notice.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.Isactive == true && x.TargetMembershipIds.Contains(scheduledCourseSectionViewModel.MembershipId.ToString()) && (x.ValidFrom <= todayDate && todayDate <= x.ValidTo)).OrderByDescending(x => x.ValidFrom).ToList();

                        if (noticeList.Count > 0)
                        {
                            scheduledCourseSectionView.NoticeList = noticeList;
                        }
                    }
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView.courseSectionViewList = null;
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// Get Dashboard View For Calendar View
        /// </summary>
        /// <param name="dashboardViewModel"></param>
        /// <returns></returns>
        public DashboardViewModel GetDashboardViewForCalendarView(DashboardViewModel dashboardViewModel)
        {
            DashboardViewModel dashboardView = new DashboardViewModel();
            try
            {
                var defaultCalendar = this.context?.SchoolCalendars.Where(x => x.TenantId == dashboardViewModel.TenantId && x.SchoolId == dashboardViewModel.SchoolId && x.AcademicYear == dashboardViewModel.AcademicYear && x.DefaultCalender == true).FirstOrDefault();
                if (defaultCalendar != null)
                {
                    dashboardView.schoolCalendar = defaultCalendar;
                    dashboardView.schoolCalendar.SchoolMaster = null;

                    var Events = this.context?.CalendarEvents.Where(x => ((x.TenantId == dashboardViewModel.TenantId /*&& x.SchoolId == calendarEventList.SchoolId*/ && x.AcademicYear == dashboardViewModel.AcademicYear && ((x.CalendarId == defaultCalendar.CalenderId /*&& x.SystemWideEvent == false */&& x.SchoolId == dashboardViewModel.SchoolId) || x.SystemWideEvent == true)) || x.TenantId == dashboardViewModel.TenantId && x.SystemWideEvent == true && x.AcademicYear == dashboardViewModel.AcademicYear) && x.VisibleToMembershipId.Contains(dashboardViewModel.MembershipId.ToString())).Select(s=> new CalendarEvents()
                    {
                        TenantId=s.TenantId,
                        SchoolId=s.SchoolId,
                        CalendarId=s.CalendarId,
                        EventId=s.EventId,
                        AcademicYear=s.AcademicYear,
                        SchoolDate=s.SchoolDate,
                        Description=s.Description,
                        EndDate=s.EndDate,
                        EventColor=s.EventColor,
                        StartDate=s.StartDate,
                        SystemWideEvent=s.SystemWideEvent,
                        Title=s.Title,
                        VisibleToMembershipId=s.VisibleToMembershipId,
                        CreatedOn=s.CreatedOn,
                        UpdatedOn=s.UpdatedOn,
                        CreatedBy = (s.CreatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == dashboardViewModel.TenantId && u.EmailAddress == s.CreatedBy).Name : null,
                        UpdatedBy = (s.UpdatedBy != null) ? this.context.UserMaster.FirstOrDefault(u => u.TenantId == dashboardViewModel.TenantId && u.EmailAddress == s.UpdatedBy).Name : null,                         
                    }).ToList();
                    if (Events.Count > 0)
                    {
                        dashboardView.calendarEventList = Events;
                    }
                }
                dashboardView.TenantId = dashboardViewModel.TenantId;
                dashboardView.SchoolId = dashboardViewModel.SchoolId;
                dashboardView.AcademicYear = dashboardViewModel.AcademicYear;
                dashboardView._tenantName = dashboardViewModel._tenantName;
                dashboardView._userName = dashboardViewModel._userName;
                dashboardView._token = dashboardViewModel._token;
                dashboardView._failure = false;
            }
            catch (Exception es)
            {
                dashboardView._failure = true;
                dashboardView._message = es.Message;
            }
            return dashboardView;
        }

        /// <summary>
        /// Reset Password For User
        /// </summary>
        /// <param name="resetPasswordModel"></param>
        /// <returns></returns>
        public ResetPasswordModel ResetPasswordForUser(ResetPasswordModel resetPasswordModel)
        {
            ResetPasswordModel resetPassword = new ResetPasswordModel();
            try
            {
                var userMasterData = this.context?.UserMaster.Where(x => x.TenantId == resetPasswordModel.userMaster.TenantId && x.SchoolId == resetPasswordModel.userMaster.SchoolId && x.UserId == resetPasswordModel.userMaster.UserId && x.EmailAddress == resetPasswordModel.userMaster.EmailAddress).FirstOrDefault();
                if (userMasterData != null)
                {
                    var decrypted = Utility.Decrypt(resetPasswordModel.userMaster.PasswordHash);
                    string passwordHash = Utility.GetHashedPassword(decrypted);

                    userMasterData.PasswordHash = passwordHash;
                    userMasterData.UpdatedOn = DateTime.UtcNow;

                    this.context?.SaveChanges();
                    resetPassword._failure = false;
                    resetPassword._message = "Password Updated Successfully";
                }
                else
                {
                    resetPassword.userMaster = null;
                    resetPassword._failure = true;
                    resetPassword._message = "Invalid Email Address";
                }
            }
            catch (Exception es)
            {
                resetPassword._failure = true;
                resetPassword._message = es.Message;
            }
            return resetPassword;
        }

        /// <summary>
        /// Get All Field List
        /// </summary>
        /// <param name="excelHeaderModel"></param>
        /// <returns></returns>      
        public ModuleFieldListModel GetAllFieldList(ModuleFieldListModel excelHeaderModel)
        {
            ModuleFieldListModel excelHeader = new ModuleFieldListModel();
            try
            {
                List<int> categoryid = new List<int>();
                List<string> fieldName = new List<string>();

                if (excelHeaderModel.Module.ToLower()=="student")
                {
                    categoryid = new List<int> { 6, 7, 8, 9 };
                    
                    fieldName = new List<string> 
                    {
                        "socialSecurityNumber",
                        "otherGovtIssuedNumber",
                        "portalAccess",
                        "studentPhoto",
                        "calenderId",
                        "rollingOption",
                        "eligibility504",
                        "economicDisadvantage",
                        "freeLunchEligibility" ,
                        "specialEducationIndicator",
                        "lepIndicator",
                        "schoolId",
                        "enrollmentCode",
                        "exitDate",
                        "exitCode",
                        "busNo",
                        "schoolBusPickUp",
                        "schoolBusDropOff",
                        "mailingAddressSameToHome",
                        "mailingAddressLineOne",
                        "mailingAddressLineTwo",
                        "mailingAddressCountry",
                        "mailingAddressState",
                        "mailingAddressCity",
                        "mailingAddressZip",
                        "twitter",
                        "facebook",
                        "instagram",
                        "youtube",
                        "linkedin"
                    };
                }
                if (excelHeaderModel.Module.ToLower() == "staff")
                {
                    categoryid = new List<int> { 15 };
                    
                    fieldName = new List<string>
                    {
                        "portalAccess",
                        "staffPhoto",
                        "schoolAttachedName",
                        "endDate",
                        "twitter",
                        "facebook",
                        "instagram",
                        "youtube",
                        "linkedin",
                        "mailingAddressSameToHome",
                        "mailingAddressLineOne",
                        "mailingAddressLineTwo",
                        "mailingAddressCountry",
                        "mailingAddressState",
                        "mailingAddressCity",
                        "mailingAddressZip",
                        "busNo",
                        "busPickup",
                        "busDropoff"
                    };
                }

                var customFieldTitleList = this.context?.CustomFields.Where(c => c.SchoolId == excelHeaderModel.SchoolId && c.TenantId == excelHeaderModel.TenantId && ((categoryid.Count == 0 || (!categoryid.Contains(c.CategoryId))) &&((fieldName.Count == 0 || (!fieldName.Contains(c.FieldName)))) && c.Module.ToLower() == excelHeaderModel.Module.ToLower())).OrderBy(x => x.CategoryId).ThenBy(b=>b.SortOrder).ToArray();

                if (customFieldTitleList.Length > 0)
                {
                    excelHeader.CustomfieldTitle = customFieldTitleList;
                    excelHeader.SchoolId = excelHeaderModel.SchoolId;
                    excelHeader.TenantId = excelHeaderModel.TenantId;
                    excelHeader.Module = excelHeaderModel.Module;
                    excelHeader._tenantName = excelHeaderModel._tenantName;
                    excelHeader._token = excelHeaderModel._token;
                    excelHeader._userName = excelHeaderModel._userName;
                    excelHeader._failure = false;
                }
            }
            catch (Exception es)
            {
                excelHeader._failure = true;
                excelHeader._message = es.Message;
            }
            return excelHeader;
        }

        /// <summary>
        /// Add/Update School Preference
        /// </summary>
        /// <param name="schoolPreferenceAddViewModel"></param>
        /// <returns></returns>
        public SchoolPreferenceAddViewModel AddUpdateSchoolPreference(SchoolPreferenceAddViewModel schoolPreferenceAddViewModel)
        {
            try
            {
                if (schoolPreferenceAddViewModel.schoolPreference.SchoolPreferenceId > 0)
                {
                    var schoolPreferenceUpdate = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == schoolPreferenceAddViewModel.schoolPreference.TenantId && x.SchoolId == schoolPreferenceAddViewModel.schoolPreference.SchoolId && x.SchoolPreferenceId == schoolPreferenceAddViewModel.schoolPreference.SchoolPreferenceId);

                    if (schoolPreferenceUpdate != null)
                    {
                        schoolPreferenceAddViewModel.schoolPreference.UpdatedOn = DateTime.UtcNow;
                        schoolPreferenceAddViewModel.schoolPreference.CreatedOn = schoolPreferenceUpdate.CreatedOn;
                        schoolPreferenceAddViewModel.schoolPreference.CreatedBy = schoolPreferenceUpdate.CreatedBy;
                        this.context.Entry(schoolPreferenceUpdate).CurrentValues.SetValues(schoolPreferenceAddViewModel.schoolPreference);
                        this.context?.SaveChanges();
                        schoolPreferenceAddViewModel._failure = false;
                        schoolPreferenceAddViewModel._message = "School Preference Updated Successfully";
                    }
                    else
                    {
                        schoolPreferenceAddViewModel._failure = true;
                        schoolPreferenceAddViewModel._message = NORECORDFOUND;
                    }
                }
                else
                {
                    long? schoolPreferenceId = Utility.GetMaxLongPK(this.context, new Func<SchoolPreference, long>(x => x.SchoolPreferenceId));
                    schoolPreferenceAddViewModel.schoolPreference.SchoolPreferenceId = (long)schoolPreferenceId;
                    schoolPreferenceAddViewModel.schoolPreference.CreatedOn = DateTime.UtcNow;
                    this.context.SchoolPreference.Add(schoolPreferenceAddViewModel.schoolPreference);
                    this.context?.SaveChanges();
                    schoolPreferenceAddViewModel._failure = false;
                    schoolPreferenceAddViewModel._message = "School Preference Added Successfully";
                }
            }
            catch (Exception es)
            {
                schoolPreferenceAddViewModel._failure = true;
                schoolPreferenceAddViewModel._message = es.Message;
            }
            return schoolPreferenceAddViewModel;
        }

        /// <summary>
        /// Get School Preference
        /// </summary>
        /// <param name="schoolPreferenceAddViewModel"></param>
        /// <returns></returns>
        public SchoolPreferenceAddViewModel ViewSchoolPreference(SchoolPreferenceAddViewModel schoolPreferenceAddViewModel)
        {
            SchoolPreferenceAddViewModel schoolPreferenceViewModel = new SchoolPreferenceAddViewModel();
            try
            {
                var schoolPreferenceView = this.context?.SchoolPreference.FirstOrDefault(x => x.TenantId == schoolPreferenceAddViewModel.schoolPreference.TenantId && x.SchoolId == schoolPreferenceAddViewModel.schoolPreference.SchoolId);

                if (schoolPreferenceView != null)
                {
                    schoolPreferenceViewModel.schoolPreference = schoolPreferenceView;
                    schoolPreferenceViewModel._tenantName = schoolPreferenceAddViewModel._tenantName;
                    schoolPreferenceViewModel._token = schoolPreferenceAddViewModel._token;
                    schoolPreferenceViewModel._userName = schoolPreferenceAddViewModel._userName;
                    schoolPreferenceViewModel._failure = false;
                }
                else
                {
                    schoolPreferenceViewModel._failure = true;
                    schoolPreferenceViewModel._message = NORECORDFOUND;
                    schoolPreferenceViewModel._tenantName = schoolPreferenceAddViewModel._tenantName;
                    schoolPreferenceViewModel._token = schoolPreferenceAddViewModel._token;
                    schoolPreferenceViewModel._userName = schoolPreferenceAddViewModel._userName;
                }
            }
            catch (Exception es)
            {
                schoolPreferenceViewModel._failure = true;
                schoolPreferenceViewModel._message = es.Message;
                schoolPreferenceViewModel._tenantName = schoolPreferenceAddViewModel._tenantName;
                schoolPreferenceViewModel._token = schoolPreferenceAddViewModel._token;
                schoolPreferenceViewModel._userName = schoolPreferenceAddViewModel._userName;
            }
            return schoolPreferenceViewModel;
        }

        /// <summary>
        /// Get Missing Attendance Count For Dashboard View
        /// </summary>
        /// <param name="scheduledCourseSectionViewModel"></param>
        /// <returns></returns>
        public ScheduledCourseSectionViewModel GetMissingAttendanceCountForDashboardView(ScheduledCourseSectionViewModel scheduledCourseSectionViewModel)
        {
            List<AllCourseSectionView> AllCourseSectionViewList = new List<AllCourseSectionView>();
            ScheduledCourseSectionViewModel scheduledCourseSectionView = new ScheduledCourseSectionViewModel();
            try
            {
                var scheduledCourseSectionData = this.context?.StaffCoursesectionSchedule.Include(x => x.CourseSection).Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId && x.IsDropped != true).ToList();

                if (scheduledCourseSectionData.Count>0)
                {
                    int count = 0;
                    List<int> ID = new List<int>();
                    
                    AllCourseSectionViewList = this.context?.AllCourseSectionView.Where(x => x.TenantId == scheduledCourseSectionViewModel.TenantId && x.SchoolId == scheduledCourseSectionViewModel.SchoolId).ToList();
                    
                    foreach (var scheduledCourseSection in scheduledCourseSectionData)
                    {
                        if (!ID.Contains(scheduledCourseSection.CourseSectionId))
                        {
                            var AllCourseSectionViewData = AllCourseSectionViewList.Where(v => v.SchoolId == scheduledCourseSectionViewModel.SchoolId && v.TenantId == scheduledCourseSectionViewModel.TenantId && v.CourseId == scheduledCourseSection.CourseId && v.CourseSectionId == scheduledCourseSection.CourseSectionId && (v.AttendanceTaken == true || v.TakeAttendanceCalendar == true || v.TakeAttendanceVariable == true || v.TakeAttendanceBlock == true)).ToList();

                            if (AllCourseSectionViewData.Count > 0)
                            {
                                if (scheduledCourseSection.CourseSection.ScheduleType == "Block Schedule (4)")
                                {
                                    foreach (var allCourseSectionVew in AllCourseSectionViewData)
                                    {
                                        var bellScheduleList = this.context?.BellSchedule.Where(v => v.SchoolId == scheduledCourseSection.SchoolId && v.TenantId == scheduledCourseSection.TenantId && v.BlockId == allCourseSectionVew.BlockId && v.BellScheduleDate >= scheduledCourseSection.DurationStartDate && v.BellScheduleDate <= scheduledCourseSection.DurationEndDate && v.BellScheduleDate <= DateTime.Today.Date).ToList();

                                        if (bellScheduleList.Count > 0)
                                        {
                                            foreach (var bellSchedule in bellScheduleList)
                                            {
                                                var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId && b.AttendanceDate.Date == bellSchedule.BellScheduleDate && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.CourseId == scheduledCourseSection.CourseId && b.PeriodId == allCourseSectionVew.BlockPeriodId);

                                                if (staffAttendanceData.Count() == 0)
                                                {
                                                    count = count + 1;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (scheduledCourseSection.CourseSection.ScheduleType == "Calendar Schedule (3)")
                                {
                                    var calenderScheduleList = AllCourseSectionViewData.Where(c => c.CalDate.Value.Date <= DateTime.Today.Date);

                                    if (calenderScheduleList.Count() > 0)
                                    {
                                        foreach (var courseCalenderSchedule in calenderScheduleList)
                                        {
                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId /*&& b.StaffId == staffScheduleData.StaffId*/ && b.AttendanceDate.Date == courseCalenderSchedule.CalDate.Value.Date && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.CourseId == scheduledCourseSection.CourseId && b.PeriodId == courseCalenderSchedule.CalPeriodId);

                                            if (staffAttendanceData.Count() == 0)
                                            {
                                                count = count + 1;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    DateTime start;
                                    DateTime end;
                                    List<DateTime> dateList = new List<DateTime>();
                                    string[] meetingDays = { };

                                    start = (DateTime)scheduledCourseSection.DurationStartDate;
                                    end = (DateTime)scheduledCourseSection.DurationEndDate;


                                    meetingDays = scheduledCourseSection.MeetingDays.ToLower().Split("|");

                                    bool allDays = meetingDays == null || !meetingDays.Any();

                                    dateList = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                                                          .Select(offset => start.AddDays(offset))
                                                          .Where(d => allDays || meetingDays.Contains(d.DayOfWeek.ToString().ToLower()))
                                                          .ToList();

                                    if (dateList.Count > 0)
                                    {
                                        dateList = dateList.Where(s => dateList.Any(secL => s.Date <= DateTime.Today.Date)).ToList();
                                    }

                                    foreach (var date in dateList)
                                    {
                                        if (scheduledCourseSection.CourseSection.ScheduleType == "Fixed Schedule (1)")
                                        {
                                            var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == scheduledCourseSection.CourseId && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.PeriodId == AllCourseSectionViewData.FirstOrDefault().FixedPeriodId);

                                            if (staffAttendanceData.Count() == 0)
                                            {
                                                count = count + 1;
                                            }
                                        }

                                        if (scheduledCourseSection.CourseSection.ScheduleType == "Variable Schedule (2)")
                                        {
                                            var courseVariableScheduleData = AllCourseSectionViewData.Where(e => e.VarDay.ToLower().Contains(date.DayOfWeek.ToString().ToLower()));

                                            if (courseVariableScheduleData != null)
                                            {
                                                foreach (var courseVariableSchedule in courseVariableScheduleData.ToList())
                                                {
                                                    var staffAttendanceData = this.context?.StudentAttendance.Where(b => b.SchoolId == scheduledCourseSection.SchoolId && b.TenantId == scheduledCourseSection.TenantId /*&& b.StaffId == staffCourseSectionData.StaffId*/ && b.AttendanceDate == date && b.CourseId == scheduledCourseSection.CourseId && b.CourseSectionId == scheduledCourseSection.CourseSectionId && b.PeriodId == courseVariableSchedule.VarPeriodId);

                                                    if (staffAttendanceData.Count() == 0)
                                                    {
                                                        count = count + 1;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            ID.Add(scheduledCourseSection.CourseSectionId);
                        }                        
                    }
                    scheduledCourseSectionView.MissingAttendanceCount = count;
                    scheduledCourseSectionView._failure = false;
                    scheduledCourseSectionView.SchoolId = scheduledCourseSectionViewModel.SchoolId;
                    scheduledCourseSectionView.TenantId = scheduledCourseSectionViewModel.TenantId;
                    scheduledCourseSectionView._tenantName = scheduledCourseSectionViewModel._tenantName;
                    scheduledCourseSectionView._token = scheduledCourseSectionViewModel._token;
                }
            }
            catch (Exception es)
            {
                scheduledCourseSectionView._failure = true;
                scheduledCourseSectionView._message = es.Message;
            }
            return scheduledCourseSectionView;
        }

        /// <summary>
        /// Change Password For User
        /// </summary>
        /// <param name="changePasswordViewModel"></param>
        /// <returns></returns>
        public ChangePasswordViewModel ChangePasswordForUser(ChangePasswordViewModel changePasswordViewModel)
        {
            try
            {
                var oldPasswordDecrypted = Utility.Decrypt(changePasswordViewModel.CurrentPasswordHash);
                string oldPasswordHash = Utility.GetHashedPassword(oldPasswordDecrypted);

                var userMasterData = this.context?.UserMaster.FirstOrDefault(x => x.TenantId == changePasswordViewModel.TenantId && x.UserId == changePasswordViewModel.UserId && x.EmailAddress == changePasswordViewModel.EmailAddress && x.PasswordHash == oldPasswordHash);

                if (userMasterData != null)
                {
                    var newPasswordDecrypted = Utility.Decrypt(changePasswordViewModel.NewPasswordHash);
                    string NewPasswordHash = Utility.GetHashedPassword(newPasswordDecrypted);

                    userMasterData.PasswordHash = NewPasswordHash;
                    userMasterData.UpdatedOn = DateTime.UtcNow;

                    this.context?.SaveChanges();
                    changePasswordViewModel._failure = false;
                    changePasswordViewModel._message = "Password Updated Successfully";
                }
                else
                {
                    changePasswordViewModel._failure = true;
                    changePasswordViewModel._message = "Invalid Email Address or Current Password";
                }
            }
            catch (Exception es)
            {
                changePasswordViewModel._failure = true;
                changePasswordViewModel._message = es.Message;
            }
            return changePasswordViewModel;
        }
    }
}
