import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icPrint from '@iconify/icons-ic/twotone-print';
import icHome from '@iconify/icons-ic/twotone-home';
import icMarkunreadMailbox from '@iconify/icons-ic/twotone-markunread-mailbox';
import icContactPhone from '@iconify/icons-ic/twotone-contact-phone';
import { StudentListByDateRangeModel } from 'src/app/models/student.model';
import { StudentService } from 'src/app/services/student.service';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { LoaderService } from 'src/app/services/loader.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatCheckbox } from '@angular/material/checkbox';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { MatPaginator, MatPaginatorIntl } from '@angular/material/paginator';
import { IdentificationInformation } from 'src/app/enums/identificationInformation.enum';
import { ReportService } from '../../../../services/report.service';
import { GetStudentAdvancedReportModel } from 'src/app/models/report.model';
import { ExcelService } from 'src/app/services/excel.service';
import icFilterList from '@iconify/icons-ic/filter-list';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';


export interface StudentListData {
  studentCheck: boolean;
  studentName: string;
  studentId: string;
  alternateId: string;
  gradeLevel: string;
  section: string;
  phone: string;
}

export const studentListData: StudentListData[] = [
  {studentCheck: false , studentName: 'Arthur Boucher', studentId: 'STD0012', alternateId: 'STD0012', gradeLevel: 'Grade 11', section: 'Section A', phone: '7654328967'},
  {studentCheck: true , studentName: 'Sophia Brown', studentId: 'STD0015', alternateId: 'STD0015', gradeLevel: 'Grade 10', section: 'Section B', phone: '5654328967'},
  {studentCheck: false , studentName: 'Wang Wang', studentId: 'STD0035', alternateId: 'STD0035', gradeLevel: 'Grade 11', section: 'Section A', phone: '7654328967'},
  {studentCheck: false , studentName: 'Clare Garcia', studentId: 'STD0102', alternateId: 'STD0102', gradeLevel: 'Grade 11', section: 'Section A', phone: '9854328967'},
  {studentCheck: false , studentName: 'Amelia Jones', studentId: 'STD0057', alternateId: 'STD0057', gradeLevel: 'Grade 11', section: 'Section B', phone: '9654328967'},
  {studentCheck: true , studentName: 'Audre Keita', studentId: 'STD0061', alternateId: 'STD0061', gradeLevel: 'Grade 9', section: 'Section A', phone: '7654328967'},
  {studentCheck: false , studentName: 'Kwame Kimathi', studentId: 'STD0052', alternateId: 'STD0052', gradeLevel: 'Grade 9', section: 'Section B', phone: '7654328967'},
  {studentCheck: false , studentName: 'James Miller', studentId: 'STD0013', alternateId: 'STD0013', gradeLevel: 'Grade 10', section: 'Section A', phone: '6543212367'},
  {studentCheck: true , studentName: 'Olivia Smith', studentId: 'STD0097', alternateId: 'STD0097', gradeLevel: 'Grade 11', section: 'Section A', phone: '7654328967'},
  {studentCheck: true , studentName: 'Amelia Jones', studentId: 'STD0067', alternateId: 'STD0067', gradeLevel: 'Grade 9', section: 'Section A', phone: '9654328967'},
];

export interface GenerateStudentListData {
  studentName: string;
  studentId: string;
  alternateId: string;
  dateOfBirth: string;
  firstLanguage: string;
  fullAddress: string;
  mailingAddress: string;
}

export const generateStudentListData: GenerateStudentListData[] = [
  {studentName: 'Arthur Boucher', studentId: '12', alternateId: 'STD0012', dateOfBirth: 'Aug 10, 2005', firstLanguage: 'English', fullAddress: '2000 Ruth St NW, Atlanta, GA, 303178', mailingAddress: '2000 Ruth St NW, Atlanta, GA, 303178'},
  {studentName: 'Sophia Brown', studentId: '15', alternateId: 'STD0015', dateOfBirth: 'Aug 10, 2005', firstLanguage: 'English', fullAddress: '1422 Piedmont Rd NE #C3, Atlanta, GA, 30309', mailingAddress: '1422 Piedmont Rd NE #C3, Atlanta, GA, 30309'},
  {studentName: 'Wang Wang', studentId: '35', alternateId: 'STD0035', dateOfBirth: 'Mar 21, 2005', firstLanguage: 'English', fullAddress: '2520 Peachtree Rd #307, Atlanta, GA, 30305', mailingAddress: '2520 Peachtree Rd #307, Atlanta, GA, 30305'},
  {studentName: 'Clare Garcia', studentId: '102', alternateId: 'STD0102', dateOfBirth: 'Dec 01, 2005', firstLanguage: 'English', fullAddress: '938 Mathews St SW, Atlanta, GA, 30309', mailingAddress: '938 Mathews St SW, Atlanta, GA, 30309'},
  {studentName: 'Amelia Jones', studentId: '57', alternateId: 'STD0057', dateOfBirth: 'Aug 10, 2005', firstLanguage: 'English', fullAddress: '2269 Plaster Rd NE, Atlanta, GA, 30345', mailingAddress: '2269 Plaster Rd NE, Atlanta, GA, 30345'},
  {studentName: 'Audre Keita', studentId: '61', alternateId: 'STD0061', dateOfBirth: 'Jun 06, 2005', firstLanguage: 'English', fullAddress: '1287 Bookshire Ln NE, Atlanta, GA, 30319', mailingAddress: '1287 Bookshire Ln NE, Atlanta, GA, 30319'},
  {studentName: 'Kwame Kimathi', studentId: '52', alternateId: 'STD0052', dateOfBirth: 'Aug 10, 2005', firstLanguage: 'English', fullAddress: '2664 Black Forest Trl SW, Atlanta, GA, 30331', mailingAddress: '2664 Black Forest Trl SW, Atlanta, GA, 30331'},
  {studentName: 'James Miller', studentId: '13', alternateId: 'STD0013', dateOfBirth: 'Dec 01, 2005', firstLanguage: 'English', fullAddress: '367 Wilkinson Dr SE, Atlanta, GA, 30317', mailingAddress: '367 Wilkinson Dr SE, Atlanta, GA, 30317'},
  {studentName: 'Olivia Smith', studentId: '97', alternateId: 'STD0097', dateOfBirth: 'Aug 09, 2005', firstLanguage: 'English', fullAddress: '2982 Briarcliff Rd NE, Atlanta GA, 30329', mailingAddress: '2982 Briarcliff Rd NE, Atlanta GA, 30329'},
  {studentName: 'Amelia Jones', studentId: '67', alternateId: 'STD0067', dateOfBirth: 'Aug 10, 2005', firstLanguage: 'English', fullAddress: '226 Plaster Rd NE, Atlanta, GA, 30315', mailingAddress: '226 Plaster Rd NE, Atlanta, GA, 30315'},
];

@Component({
  selector: 'vex-advance-report',
  templateUrl: './advance-report.component.html',
  styleUrls: ['./advance-report.component.scss'],
  animations: [
    fadeInUp400ms,
    stagger40ms,
    fadeInRight400ms
  ]
})
export class AdvanceReportComponent implements OnInit {

  icPrint = icPrint;
  icHome = icHome;
  icMarkunreadMailbox = icMarkunreadMailbox;
  icContactPhone = icContactPhone;
  currentTab: string = 'selectStudents';

  displayedColumns: string[] = ['studentCheck', 'studentName', 'studentId', 'alternateId', 'gradeLevel', 'section', 'phone'];
  studentList = studentListData;
  displayedColumnsReportList: string[] = ['studentName', 'studentId', 'alternateId', 'dateOfBirth', 'firstLanguage', 'fullAddress', 'mailingAddress'];
  generateStudentList: any;
  getAllStudent: StudentListByDateRangeModel = new StudentListByDateRangeModel();
  totalCount: number = 0;
  pageNumber: number;
  pageSize: number;
  studentModelList: MatTableDataSource<any>;
  listOfStudents = [];
  selectedStudents = [];
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  searchCtrl = new FormControl();
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('selectedStudentPaginator') selectedStudentPaginator: MatPaginator;

  showAdvanceSearchPanel: boolean = false;
  loading: boolean;
  columns = [
    { label: '', property: 'studentCheck', type: 'text', visible: true },
    { label: 'Name', property: 'studentName', type: 'text', visible: true },
    { label: 'Student ID', property: 'studentId', type: 'text', visible: true },
    { label: 'Alternate ID', property: 'alternateId', type: 'text', visible: true },
    { label: 'Grade Level', property: 'gradeLevel', type: 'text', visible: true },
    { label: 'Section', property: 'section', type: 'text', visible: true },
    { label: 'Telephone', property: 'phone', type: 'text', visible: true },
    { label: 'School Name', property: 'schoolName', type: 'text', visible: false },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  searchCount;
  searchValue;
  toggleValues;
  selectedFieldsArray = [];
  identificationInformation = IdentificationInformation;
  getStudentAdvancedReportModel: GetStudentAdvancedReportModel = new GetStudentAdvancedReportModel();
  pageSizeForReport: number;
  selectedStudentListForTable: MatTableDataSource<any>;
  icFilterList = icFilterList;


  fieldsDetailsArray = {
    identificationInformation:  [
    { label: 'fullName', property: 'fullName', checked: false },
    { label: 'salutation', property: 'salutation', checked: false },
    { label: 'firstGivenName', property: 'firstGivenName', checked: false },
    { label: 'middleName', property: 'middleName', checked: false },
    { label: 'lastFamilyName', property: 'lastFamilyName', checked: false },
    { label: 'suffix', property: 'suffix', checked: false },
    { label: 'preferredCommonName', property: 'preferredCommonName', checked: false },
    { label: 'studentId', property: 'studentId', checked: false },
    { label: 'alternateId', property: 'alternateId', checked: false },
    { label: 'districtId', property: 'districtId', checked: false },
    { label: 'stateId', property: 'stateId', checked: false },
    { label: 'rollNumber', property: 'rollNumber', checked: false },
    { label: 'socialSecurityNumber', property: 'socialSecurityNumber', checked: false },
    { label: 'otherGovtIssuedNumber', property: 'otherGovtIssuedNumber', checked: false },
    { label: 'admissionNumber', property: 'admissionNumber', checked: false },
    ],
    demographicInformation: [
      { label: 'dateOfBirth', property: 'dateOfBirth', checked: false },
      { label: 'gender', property: 'gender', checked: false },
      { label: 'firstGivenName', property: 'firstGivenName', checked: false },
      { label: 'race', property: 'race', checked: false },
      { label: 'ethnicity', property: 'ethnicity', checked: false },
      { label: 'maritalStatus', property: 'maritalStatus', checked: false },
      { label: 'countryOfBirth', property: 'countryOfBirth', checked: false },
      { label: 'nationality', property: 'nationality', checked: false },
      { label: 'firstLanguage', property: 'firstLanguage', checked: false },
      { label: 'secondLanguage', property: 'secondLanguage', checked: false },
      { label: 'thirdLanguage', property: 'thirdLanguage', checked: false },
   ],
   addressContact : [
    { label: 'studentsFullHomeAddress', property: 'studentsFullHomeAddress', checked: false },
    { label: 'homeAddressLineOne', property: 'homeAddressLineOne', checked: false },
    { label: 'homeAddressLineTwo', property: 'homeAddressLineTwo', checked: false },
    { label: 'homeAddressCountry', property: 'homeAddressCountry', checked: false },

    { label: 'stateProvinceLocality', property: 'homeAddressState', checked: false },

    { label: 'homeAddressCity', property: 'homeAddressCity', checked: false },
    { label: 'homeAddressZip', property: 'homeAddressZip', checked: false },
    { label: 'busNo', property: 'busNo', checked: false },
    { label: 'busPickup', property: 'busPickup', checked: false },
    { label: 'busDropoff', property: 'busDropoff', checked: false },
   ],
   studentMailingAddress: [
    { label: 'studentsFullMailingAddress', property: 'studentsFullMailingAddress', checked: false },
    { label: 'mailingAddressLineOne', property: 'mailingAddressLineOne', checked: false },
    { label: 'mailingAddressLineTwo', property: 'mailingAddressLineTwo', checked: false },
    { label: 'mailingAddressCountry', property: 'mailingAddressCountry', checked: false },
    { label: 'stateProvinceLocality', property: 'mailingAddressState', checked: false },
    { label: 'mailingAddressCity', property: 'mailingAddressCity', checked: false },
    { label: 'mailingAddressZip', property: 'mailingAddressZip', checked: false },
   ],
   personalContactInformation: [
    { label: 'homePhone', property: 'homePhone', checked: false },
    { label: 'mobilePhone', property: 'mobilePhone', checked: false },
    { label: 'personalEmail', property: 'personalEmail', checked: false },
    { label: 'schoolEmail', property: 'schoolEmail', checked: false },  
   ]
  }
  
  constructor(
    public translateService: TranslateService,
    private studentService: StudentService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private defaultValuesService: DefaultValuesService,
    private loaderService: LoaderService,
    private reportService: ReportService,
    private excelService: ExcelService,
    private paginatorObj: MatPaginatorIntl,
    ) { 
    paginatorObj.itemsPerPageLabel = translateService.instant('itemsPerPage');
      this.selectedStudentListForTable = new MatTableDataSource([]);

      this.loaderService.isLoading.subscribe((val) => {
        this.loading = val;
      });  
      
    this.pageSizeForReport = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;      
  }

  ngOnInit(): void {
    this.getAllStudent.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.getAllStudentList();
    this.searchCtrl = new FormControl();
  }

  ngAfterViewInit(): void {
    this.selectedStudentListForTable.paginator = this.selectedStudentPaginator;

    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ];
        Object.assign(this.getAllStudent, { filterParams: filterParams });
        this.getAllStudent.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
      else {
        Object.assign(this.getAllStudent, { filterParams: null });
        this.getAllStudent.pageNumber = this.paginator.pageIndex + 1;
        this.getAllStudent.pageSize = this.pageSize;
        this.getAllStudentList();
      }
    });
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  get visibleColumnsForSelectedStudent() {
    return this.selectedFieldsArray.filter(column => column.visible).map(column => column.property);
  }

  getAllStudentList() {
    if (this.getAllStudent.sortingModel?.sortColumn === "") {
      this.getAllStudent.sortingModel = null;
    }
    this.studentService.getAllStudentListByDateRange(this.getAllStudent).subscribe(data => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        if (data.studentListViews === null) {
          this.totalCount = null;
          this.studentModelList = new MatTableDataSource([]);
          this.snackbar.open(data._message, '', {
            duration: 10000
          });
        } else {
          this.studentModelList = new MatTableDataSource([]);
          this.totalCount = null;
        }
      } else {
        this.totalCount = data.totalCount;
        this.pageNumber = data.pageNumber;
        this.pageSize = data._pageSize;
        data.studentListViews.forEach((student) => {
          student.checked = false;
        });
        this.listOfStudents = data.studentListViews.map((item) => {
          this.selectedStudents.map((selectedUser) => {
            if (item.studentId == selectedUser.studentId) {
              item.checked = true;
              return item;
            }
          });
          return item;
        });

        this.masterCheckBox.checked = this.listOfStudents.every((item) => {
          return item.checked;
        })
        this.studentModelList = new MatTableDataSource(data.studentListViews);
        this.getAllStudent = new StudentListByDateRangeModel();
      }
    });
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStudents) {
      for (let selectedUser of this.selectedStudents) {
        if (user.studentId === selectedUser.studentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }
  }

  getSearchResult(res) {
    this.getAllStudent = new StudentListByDateRangeModel();
    if (res?.totalCount) {
      this.searchCount = res.totalCount;
      this.totalCount = res.totalCount;
    }
    else {
      this.searchCount = 0;
      this.totalCount = 0;
    }
    this.pageNumber = res.pageNumber;
    this.pageSize = res.pageSize;
    if (res && res.studentListViews) {
      res?.studentListViews?.forEach((student) => {
        student.checked = false;
      });
      this.listOfStudents = res.studentListViews.map((item) => {
        this.selectedStudents.map((selectedUser) => {
          if (item.studentId == selectedUser.studentId) {
            item.checked = true;
            return item;
          }
        });
        return item;
      });

      this.masterCheckBox.checked = this.listOfStudents.every((item) => {
        return item.checked;
      })
    }
    this.studentModelList = new MatTableDataSource(res?.studentListViews);
    this.getAllStudent = new StudentListByDateRangeModel();
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStudents === true) {
      this.columns[8].visible = true;
    } else if (event.inactiveStudents === false) {
      this.columns[8].visible = false;
    }
    if (event.searchAllSchool === true) {
      this.columns[7].visible = true;
    } else if (event.searchAllSchool === false) {
      this.columns[7].visible = false;
    }
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  getPageEvent(event) {
    if (this.searchCtrl.value != null && this.searchCtrl.value != "") {
      let filterParams = [
        {
          columnName: null,
          filterValue: this.searchCtrl.value,
          filterOption: 3
        }
      ]
      Object.assign(this.getAllStudent, { filterParams: filterParams });
    }
    this.getAllStudent.pageNumber = event.pageIndex + 1;
    this.getAllStudent.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.getAllStudentList();
  }

  setAll(event) {
    this.listOfStudents.forEach(user => { user.checked = event; });
    this.studentModelList = new MatTableDataSource(this.listOfStudents);
    this.decideCheckUncheck();
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfStudents) {
      if (item.studentId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.studentModelList = new MatTableDataSource(this.listOfStudents);
    this.masterCheckBox.checked = this.listOfStudents.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStudents.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId == selectedUser.studentId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStudents.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStudents) {
          if (item.studentId == selectedUser.studentId) {
            this.selectedStudents = this.selectedStudents.filter((user) => user.studentId != item.studentId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStudents = this.selectedStudents.filter((item) => item.checked);
  }

  changeTab(status) {
    if(status === 'selectFields' && this.selectedStudentListForTable.data.length > 0 && this.selectedStudents.length > 0) {
      this.currentTab = status;
    } else if(status === 'generateReport' && this.selectedFieldsArray.length > 0) {
      this.currentTab = status;
    } else if(status === 'selectStudents') {
      this.currentTab = status;
    }
  }

  changeFields(event, type, masterCheck?, key?) {

    if(masterCheck) {
      if(this.fieldsDetailsArray[key][0].checked) {
        this.fieldsDetailsArray[key].map((item, index)=>{
          if(index > 0) {
            item.checked = true;
            if(this.selectedFieldsArray.findIndex(x=> x.property === item.property) === -1) {
              this.selectedFieldsArray.push({property: item.property, visible: this.selectedFieldsArray.length < 7 ? true : false});
            }
          }
        })
      } else {
        this.fieldsDetailsArray[key].map((item, index)=>{
          if(index > 0) {
            item.checked = false;
            const index = this.selectedFieldsArray.findIndex(x=> x.property === item.property);
          this.selectedFieldsArray.splice(index, 1);
          }
        })
      }
    } else {
    if(event.checked) {
      if(key) {
      const [, ...dataWithoutfirstIndex] = this.fieldsDetailsArray[key];
        if(dataWithoutfirstIndex.every(x=> x.checked)) {
          this.fieldsDetailsArray[key][0].checked = true;
          // this.selectedFieldsArray.push(this.fieldsDetailsArray[key][0].property);
        }
        this.selectedFieldsArray.push({property: type, visible: this.selectedFieldsArray.length < 7 ? true : false});

      } else {
        this.selectedFieldsArray.push({property: type, visible: this.selectedFieldsArray.length < 7 ? true : false});
      }
    } else {
      if(key) {
          this.fieldsDetailsArray[key][0].checked = false;
        const index = this.selectedFieldsArray.findIndex(x=> x.property === type);
        this.selectedFieldsArray.splice(index, 1);
      } else {
        const index = this.selectedFieldsArray.findIndex(x=> x.property === type);
        this.selectedFieldsArray.splice(index, 1);
      }
    }
    }


  }

  generateStudentAdvanceReport() {
    if (this.selectedStudents.length === 0) {
      this.snackbar.open('Please select any student to generate report.', '', {
        duration: 2000
      });
      return;
    }

    this.getStudentAdvancedReportModel.studentGuids = this.selectedStudents.map((item) => {
      return item.studentGuid
    })

    this.reportService.getStudentAdvancedReport(this.getStudentAdvancedReportModel).subscribe((data: any) => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
      } else {
        this.generateStudentList = data.schoolListForReport;        
        if(this.generateStudentList) {
          this.generateStudentList[0].studentListForReport.map((item: any)=>{
            const middleName = item.studentMaster.middleName ? ' ' + item.studentMaster?.middleName + ' ' : ' '
            item.studentMaster.fullName = item.studentMaster.firstGivenName + middleName + item.studentMaster?.lastFamilyName;
          item.fieldsCategoryList[0].customFields.map((subItem)=>{
            item.studentMaster[subItem.title] = subItem.customFieldsValue?.length > 0 ? subItem.customFieldsValue[0].customFieldValue : subItem.defaultSelection;
          })
          })
        }
        this.selectedStudentListForTable = new MatTableDataSource(this.generateStudentList[0]?.studentListForReport);
        this.selectedStudentListForTable.paginator = this.selectedStudentPaginator;
        this.currentTab = 'selectFields';
      }
    });
  }

  generateExcel() {    
    if (this.generateStudentList[0].studentListForReport.length > 0) {
      let object = {};
      let studentList = [];
      this.generateStudentList[0].studentListForReport.map((item) => {        
        this.selectedFieldsArray.map((fields)=>{          
          Object.assign(object, {[this.defaultValuesService.translateKey(fields.property)]: item.studentMaster[fields.property]});
        })        
        studentList.push(JSON.parse(JSON.stringify(object)));
          })          
      this.excelService.exportAsExcelFile(studentList, 'Student_Advance Report');
    }
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
    // column.visible = !column.visible;
  }

}
