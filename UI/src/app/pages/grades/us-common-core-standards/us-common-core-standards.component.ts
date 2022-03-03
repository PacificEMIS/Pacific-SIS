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

import { Component, OnInit, Input ,ViewChild, OnDestroy } from '@angular/core';
import icMoreVert from '@iconify/icons-ic/twotone-more-vert';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icSearch from '@iconify/icons-ic/search';
import icFilterList from '@iconify/icons-ic/filter-list';
import icImport from '@iconify/icons-ic/twotone-unarchive';
import icUpdate from '@iconify/icons-ic/cached';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router} from '@angular/router';
import { fadeInUp400ms } from '../../../../@vex/animations/fade-in-up.animation';
import { stagger40ms } from '../../../../@vex/animations/stagger.animation';
import { TranslateService } from '@ngx-translate/core';
import { ViewDetailsComponent } from './view-details/view-details.component';
import { CryptoService } from 'src/app/services/Crypto.service';
import { RolePermissionListViewModel, RolePermissionViewModel } from 'src/app/models/roll-based-access.model';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { MatTableDataSource } from '@angular/material/table';
import { GetAllSchoolSpecificListModel, GradeStandardSubjectCourseListModel, SchoolSpecificStandarModel , AddUsStandardData} from '../../../models/grades.model';
import { GradesService } from '../../../services/grades.service';
import { CommonService } from 'src/app/services/common.service';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { GradeLevelService } from '../../../services/grade-level.service';
import { GetAllGradeLevelsModel } from '../../../models/grade-level.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MatSort } from '@angular/material/sort';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { debounceTime, distinctUntilChanged, takeUntil } from 'rxjs/operators';
import { ExcelService } from '../../../services/excel.service';
import { LoaderService } from "src/app/services/loader.service";
import { Subject } from "rxjs";

@Component({
  selector: 'vex-us-common-core-standards',
  templateUrl: './us-common-core-standards.component.html',
  styleUrls: ['./us-common-core-standards.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms
  ]
})
export class UsCommonCoreStandardsComponent implements OnInit {

  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort

  columns = [
    { label: 'standardRefNo', property: 'standard_ref_no', type: 'text', visible: true },
    { label: 'subject', property: 'subject', type: 'text', visible: true },
    { label: 'grade', property: 'grade', type: 'text', visible: true },
    { label: 'course', property: 'course', type: 'text', visible: true },
    { label: 'domain', property: 'domain', type: 'text', visible: true },
    { label: 'topic', property: 'topic', type: 'text', visible: true },
    { label: 'standardDetails', property: 'standard_details', type: 'text', visible: false }
  ];

  CommonCoreStandardsModelList;

  icMoreVert = icMoreVert;
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icSearch = icSearch;
  icImport = icImport;
  icFilterList = icFilterList;
  icUpdate = icUpdate;
  selectedOption = '1';
  loading:Boolean;
  editPermission = false;
  deletePermission = false;
  addPermission = false;
  permissionListViewModel: RolePermissionListViewModel = new RolePermissionListViewModel();
  permissionGroup: RolePermissionViewModel = new RolePermissionViewModel();
  permisisons: Permissions
  StudentModelList: MatTableDataSource<any>;
  schoolSpecificStandardsList: GetAllSchoolSpecificListModel = new GetAllSchoolSpecificListModel();
  addUsStandardDataModel: AddUsStandardData = new AddUsStandardData()
  totalCount;
  pageNumber;
  pageSize;
  searchCtrl: FormControl;
  form: FormGroup;
  gradeLevelList: GetAllGradeLevelsModel = new GetAllGradeLevelsModel();
  subjectList: GradeStandardSubjectCourseListModel = new GradeStandardSubjectCourseListModel();
  courseList: GradeStandardSubjectCourseListModel = new GradeStandardSubjectCourseListModel();
  destroySubject$: Subject<void> = new Subject();
  isDisabled:boolean;

  constructor(private router: Router,
              private dialog: MatDialog,
              public translateService:TranslateService,
              private pageRolePermissions: PageRolesPermission,
              private gradesService:GradesService,
              private commonService: CommonService,
              private snackbar: MatSnackBar,
              private fb: FormBuilder,
              private gradeLevelService: GradeLevelService, 
              private defaultValuesService: DefaultValuesService,
              private excelService:ExcelService,
              private loaderService: LoaderService,
              private paginatorObj: MatPaginatorIntl,
    ) {
      paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
    //translateService.use('en');
    this.loaderService.isLoading
    .pipe(takeUntil(this.destroySubject$))
    .subscribe((currentState) => {
      this.loading = currentState;
    });
  }

  ngOnInit(): void {
    this.permisisons = this.pageRolePermissions.checkPageRolePermission('/school/settings/grade-settings/standard-grades-setup') 
    this.searchCtrl = new FormControl();
    this.form = this.fb.group({
      subject:['all',[Validators.required]],
      course:['all',[Validators.required]],
      gradeLevel:['all',[Validators.required]],
    })
    this.getAllSchoolSpecificList();
  }

  ngAfterViewInit() {
    //  Sorting
    this.schoolSpecificStandardsList = new GetAllSchoolSpecificListModel();
    this.sort.sortChange.subscribe((res) => {
      this.schoolSpecificStandardsList.pageNumber = this.pageNumber
      this.schoolSpecificStandardsList.pageSize = this.pageSize;
      this.schoolSpecificStandardsList.sortingModel.sortColumn = res.active;
      if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
        let filterParams = [
          {
            columnName: null,
            filterValue: this.searchCtrl.value,
            filterOption: 3
          }
        ]
        Object.assign(this.schoolSpecificStandardsList, { filterParams: filterParams });
      }
      if (res.direction == "") {
        this.schoolSpecificStandardsList.sortingModel = null;
        this.getAllSchoolSpecificList();
        this.schoolSpecificStandardsList = new GetAllSchoolSpecificListModel();
        this.schoolSpecificStandardsList.sortingModel = null;
      } else {
        this.schoolSpecificStandardsList.sortingModel.sortDirection = res.direction;
        this.getAllSchoolSpecificList();
      }
    });

    //  Searching
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term != '') {
        this.searchWithTerm(term)
      } else {
        this.searchWithoutTerm()
      }
    })
  }

  
  searchWithoutTerm() {
    Object.assign(this.schoolSpecificStandardsList, { filterParams: null });
    this.schoolSpecificStandardsList.pageNumber = this.paginator.pageIndex + 1;
    this.schoolSpecificStandardsList.pageSize = this.pageSize;
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.schoolSpecificStandardsList.sortingModel.sortColumn = this.sort.active;
      this.schoolSpecificStandardsList.sortingModel.sortDirection = this.sort.direction;
    }
    this.getAllSchoolSpecificList();
  }
  
  searchWithTerm(term) {
    let filterParams = [
      {
        columnName: null,
        filterValue: term,
        filterOption: 3
      }
    ]
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.schoolSpecificStandardsList.sortingModel.sortColumn = this.sort.active;
      this.schoolSpecificStandardsList.sortingModel.sortDirection = this.sort.direction;
    }
    Object.assign(this.schoolSpecificStandardsList, { filterParams: filterParams });
    this.schoolSpecificStandardsList.pageNumber = 1;
    this.paginator.pageIndex = 0;
    this.schoolSpecificStandardsList.pageSize = this.pageSize;
    this.getAllSchoolSpecificList();
  }

  exportSchoolSpecificStandardsListToExcel() {
    let schoolSpecificStandardsList=new GetAllSchoolSpecificListModel();
    schoolSpecificStandardsList.pageNumber = 0;
    schoolSpecificStandardsList.pageSize = 0;
    schoolSpecificStandardsList.sortingModel=null;
    schoolSpecificStandardsList.IsSchoolSpecific=false;
    this.gradesService.getAllGradeUsStandardList(schoolSpecificStandardsList).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          if(!res.gradeUsStandardList){
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
      } else {        
        if (res.gradeUsStandardList?.length > 0) {
          let StandardsList = res.gradeUsStandardList?.map((item) => {
            return {
                [this.translateKey('standardRefNo')]: item.standardRefNo,
                [this.translateKey('subject')]: item.subject,
                [this.translateKey('course')]: item.course,
                [this.translateKey('gradeLevel')]: item.gradeLevel,
                [this.translateKey('domain')]: item.domain? item.domain:'-',
                [this.translateKey('topic')]: item.topic,
                [this.translateKey('standardDetails')]: item.standardDetails
            }
          });
          this.excelService.exportAsExcelFile(StandardsList, 'School_Specific_Standards_List_')
        } else {
          this.snackbar.open('No Records Found. Failed to Export School Specific Standards List', '', {
            duration: 5000
          });
        }
      }
    });

  }

  translateKey(key) {
    let trnaslateKey;
    this.translateService.get(key).subscribe((res: string) => {
       trnaslateKey = res;
    });
    return trnaslateKey;
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    column.visible = !column.visible;
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  getAllSchoolSpecificList(){
    if (this.schoolSpecificStandardsList.sortingModel?.sortColumn == "") {
      this.schoolSpecificStandardsList.sortingModel=null;
    }
    this.schoolSpecificStandardsList.IsSchoolSpecific=false;
    this.schoolSpecificStandardsList.isListView=true;
    this.gradesService.getAllGradeUsStandardList(this.schoolSpecificStandardsList).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if(!res.gradeUsStandardList){
          this.snackbar.open( res._message, '', {
            duration: 10000
          });
        }
      } else {
        if(res.gradeUsStandardList.length > 0){
          this.isDisabled = true;
          this.getAllGradeLevel();
        } else {
          this.isDisabled = false;
        }
        this.totalCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        this.CommonCoreStandardsModelList = new MatTableDataSource(res.gradeUsStandardList);
      }
    });
  }

  addUsStandardData(){
  this.addUsStandardDataModel._token=this.defaultValuesService.getToken();
  this.addUsStandardDataModel.createdBy=this.defaultValuesService.getUserGuidId();
    this.gradesService.addUsStandardData(this.addUsStandardDataModel).subscribe(res => {
    if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        if(!res.gradeUsStandardList){
          this.snackbar.open( res._message, '', {
            duration: 10000
          });
        }
      } else {
        this.getAllSchoolSpecificList()
      }
    });
  }

  getAllGradeLevel() {
    this.gradeLevelService.getAllGradeLevels(this.gradeLevelList).subscribe((res) => {
      if (typeof (res) == 'undefined') {
        this.snackbar.open('Grade Level List failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            if (res.tableGradelevelList == null) {
              this.gradeLevelList.tableGradelevelList=[]
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            }
            else{
              this.gradeLevelList.tableGradelevelList=[]
            }

        }
        else {
          this.gradeLevelList=res;
        }
      }
    });
  }

  filterSchoolSpecificStandardsList(){
    this.form.markAllAsTouched();
    if (this.form.valid){
        let filterParams= [
          {
            columnName: "subject",
            filterValue: this.form.value.subject=="all"?null:this.form.value.subject,
            filterOption: 11
          },
          {
            columnName: "course",
            filterValue: this.form.value.course=="all"?null:this.form.value.course,
            filterOption: 11
          },
          {
            columnName: "gradeLevel",
            filterValue: this.form.value.gradeLevel=="all"?null:this.form.value.gradeLevel,
            filterOption: 11
          }
        ]
        Object.assign(this.schoolSpecificStandardsList, { filterParams: filterParams });
        this.getAllSchoolSpecificList();      
    }
  }

  getPageEvent(event) {
    if (this.sort.active != undefined && this.sort.direction != "") {
      this.schoolSpecificStandardsList.sortingModel.sortColumn = this.sort.active;
      this.schoolSpecificStandardsList.sortingModel.sortDirection = this.sort.direction;
    }
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.schoolSpecificStandardsList, { filterParams: filterParams });
    }
    this.schoolSpecificStandardsList.pageNumber = event.pageIndex + 1;
    this.schoolSpecificStandardsList.pageSize = event.pageSize;
    this.getAllSchoolSpecificList();
  }

  openViewDetails(viewDetails) {
    this.dialog.open(ViewDetailsComponent, {
      data: {
        details: viewDetails
      },
      width: '600px'
    });
  }
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
