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

import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { GetAllGradeLevelsModel } from '../../../../models/grade-level.model';
import { GradeLevelService } from '../../../../services/grade-level.service';
import { CourseManagerService } from '../../../../services/course-manager.service';
import { GetAllSubjectModel } from '../../../../models/course-manager.model';
import { GetAllStaffModel, StaffListModel, StaffMasterModel, StaffMasterSearchModel } from '../../../../models/staff.model';
import { MembershipService } from '../../../../services/membership.service';
import { GetAllMembersList } from '../../../../models/membership.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { StaffService } from '../../../../services/staff.service';
import { MatTableDataSource } from '@angular/material/table';
import { LoaderService } from '../../../../services/loader.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';
import { LanguageModel } from '../../../../models/language.model';
import { LoginService } from '../../../../services/login.service';
import { MatCheckbox } from '@angular/material/checkbox';
import { MatPaginator } from '@angular/material/paginator';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-add-teacher',
  templateUrl: './add-teacher.component.html',
  styleUrls: ['./add-teacher.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class AddTeacherComponent implements OnInit {
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;

  icClose = icClose;
  displayedColumns: string[] = ['staffSelected', 'staffName', 'staffId', 'primaryGrade', 'primarySubject', 'homeroomTeacher'];
  getAllGradeLevelsModel:GetAllGradeLevelsModel= new GetAllGradeLevelsModel();
  getAllSubjectModel: GetAllSubjectModel = new GetAllSubjectModel();
  staffMasterSearchModel: StaffMasterSearchModel = new StaffMasterSearchModel();
  getAllMembersList: GetAllMembersList = new GetAllMembersList();
  filterParams=[]
  getAllStaff: GetAllStaffModel = new GetAllStaffModel();
  totalCount=0; pageNumber; pageSize;
  staffList: MatTableDataSource<any>;
  staffFullName:string;
  loading:boolean=false;
  destroySubject$: Subject<void> = new Subject();
  isSearchRecordAvailable=false;
  languages: LanguageModel = new LanguageModel();
  @ViewChild('masterCheckBox') masterCheckBox:MatCheckbox;
  listOfStaff=[];
  selectedStaff=[]
  constructor(private dialogRef: MatDialogRef<AddTeacherComponent>, 
    public translateService:TranslateService,
    private gradeLevelService: GradeLevelService,
    private courseManagerService:CourseManagerService,
    private membershipService: MembershipService,
    private snackbar: MatSnackBar,
    private staffService:StaffService,
    private loaderService:LoaderService,
    private loginService:LoginService,
    private commonService: CommonService,
    private defaultValuesService: DefaultValuesService,
    @Inject(MAT_DIALOG_DATA) public data
    ) { 
      this.getAllSubjectModel.subjectList= this.data.subjectList,
      this.getAllMembersList.getAllMemberList= this.data.memberList;
      this.languages.tableLanguage= this.data.languagelist;
      this.getAllGradeLevelsModel.tableGradelevelList= this.data.gradelevelList;
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    
  }

  someComplete():boolean{
    let indetermine=false;
      for(let user of this.listOfStaff){
        for(let selectedUser of this.selectedStaff){
          if(user.staffId==selectedUser.staffId){
            indetermine=true;
          }
        }
      }
      if(indetermine){
        this.masterCheckBox.checked=this.listOfStaff.every((item)=>{
          return item.checked;
        })
        if(this.masterCheckBox.checked){
          return false;
        }else{
          return true;
        }
      }
      
  }
  
  setAll(event){
    this.listOfStaff.forEach(user => {user.checked = event;});
    this.staffList=new MatTableDataSource(this.listOfStaff);
    this.decideCheckUncheck();
  }
  
    onChangeSelection(eventStatus:boolean,id){
      for(let item of this.listOfStaff){
        if(item.staffId==id){
          item.checked=eventStatus;
          break;
        }
      }
      this.staffList=new MatTableDataSource(this.listOfStaff);
      this.masterCheckBox.checked=this.listOfStaff.every((item)=>{
        return item.checked;
      });
    
     this.decideCheckUncheck();
    }
  
  decideCheckUncheck(){
    this.listOfStaff.map((item)=>{
      let isIdIncludesInSelectedList=false;
      if(item.checked){
        for(let selectedUser of this.selectedStaff){
          if(item.staffId==selectedUser.staffId){
            isIdIncludesInSelectedList=true;
            break;
           }
        }
        if(!isIdIncludesInSelectedList){
          this.selectedStaff.push(item);
        }
      }else{
        for(let selectedUser of this.selectedStaff){
          if(item.staffId==selectedUser.staffId){
            this.selectedStaff=this.selectedStaff.filter((user)=>user.staffId!=item.staffId);
            break;
           }
        }
      }
      isIdIncludesInSelectedList=false;
      
    });
    this.selectedStaff=this.selectedStaff.filter((item)=>item.checked);
  }

  submit(){
    this.filterParams=[]
    for (let key in this.staffMasterSearchModel) {

      if (this.staffMasterSearchModel.hasOwnProperty(key))
        if (this.staffMasterSearchModel[key]) {
          this.filterParams.push({ "columnName": key, "filterOption": this.findFilterOption(key), "filterValue": this.staffMasterSearchModel[key] })
        }
    }
    this.callStaffListBasedOnFilterParams();
  }

 
  findFilterOption(keyName):number{
    if(keyName=='otherSubjectTaught' || keyName=='otherGradeLevelTaught'){
      return 3;
    }else{
      return 11
    }
  }

  getPageEvent(event){
   
    this.getAllStaff.pageNumber=event.pageIndex+1;
    this.getAllStaff.pageSize=event.pageSize;
    this.callStaffListBasedOnFilterParams();

  }

  callStaffListBasedOnFilterParams(){
    this.isSearchRecordAvailable= true;
    this.getAllStaff.filterParams = this.filterParams;
    this.getAllStaff.sortingModel = null;
    this.getAllStaff.fullName = this.staffFullName;
    this.getAllStaff.profilePhoto=true;
    this.staffService.getAllStaffList(this.getAllStaff).subscribe(res => {
     if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if(res.staffMaster===null){
            this.staffList = new MatTableDataSource([]);   
            this.snackbar.open( res._message, '', {
              duration: 5000
            });
        } else{
          this.staffList = new MatTableDataSource([]); 
          this.totalCount= res?.totalCount?res.totalCount:0;  
        }
      }else{
        this.totalCount= res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        res.staffMaster=this.findLanguageNameByLanguageId(res.staffMaster);
        this.staffList = new MatTableDataSource(res.staffMaster);
        
        res.staffMaster.forEach(user => {
          user.checked=false
        });
        let response=res.staffMaster.map((item)=>{
          this.selectedStaff.map((selectedUser)=>{
            if(item.staffId==selectedUser.staffId){
              item.checked=true;
              return item;
            }
          });
          return item;
        });
        this.listOfStaff=response;
        this.masterCheckBox.checked=response.every((item)=>{
          return item.checked;
        })

        this.getAllStaff=new GetAllStaffModel();     
      }
    });
  }

  findLanguageNameByLanguageId(staffMaster){
    staffMaster.map((item)=>{
      this.languages?.tableLanguage?.map((language)=>{
        if(language.langId==+item.firstLanguage){
          item.firstLanguageName=language.locale
        }
        if(language.langId==+item.secondLanguage){
          item.secondLanguageName=language.locale
        }
        if(language.langId==+item.thirdLanguage){
          item.thirdLanguageName=language.locale
        }
      }) 
    })
    return staffMaster
  }

 

 
  

  addSelectedTeachers(){
    if(this.selectedStaff.length>0){
      this.dialogRef.close(this.selectedStaff);
    }else{
      this.snackbar.open('Please select at least 1 teacher', '', {
        duration: 2000
      });
    }
  }

  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
