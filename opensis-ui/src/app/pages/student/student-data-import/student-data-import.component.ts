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

import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslateService } from '@ngx-translate/core';
import { DefaultValuesService } from '../../../common/default-values.service';
import { BulkDataImport } from '../../../enums/bulk-data-import.enum';
import { FieldCategoryModuleEnum } from '../../../enums/field-category-module.enum';
import { CountryModel } from '../../../models/country.model';
import { CustomFieldModel } from '../../../models/custom-field.model';
import { CustomFieldsValueModel } from '../../../models/custom-fields-value.model';
import { FieldsCategoryListView, FieldsCategoryModel } from '../../../models/fields-category.model';
import { LanguageModel } from '../../../models/language.model';
import { GetAllSectionModel } from '../../../models/section.model';
import { AfterImportStatus, StudentImportModel, StudentMasterImportModel } from '../../../models/student.model';
import { CommonService } from '../../../services/common.service';
import { CustomFieldService } from '../../../services/custom-field.service';
import { ExcelService } from '../../../services/excel.service';
import { LoginService } from '../../../services/login.service';
import { SectionService } from '../../../services/section.service';
import { StudentService } from '../../../services/student.service';
import { AgeRangeList, BulkDataImportExcelHeader, EducationalStage } from '../../../models/common.model';
import { Permissions } from '../../../models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import * as moment from 'moment';

@Component({
  selector: 'vex-student-data-import',
  templateUrl: './student-data-import.component.html',
  styleUrls: ['./student-data-import.component.scss']
})
export class StudentDataImportComponent implements OnInit {
  bulkDataImport= BulkDataImport;
  currentTab: BulkDataImport = BulkDataImport.UPLOAD;
  fieldCategoryModuleEnum=FieldCategoryModuleEnum;
  acceptedFileTypes="application/vnd.ms-excel,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,.csv"
  files: File[] = [];
  headers = [];
  fieldList:CustomFieldModel[]=[new CustomFieldModel()];
  jsonData=[];
  tempJsonData = [];
  newHeaders=[];
  newCloneHeaders = [];
  url:string;
  permissionStatus=[true,false,false,false];
  importStudentModel:StudentImportModel = new StudentImportModel();
  rejectedStudentList = [];
  toggleRejectedList:boolean=false;
  importDone:boolean=false;
  afterImportStatus: AfterImportStatus = new AfterImportStatus();
  importLoader:boolean = false;
  urlFetchLoader: boolean = false;
  failedImport: boolean = false;
  languages: LanguageModel = new LanguageModel();
  countryModel: CountryModel = new CountryModel();
  sectionList: GetAllSectionModel = new GetAllSectionModel();
  exportExcel: BulkDataImportExcelHeader = new BulkDataImportExcelHeader();
  headerObject: {};
  isNewHeaderFilled= false;
  permissions: Permissions;
  constructor(public translateService:TranslateService,
    private snackbar: MatSnackBar,
    private excelService:ExcelService,
    private pageRolePermissions:PageRolesPermission,
    private studentService:StudentService,
    private defaultValueService:DefaultValuesService,
    private loginService:LoginService,
    private commonService:CommonService,
    private sectionService:SectionService) { 
    //translateService.use('en');
  }

  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/student-settings/student-bulk-data-import')
    this.getAllLanguage();
    this.getAllCountry();
    this.getAllSection();
    this.getExcelHeader();
  }

  getAllLanguage() {
    this.loginService.getAllLanguage(this.languages).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.languages.tableLanguage  = [];
          if(!res.tableLanguage){
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }else{
          this.languages.tableLanguage=res.tableLanguage;  
        }
      }
      else {
        this.languages.tableLanguage  = [];
      }
    })
  }

  generateExcel() {
    this.excelService.exportAsExcelFile([this.headerObject],'student_data_for_import');
  }

  getExcelHeader() {
    this.headerObject = {};
    this.exportExcel.module = 'student';
    this.commonService.getAllFieldList(this.exportExcel).subscribe((res)=>{
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.newHeaders = []
          if (!res.customfieldTitle) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        }
        else {
          res.customfieldTitle.map((item)=>{
            Object.assign(this.headerObject, {[item.title]: null});
            this.fieldList.push(item)
          });
          this.fieldList.shift();
        }
      }
      else {
        this.snackbar.open(sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
    
    });
  }


  onHeaderChange(){
    let isFilled = true;
    for(const item of this.newHeaders){
      if(!item){
        isFilled=false;
        break;
      }
    }
    if(isFilled && this.headers.length===this.newHeaders.length){
      this.isNewHeaderFilled=isFilled;
    }
  }

  getAllCountry() {
      this.commonService.GetAllCountry(this.countryModel).subscribe(res => {
        if (res) {
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.countryModel.tableCountry=[];
            if(!res.tableCountry){
              this.snackbar.open(res._message, '', {
                duration: 10000
              });
            }
          } else {
            this.countryModel.tableCountry=res.tableCountry;
          }
        }
        else {
          this.countryModel.tableCountry=[];
        }
      })
  }

  getAllSection() {
      this.sectionService.GetAllSection(this.sectionList).subscribe(res => {
        if(res){
        if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.sectionList.tableSectionsList=[]
              if(!res.tableSectionsList){
                this.snackbar.open(res._message, '', {
                  duration: 10000
                });
              }
          }
          else {
            this.sectionList.tableSectionsList = res.tableSectionsList;
          }
        }else{
          this.sectionList.tableSectionsList=[]
        }
      });
  }


  onUpload(event) {
    this.resetAllPermissionsAndStorage()
    this.files.push(...event.addedFiles);
    this.onFileChange(event);
  }

  onFileChange(ev) {
    this.url = null;
    const reader = new FileReader();
    const file = ev.addedFiles[0];
    reader.onload = (event) => {
      const fileData = reader.result;
      this.jsonData = this.excelService.sheetToJson(fileData);
      this.tempJsonData = [...this.jsonData];
      this.headers = this.jsonData[0];
      this.checkForSameHeader();
    }
    reader.readAsBinaryString(file);

  }

  fetchData() {
    this.resetAllPermissionsAndStorage();
    this.urlFetchLoader=true;
    this.files = []
    let url = this.url;
    let fileData = null;
    let oReq = new XMLHttpRequest();
    oReq.open("GET", url, true);
    oReq.onerror = (res)=>{
      if(res) {
        this.snackbar.open('Please enter valid XLXS download link.', '', {
          duration: 10000
        });
        this.urlFetchLoader=false;
      }
    }
    oReq.responseType = "arraybuffer";
    oReq.onload = () => {
      let arraybuffer = oReq.response;
      / convert data to binary string /
      let data = new Uint8Array(arraybuffer);
      let arr = new Array();
      for (let i = 0; i != data.length; ++i) arr[i] = String.fromCharCode(data[i]);
      fileData = arr.join("");
      this.urlFetchLoader=false;
      this.jsonData = this.excelService.sheetToJson(fileData);
      this.tempJsonData = [...this.jsonData];
      this.headers = this.jsonData[0];
      this.checkForSameHeader();
      this.uploadFile();
    }
    oReq.send();
  }

  uploadFile() {
    this.currentTab = this.bulkDataImport.MAP;
    this.permissionStatus[this.currentTab] = true;
  }

  setNewHeadersAndMapIt() {
    this.newCloneHeaders = []
    this.newHeaders.map((item) => {
      if (!item.systemField) {
        let title = item.fieldName + '|' + item.categoryId + '|' + item.fieldId + '|' + item.type
        this.newCloneHeaders.push(title);
        return;
      }
      this.newCloneHeaders.push(item.fieldName)

    });
    let json = [];
    json = [...this.tempJsonData]
    json[0] = [...this.newCloneHeaders];
    this.jsonData = this.twoDArrayToObject(json);
    this.currentTab = this.bulkDataImport.PREVIEW;
    this.permissionStatus[this.currentTab] = true;
  }

  returnToMap() {
    this.currentTab = this.bulkDataImport.MAP;
  }

  goToImport() {
    this.importStudentModel.studentAddViewModelList = [];
    this.permissionStatus[this.currentTab] = true;
    this.jsonData.map((item, i) => {
      this.importStudentModel.studentAddViewModelList.push(new StudentMasterImportModel())
      for (var key in item) {
        if (key.split('|')[1]) {
          let categoryIndex = this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList.findIndex((cat) => {
            return cat.categoryId == +key.split('|')[1]
          });
          if (categoryIndex != -1) {
            this.pushIntoExistingFieldCategory(item, key, i, categoryIndex)
          } else {
            this.createANewFieldCategory(item, key, i);
          }
        }
      }
      this.importStudentModel.studentAddViewModelList[i].studentMaster = item;
      this.importStudentModel.studentAddViewModelList[i].countryOfBirthName = item.countryOfBirth;
      this.importStudentModel.studentAddViewModelList[i].nationalityName = item.nationality;
      this.importStudentModel.studentAddViewModelList[i].firstLanguageName = item.firstLanguageId;
      this.importStudentModel.studentAddViewModelList[i].secondLanguageName = item.secondLanguageId;
      this.importStudentModel.studentAddViewModelList[i].thirdLanguageName = item.thirdLanguageId;
      this.importStudentModel.studentAddViewModelList[i].sectionName = item.sectionId;
      this.importStudentModel.studentAddViewModelList[i].currentGradeLevel = item.gradeLevelTitle;
      if (moment(item.enrollmentDate).isValid() && item.enrollmentDate) {
        this.importStudentModel.studentAddViewModelList[i].enrollmentDate = moment(item.enrollmentDate).format('YYYY/MM/DD');
      } else {
        this.importStudentModel.studentAddViewModelList[i].enrollmentDate = null;
      }
      if (moment(item.dob).isValid() && item.dob) {
        this.importStudentModel.studentAddViewModelList[i].dob = moment(item.dob).format('YYYY/MM/DD');
      } else {
        this.importStudentModel.studentAddViewModelList[i].dob = null;
      }
      if (moment(item.estimatedGradDate).isValid() && item.estimatedGradDate) {
        this.importStudentModel.studentAddViewModelList[i].estimatedGradDate = moment(item.estimatedGradDate).format('YYYY/MM/DD');
      } else {
        this.importStudentModel.studentAddViewModelList[i].estimatedGradDate = null;
      }
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.countryOfBirth;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.nationality;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.firstLanguageId;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.secondLanguageId;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.thirdLanguageId;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.sectionId;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.gradeLevelTitle;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.enrollmentDate;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.dob;
      delete this.importStudentModel.studentAddViewModelList[i].studentMaster.estimatedGradDate;
    });

    this.importStudentModel.studentAddViewModelList.map((item) => {
      if(item.fieldsCategoryList){
        item.fieldsCategoryList = item.fieldsCategoryList?.filter(cat => cat.categoryId != 0);
      }else{
        item.fieldsCategoryList=null;
      }
      item.studentMaster = item.studentMaster
      
      Object.keys(item.studentMaster).filter(key => key.includes('|')).forEach(key => delete item.studentMaster[key]);
      Object.keys(item.studentMaster).map((key)=>{
        if(key==='loginEmail'){
          item.loginEmail=item.studentMaster[key]
        }
        if(key==='passwordHash'){
          item.passwordHash=item.studentMaster[key]
        }
      })
    // this.replaceNamesWithId(item);  //Language,Country, Nationality, Section 

    });
    if(!this.importStudentModel.studentAddViewModelList.length){
      this.snackbar.open('Please add some data before importing.', 'Ok', {
        duration: 10000
      });
      return
    }
    this.currentTab = this.bulkDataImport.IMPORT;
    this.importStudentList();
  }

  pushIntoExistingFieldCategory(item, key, i, categoryIndex) {
    let customField: CustomFieldModel = new CustomFieldModel();;
    customField.categoryId = +key.split('|')[1];
    customField.title = key.split('|')[0];
    customField.fieldId=+key.split('|')[2];
    customField.type= key.split('|')[3];
    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[categoryIndex].customFields.push(customField);
    let currentCustomFieldIndex = this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[categoryIndex].customFields.length - 1;

    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[categoryIndex]
      .customFields[currentCustomFieldIndex == -1 ? 0 : currentCustomFieldIndex]

    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[categoryIndex]
      .customFields[currentCustomFieldIndex == -1 ? 0 : currentCustomFieldIndex]
      .customFieldsValue[0] = this.fillCustomFieldValues(item, key);
  }

  createANewFieldCategory(item, key, i) {
    let fieldCategory: FieldsCategoryModel = new FieldsCategoryModel();
    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList.push(fieldCategory)
    let currentCategoryIndex = this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList.length - 1;
    fieldCategory.categoryId = +key.split('|')[1];
    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[currentCategoryIndex] = fieldCategory;

    let customField: CustomFieldModel = new CustomFieldModel();
    customField.categoryId = +key.split('|')[1];
    customField.title = key.split('|')[0];
    customField.fieldId=+key.split('|')[2];
    customField.type= key.split('|')[3];
    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[currentCategoryIndex].customFields.push(customField);
    let currentCustomFieldIndex = this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[currentCategoryIndex].customFields.length - 1;

    this.importStudentModel.studentAddViewModelList[i].fieldsCategoryList[currentCategoryIndex]
      .customFields[currentCustomFieldIndex == -1 ? 0 : currentCustomFieldIndex]
      .customFieldsValue[0] = this.fillCustomFieldValues(item, key);
  }


  fillCustomFieldValues(item, key) {
    let customFieldValue: CustomFieldsValueModel = new CustomFieldsValueModel();
    customFieldValue.categoryId = +key.split('|')[1];
    customFieldValue.customFieldTitle = key.split('|')[0];
    customFieldValue.customFieldValue = item[key];
    customFieldValue.fieldId = +key.split('|')[2];
    customFieldValue.module = this.fieldCategoryModuleEnum.Student;
    customFieldValue.customFieldType = key.split('|')[3];
    customFieldValue.updatedBy=this.defaultValueService.getEmailId()
    return customFieldValue;
  }

  // replaceNamesWithId(item){
  //       if(Object.keys(item.studentMaster).includes('firstLanguageId')){
  //           this.findFirstLanguageNameById(item);
  //       }
  //       if(Object.keys(item.studentMaster).includes('secondLanguageId')){
  //         this.findSecondLanguageNameById(item);
  //       }
  //       if(Object.keys(item.studentMaster).includes('thirdLanguageId')){
  //         this.findThirdLanguageNameById(item);
  //       }
  //       if(Object.keys(item.studentMaster).includes('nationality')){
  //         this.replaceNationalityNameById(item);
  //       }
  //       if(Object.keys(item.studentMaster).includes('countryOfBirth')){
  //         this.replaceCountryNameById(item);
  //       }
  //       if(Object.keys(item.studentMaster).includes('sectionId')){
  //         this.replaceSectionNameById(item);
  //       }
  // }

  // findFirstLanguageNameById(item){
  //   let firstLanguage=item.studentMaster.firstLanguageId;
  //   if(!firstLanguage){
  //     return;
  //   }
  //   let firstLanguageFound = false;
  //   for(let language of this.languages.tableLanguage){
  //     if(language.locale?.toLowerCase()===firstLanguage?.toLowerCase()){
  //       item.studentMaster.firstLanguageId=+language.langId;
  //       firstLanguageFound=true;
  //       break;
  //     }
  //   }
  //   if(!firstLanguageFound){
  //     item.studentMaster.firstLanguageId=null;
  //   }
  // }

  // findSecondLanguageNameById(item){
  //   let secondLanguage=item.studentMaster.secondLanguageId;
  //   if(!secondLanguage){
  //     return;
  //   }
  //   let secondLanguageFound = false;
  //   for(let language of this.languages.tableLanguage){
  //     if(language.locale?.toLowerCase()===secondLanguage?.toLowerCase()){
  //       item.studentMaster.secondLanguageId=+language.langId;
  //       secondLanguageFound=true;
  //       break;
  //     }
  //   }
  //   if(!secondLanguageFound){
  //     item.studentMaster.secondLanguageId=null;
  //   }
  // }

  // findThirdLanguageNameById(item){
  //   let thirdLanguage=item.studentMaster.thirdLanguageId;
  //   if(!thirdLanguage){
  //     return;
  //   }
  //   let thirdLanguageFound = false;
  //   for(let language of this.languages.tableLanguage){
  //     if(language.locale?.toLowerCase()===thirdLanguage?.toLowerCase()){
  //       item.studentMaster.thirdLanguageId=+language.langId;
  //       thirdLanguageFound=true;
  //       break;
  //     }
  //   }
  //   if(!thirdLanguageFound){
  //     item.studentMaster.thirdLanguageId=null;
  //   }
  // }

  // replaceNationalityNameById(item){
  //   let nationality=item.studentMaster.nationality;
  //   if(!nationality){
  //     return;
  //   }
  //   let nationalityFound = false;
  //   for(let country of this.countryModel.tableCountry){
  //     if(country.name?.toLowerCase()===nationality?.toLowerCase()){
  //       item.studentMaster.nationality=+country.id;
  //       nationalityFound=true;
  //       break;
  //     }
  //   }
  //   if(!nationalityFound){
  //     item.studentMaster.nationality=null;
  //   }
  // }

  // replaceCountryNameById(item){
  //   let countryName=item.studentMaster.countryOfBirth;
  //   if(!countryName){
  //     return;
  //   }
  //   let countryFound = false;
  //   for(let country of this.countryModel.tableCountry){
  //     if(country.name?.toLowerCase()===countryName?.toLowerCase()){
  //       item.studentMaster.countryOfBirth=+country.id;
  //       countryFound=true;
  //       break;
  //     }
  //   }
  //   if(!countryFound){
  //     item.studentMaster.countryOfBirth=null;
  //   }
  // }

  // replaceSectionNameById(item){
  //   let sectionName=item.studentMaster.sectionId;
  //   if(!sectionName){
  //     return;
  //   }
  //   let sectionFound = false;
  //   for(let section of this.sectionList.tableSectionsList){
  //     if(section.name?.toLowerCase()===sectionName?.toLowerCase()){
  //       item.studentMaster.sectionId=+section.sectionId;
  //       sectionFound=true;
  //       break;
  //     }
  //   }
  //   if(!sectionFound){
  //     item.studentMaster.sectionId=null;
  //   }
  // }



  changeView(status) {
    if (this.permissionStatus[status]) {
      this.currentTab = status;
    }
  }

  twoDArrayToObject(TwoDArray,headerKeys?) {
    let keys = headerKeys?headerKeys:TwoDArray.shift();
    let arrayOfObjects = []
    for (let i = 0; i < TwoDArray.length; i++) {
      let tempObject = {}
      for (let j = 0; j < TwoDArray[i].length; j++) {
        tempObject[keys[j]] = TwoDArray[i][j];
      }
      arrayOfObjects.push(tempObject);
    }
    return arrayOfObjects;
  }

  checkForSameHeader(){
    for(let field of this.fieldList){
      this.headers.map((excelHeaderName,i)=>{
        if(excelHeaderName==field.title){
          this.newHeaders[i]=field;
        }
      });
      if(this.headers.length==this.newHeaders.length){
        this.isNewHeaderFilled = true;
        break;
      } 
    }
  }
  importStudentList() {
    this.failedImport=false;
    this.importLoader=true;
    this.rejectedStudentList=[];
    this.toggleRejectedList=false;
    this.importDone=false;
    this.studentService.addStudentList(this.importStudentModel).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if (res.studentAddViewModelList===null) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
              let rejectList = [...this.tempJsonData]
              this.rejectedStudentList=rejectList.shift();
          this.calculateImportStatus();

          } else {
            res.conflictIndexNo?.split(',').map((index,i)=>{
                this.rejectedStudentList[i]=[...this.tempJsonData[+index+1]]
            })
            this.calculateImportStatus();
          }
        }
        else {
          this.calculateImportStatus();
        }
      }
      else {
        this.snackbar.open('Student Import failed. ' + sessionStorage.getItem("httpError"), '', {
          duration: 10000
        });
      }
      this.importLoader=false;
      this.importDone=true;
    },
    ((error)=>{
      this.importLoader=false;
      this.failedImport=true;
    })
    )  
  }

  calculateImportStatus(){
    this.afterImportStatus.totalStudentsSent = this.importStudentModel.studentAddViewModelList?.length;
    this.afterImportStatus.totalStudentsImported = this.afterImportStatus.totalStudentsSent - this.rejectedStudentList.length
    if(this.rejectedStudentList.length){
      this.afterImportStatus.totalStudentsImportedInPercentage = ((this.afterImportStatus.totalStudentsSent-this.rejectedStudentList.length)/this.afterImportStatus.totalStudentsSent)*100;
    }else{
    this.afterImportStatus.totalStudentsImportedInPercentage = 100;
    }
  }

  exportRejectionList(){
    let rejectedListInObject = [];
    rejectedListInObject=this.twoDArrayToObject(this.rejectedStudentList,this.headers);
    this.excelService.exportAsExcelFile(rejectedListInObject,'Rejected_Students_List_')
  }

  onRemove(event) {
    this.files.splice(this.files.indexOf(event), 1);
    this.resetAllPermissionsAndStorage()
  }

  resetAllPermissionsAndStorage() {
    this.tempJsonData=[]
    this.rejectedStudentList=[]
    this.headers = [];
    this.newHeaders = [];
    this.isNewHeaderFilled=false;
    this.jsonData = [];
    this.newCloneHeaders = [];
    this.permissionStatus = [true, false, false, false];
  }

}
