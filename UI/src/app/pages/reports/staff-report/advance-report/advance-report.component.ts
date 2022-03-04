import { Component, OnInit, ViewChild } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icPrint from '@iconify/icons-ic/twotone-print';
import icHome from '@iconify/icons-ic/twotone-home';
import icMarkunreadMailbox from '@iconify/icons-ic/twotone-markunread-mailbox';
import icContactPhone from '@iconify/icons-ic/twotone-contact-phone';
import icGrade from '@iconify/icons-ic/twotone-grade';
import { GetAllStaffReportModel } from '../../../../models/staff.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { StaffService } from 'src/app/services/staff.service';
import { CommonService } from 'src/app/services/common.service';
import { MatTableDataSource } from '@angular/material/table';
import { MatSnackBar } from '@angular/material/snack-bar';
import moment from 'moment';
import { FormControl } from '@angular/forms';
import { LoaderService } from 'src/app/services/loader.service';
import { MatSort } from '@angular/material/sort';
import { debounceTime, distinctUntilChanged, takeUntil, filter } from 'rxjs/operators';
import { MatPaginator } from '@angular/material/paginator';
import icSearch from '@iconify/icons-ic/search';
import { MatCheckbox } from '@angular/material/checkbox';
import { fadeInUp400ms } from 'src/@vex/animations/fade-in-up.animation';
import { stagger40ms } from 'src/@vex/animations/stagger.animation';
import { fadeInRight400ms } from 'src/@vex/animations/fade-in-right.animation';
import { GetStaffAdvancedReportModel } from 'src/app/models/report.model';
import { ReportService } from 'src/app/services/report.service';
import { SharedFunction } from 'src/app/pages/shared/shared-function';
import icFilterList from '@iconify/icons-ic/filter-list';
import { ExcelService } from 'src/app/services/excel.service';

export interface GenerateListData {
  staffName: string;
  staffId: string;
  gender: string;
  dateOfBirth: string;
  firstLanguage: string;
  mobilePhone: string;
  fullAddress: string;
}

export const generateListData: GenerateListData[] = [
  { staffName: 'Arthur Boucher', staffId: '12', gender: 'Male', dateOfBirth: 'Aug 10, 1985', firstLanguage: 'English', fullAddress: '2000 Ruth St NW, Atlanta, GA, 303178', mobilePhone: '6754328796' },
  { staffName: 'Sophia Brown', staffId: '15', gender: 'Female', dateOfBirth: 'Aug 10, 1980', firstLanguage: 'English', fullAddress: '1422 Piedmont Rd NE #C3, Atlanta, GA, 30309', mobilePhone: '8754328796' },
  { staffName: 'Wang Wang', staffId: '35', gender: 'Male', dateOfBirth: 'Mar 21, 1968', firstLanguage: 'English', fullAddress: '2520 Peachtree Rd #307, Atlanta, GA, 30305', mobilePhone: '9754328796' },
  { staffName: 'Clare Garcia', staffId: '102', gender: 'Female', dateOfBirth: 'Dec 01, 1982', firstLanguage: 'English', fullAddress: '938 Mathews St SW, Atlanta, GA, 30309', mobilePhone: '5754328796' },
  { staffName: 'Amelia Jones', staffId: '57', gender: 'Female', dateOfBirth: 'Aug 10, 1976', firstLanguage: 'English', fullAddress: '2269 Plaster Rd NE, Atlanta, GA, 30345', mobilePhone: '8754328796' },
  { staffName: 'Audre Keita', staffId: '61', gender: 'Male', dateOfBirth: 'Jun 06, 1976', firstLanguage: 'English', fullAddress: '1287 Bookshire Ln NE, Atlanta, GA, 30319', mobilePhone: '9854328796' },
  { staffName: 'Kwame Kimathi', staffId: '52', gender: 'Male', dateOfBirth: 'Aug 10, 1968', firstLanguage: 'English', fullAddress: '2664 Black Forest Trl SW, Atlanta, GA, 30331', mobilePhone: '3554328796' },
  { staffName: 'James Miller', staffId: '13', gender: 'Male', dateOfBirth: 'Dec 01, 1985', firstLanguage: 'English', fullAddress: '367 Wilkinson Dr SE, Atlanta, GA, 30317', mobilePhone: '8254328796' },
  { staffName: 'Olivia Smith', staffId: '97', gender: 'Female', dateOfBirth: 'Aug 09, 1986', firstLanguage: 'English', fullAddress: '2982 Briarcliff Rd NE, Atlanta GA, 30329', mobilePhone: '7554328796' },
  { staffName: 'Amelia Jones', staffId: '67', gender: 'Female', dateOfBirth: 'Aug 10, 1990', firstLanguage: 'English', fullAddress: '226 Plaster Rd NE, Atlanta, GA, 30315', mobilePhone: '9854328796' },
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
  icGrade = icGrade;
  currentTab: string = 'selectStaff';
  displayedColumnsReportList: string[] = ['staffName', 'staffId', 'gender', 'dateOfBirth', 'firstLanguage', 'mobilePhone', 'fullAddress'];
  generateList = generateListData;
  columns = [
    { label: '', property: 'staffCheck', type: 'text', visible: true },
    { label: 'Name', property: 'name', type: 'text', visible: true },
    { label: 'Staff ID', property: 'staffId', type: 'text', visible: true },
    { label: 'Profile', property: 'profile', type: 'text', visible: true },
    { label: 'Job Title', property: 'jobTitle', type: 'text', visible: true },
    { label: 'Status', property: 'status', type: 'text', visible: false }
  ];
  getAllStaffModel: GetAllStaffReportModel = new GetAllStaffReportModel();
  staffList: MatTableDataSource<any>;
  selectedStaffListForTable: MatTableDataSource<any>;
  generateStaffList: any;
  loading: boolean;
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  searchCtrl = new FormControl();
  @ViewChild(MatSort) sort: MatSort
  @ViewChild(MatPaginator, { static: false }) paginator: MatPaginator;
  @ViewChild('selectedStaffPaginator') selectedStaffPaginator: MatPaginator;
  @ViewChild('masterCheckBox') masterCheckBox: MatCheckbox;
  searchCount;
  toggleValues;
  icSearch = icSearch;
  showAdvanceSearchPanel: boolean = false;
  filterJsonParams;
  listOfStaff = [];
  selectedStaff = [];
  searchValue;
  pageSizeForReport: number;
  getStaffAdvancedReportModel: GetStaffAdvancedReportModel = new GetStaffAdvancedReportModel();
  icFilterList = icFilterList;
  fieldsDetailsArray = {
    identificationInformation: [
      { label: 'fullName', property: 'fullName', checked: false },
      { label: 'salutation', property: 'salutation', checked: false },
      { label: 'firstGivenName', property: 'firstGivenName', checked: false },
      { label: 'middleName', property: 'middleName', checked: false },
      { label: 'lastFamilyName', property: 'lastFamilyName', checked: false },
      { label: 'suffix', property: 'suffix', checked: false },
      { label: 'staffId', property: 'staffId', checked: false },
      { label: 'alternateId', property: 'alternateId', checked: false },
      { label: 'districtId', property: 'districtId', checked: false },
      { label: 'stateId', property: 'stateId', checked: false },
      { label: 'preferredCommonName', property: 'preferredCommonName', checked: false },
      { label: 'previousMaidenName', property: 'previousMaidenName', checked: false },
      { label: 'socialSecurityNumber', property: 'socialSecurityNumber', checked: false },
      { label: 'otherGovtIssuedNumber', property: 'otherGovtIssuedNumber', checked: false },
    ],
    demographicInformation: [
      { label: 'gender', property: 'gender', checked: false },
      { label: 'race', property: 'race', checked: false },
      { label: 'ethnicity', property: 'ethnicity', checked: false },
      { label: 'maritalStatus', property: 'maritalStatus', checked: false },
      { label: 'dateOfBirth', property: 'dateOfBirth', checked: false },
      { label: 'countryOfBirth', property: 'countryOfBirth', checked: false },
      { label: 'nationality', property: 'nationality', checked: false },
      { label: 'firstLanguage', property: 'firstLanguage', checked: false },
      { label: 'secondLanguage', property: 'secondLanguage', checked: false },
      { label: 'thirdLanguage', property: 'thirdLanguage', checked: false },
      { label: 'physicalDisability', property: 'physicalDisability', checked: false },
      { label: 'loginemail', property: 'loginemail', checked: false },
    ],
    schoolInfo: [
      { label: 'jobTitle', property: 'jobTitle', checked: false },
      { label: 'joiningDate', property: 'joiningDate', checked: false },
      { label: 'endDate', property: 'endDate', checked: false },
      { label: 'otherGradeLevelTaught', property: 'otherGradeLevelTaught', checked: false },
      { label: 'primarySubjectTaught', property: 'primarySubjectTaught', checked: false },
      { label: 'otherSubjectTaught', property: 'otherSubjectTaught', checked: false },
    ],
    addressAndContactInfo: [
      { label: 'homePhone', property: 'homePhoneOne', checked: false },
      { label: 'mobilePhone', property: 'mobilePhoneOne', checked: false },
      { label: 'officePhone', property: 'officePhone', checked: false },
      { label: 'schoolEmail', property: 'schoolEmail', checked: false },
    ],
    addressContact: [
      { label: 'fullHomeAddress', property: 'fullHomeAddress', checked: false },
      { label: 'homeAddressLineOne', property: 'homeAddressLineOne', checked: false },
      { label: 'homeAddressLineTwo', property: 'homeAddressLineTwo', checked: false },
      { label: 'homeAddressCountry', property: 'homeAddressCountry', checked: false },
      { label: 'stateProvinceLocalityOne', property: 'homeAddressState', checked: false },
      { label: 'homeAddressCity', property: 'homeAddressCity', checked: false },
      { label: 'homeAddressZip', property: 'homeAddressZip', checked: false },
      { label: 'busNo', property: 'busNo', checked: false },
      { label: 'busPickup', property: 'busPickup', checked: false },
      { label: 'busDropoff', property: 'busDropoff', checked: false },
    ],
    staffMailingAddress: [
      { label: 'fullMailingAddress', property: 'fullMailingAddress', checked: false },
      { label: 'mailingAddressLineOne', property: 'mailingAddressLineOne', checked: false },
      { label: 'mailingAddressLineTwo', property: 'mailingAddressLineTwo', checked: false },
      { label: 'mailingAddressCountry', property: 'mailingAddressCountry', checked: false },
      { label: 'stateProvinceLocalityTwo', property: 'mailingAddressState', checked: false },
      { label: 'mailingAddressCity', property: 'mailingAddressCity', checked: false },
      { label: 'mailingAddressZip', property: 'mailingAddressZip', checked: false },
    ],
    emergencyContactInformation: [
      { label: 'emergencyContactName', property: 'emergencyContactName', checked: false },
      { label: 'relationshipToStaff', property: 'relationshipToStaff', checked: false },
      { label: 'homePhone', property: 'homePhoneTwo', checked: false },
      { label: 'workPhone', property: 'workPhone', checked: false },
      { label: 'mobilePhone', property: 'mobilePhoneTwo', checked: false },
      { label: 'email', property: 'email', checked: false },
    ]
  }

  selectedFieldsArray = [];

  constructor(public translateService: TranslateService,
    public defaultValuesService: DefaultValuesService,
    private staffService: StaffService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private loaderService: LoaderService,
    private reportService: ReportService,
    private commonFunction: SharedFunction,
    private excelService: ExcelService,
  ) {
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
    this.pageSizeForReport = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;

  }

  ngOnInit(): void {
    this.getAllStaffModel.pageSize = this.defaultValuesService.getPageSize() ? this.defaultValuesService.getPageSize() : 10;
    this.callStaffList()
    this.searchCtrl = new FormControl();
  }

  ngAfterViewInit() {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term.trim().length > 0) {
        let filterParams = [
          {
            columnName: null,
            filterValue: term,
            filterOption: 3
          }
        ];
        Object.assign(this.getAllStaffModel, { filterParams: filterParams });
        this.getAllStaffModel.pageNumber = 1;
        this.paginator.pageIndex = 0;
        this.getAllStaffModel.pageSize = this.pageSize;
        this.callStaffList();
      }
      else {
        Object.assign(this.getAllStaffModel, { filterParams: null });
        this.getAllStaffModel.pageNumber = this.paginator.pageIndex + 1;
        this.getAllStaffModel.pageSize = this.pageSize;
        this.callStaffList();
      }
    });
  }

  changeTab(status) {
    if (status === 'selectFields' && this.selectedStaffListForTable !== undefined && this.selectedStaff.length > 0) {
      this.currentTab = status;
    } else if (status === 'generateReport' && this.selectedFieldsArray.length > 0) {
      this.currentTab = status;
    } else if (status === 'selectStaff') {
      this.currentTab = status;
    }
  }

  callStaffList() {
    if (this.getAllStaffModel.sortingModel?.sortColumn == "") {
      this.getAllStaffModel.sortingModel = null
    }
    this.getAllStaffModel.markingPeriodStartDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodStartDate())
    this.getAllStaffModel.markingPeriodEndDate = this.commonFunction.formatDateSaveWithoutTime(this.defaultValuesService.getMarkingPeriodEndDate())
    this.staffService.getAllStaffListByDateRange(this.getAllStaffModel).subscribe(res => {
      if (res._failure) {
        this.commonService.checkTokenValidOrNot(res._message);
        if (res.staffMaster === null) {
          this.totalCount = null;
          this.staffList = new MatTableDataSource([]);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        } else {
          this.staffList = new MatTableDataSource([]);
          this.totalCount = null;
        }
      } else {
        this.totalCount = res.totalCount;
        this.pageNumber = res.pageNumber;
        this.pageSize = res._pageSize;
        res.staffMaster.forEach((staff) => {
          staff.checked = false;
        });
        this.listOfStaff = res.staffMaster.map(item => {
          this.selectedStaff.map(selectedUser => {
            if (item.staffId == selectedUser.staffId) {
              item.checked = true;
              return item;
            }
          })
          return item;
        })
        this.masterCheckBox.checked = this.listOfStaff.every(item => {
          return item.checked;
        })
        this.staffList = new MatTableDataSource(res.staffMaster);
        this.getAllStaffModel = new GetAllStaffReportModel();
      }
    });
  }

  get visibleColumns() {
    return this.columns.filter(column => column.visible).map(column => column.property);
  }

  get visibleColumnsForSelectedStaff() {
    return this.selectedFieldsArray.filter(column => column.visible).map(column => column.property);
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
      Object.assign(this.getAllStaffModel, { filterParams: filterParams });
    }
    this.getAllStaffModel.pageNumber = event.pageIndex + 1;
    this.getAllStaffModel.pageSize = event.pageSize;
    this.defaultValuesService.setPageSize(event.pageSize);
    this.callStaffList();
  }

  showAdvanceSearch() {
    this.showAdvanceSearchPanel = true;
    this.filterJsonParams = null;
  }

  someComplete(): boolean {
    let indetermine = false;
    for (let user of this.listOfStaff) {
      for (let selectedUser of this.selectedStaff) {
        if (user.studentId === selectedUser.studentId) {
          indetermine = true;
        }
      }
    }
    if (indetermine) {
      this.masterCheckBox.checked = this.listOfStaff.every((item) => {
        return item.checked;
      })
      if (this.masterCheckBox.checked) {
        return false;
      } else {
        return true;
      }
    }
  }

  setAll(event) {
    this.listOfStaff.forEach(user => { user.checked = event; });
    this.staffList = new MatTableDataSource(this.listOfStaff);
    this.decideCheckUncheck();
  }

  decideCheckUncheck() {
    this.listOfStaff.map((item) => {
      let isIdIncludesInSelectedList = false;
      if (item.checked) {
        for (let selectedUser of this.selectedStaff) {
          if (item.staffId == selectedUser.staffId) {
            isIdIncludesInSelectedList = true;
            break;
          }
        }
        if (!isIdIncludesInSelectedList) {
          this.selectedStaff.push(item);
        }
      } else {
        for (let selectedUser of this.selectedStaff) {
          if (item.staffId == selectedUser.staffId) {
            this.selectedStaff = this.selectedStaff.filter((user) => user.staffId != item.staffId);
            break;
          }
        }
      }
      isIdIncludesInSelectedList = false;

    });
    this.selectedStaff = this.selectedStaff.filter((item) => item.checked);
  }

  onChangeSelection(eventStatus: boolean, id) {
    for (let item of this.listOfStaff) {
      if (item.staffId == id) {
        item.checked = eventStatus;
        break;
      }
    }
    this.staffList = new MatTableDataSource(this.listOfStaff);
    this.masterCheckBox.checked = this.listOfStaff.every((item) => {
      return item.checked;
    });

    this.decideCheckUncheck();
  }

  getSearchResult(res) {
    this.getAllStaffModel = new GetAllStaffReportModel();
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
    if (res && res.staffMaster) {
      res?.staffMaster?.forEach((staff) => {
        staff.checked = false;
      });
      this.listOfStaff = res.staffMaster.map((item) => {
        this.selectedStaff.map((selectedUser) => {
          if (item.staffId == selectedUser.staffId) {
            item.checked = true;
            return item;
          }
        });
        return item;
      });

      this.masterCheckBox.checked = this.listOfStaff.every((item) => {
        return item.checked;
      })
    }
    this.staffList = new MatTableDataSource(res?.staffMaster);
    this.getAllStaffModel = new GetAllStaffReportModel();
  }

  getSearchInput(event) {
    this.searchValue = event;
  }

  hideAdvanceSearch(event) {
    this.showAdvanceSearchPanel = false;
  }

  getToggleValues(event) {
    this.toggleValues = event;
    if (event.inactiveStaff === true) {
      this.columns[5].visible = true;
    } else if (event.inactiveStaff === false) {
      this.columns[5].visible = false;
    }
  }

  generateStaffAdvanceReport() {
    if (this.selectedStaff.length === 0) {
      this.snackbar.open('Please select any staff to generate report.', '', {
        duration: 2000
      });
      return;
    }

    this.getStaffAdvancedReportModel.staffIds = this.selectedStaff.map((item) => {
      return item.staffId
    })

    this.reportService.getStaffAdvancedReport(this.getStaffAdvancedReportModel).subscribe((data: any) => {
      if (data._failure) {
        this.commonService.checkTokenValidOrNot(data._message);
        this.snackbar.open(data._message, "", {
          duration: 10000
        });
      } else {
        this.generateStaffList = data.schoolListForReport;
        if (this.generateStaffList) {
          this.generateStaffList[0].staffListForReport.map((item: any) => {
            if(item.staffMaster.joiningDate)
              item.staffMaster.joiningDate=this.commonFunction.formatDateSaveWithoutTime(item.staffMaster.joiningDate);
            if(item.staffMaster.endDate)
              item.staffMaster.endDate=this.commonFunction.formatDateSaveWithoutTime(item.staffMaster.endDate);
            const middleName = item.staffMaster.middleName ? ' ' + item.staffMaster?.middleName + ' ' : ' ';
            item.staffMaster.fullName = item.staffMaster.firstGivenName + middleName + item.staffMaster?.lastFamilyName;
            item.fieldsCategoryList[0].customFields.map((subItem) => {
              item.staffMaster[subItem.title] = subItem.customFieldsValue?.length > 0 ? subItem.customFieldsValue[0].customFieldValue : subItem.defaultSelection;
            })
          })
        }
        this.selectedStaffListForTable = new MatTableDataSource(this.generateStaffList[0]?.staffListForReport);
        this.selectedStaffListForTable.paginator = this.selectedStaffPaginator;
        this.currentTab = 'selectFields';
      }
    });
  }

  generateExcel() {
    if (this.generateStaffList[0].staffListForReport.length > 0) {
      let object = {};
      let staffList = [];
      this.generateStaffList[0].staffListForReport.map((item) => {
        this.selectedFieldsArray.map((fields) => {
          Object.assign(object, { [this.defaultValuesService.translateKey(fields.property)]: item.staffMaster[fields.property] ? item.staffMaster[fields.property] : '-'});
        })
        staffList.push(JSON.parse(JSON.stringify(object)));
      })
      this.excelService.exportAsExcelFile(staffList, 'Staff_Advance Report');
    }
  }

  changeFields(event, type, masterCheck?, key?) {
    if (masterCheck) {
      if (this.fieldsDetailsArray[key][0].checked) {
        this.fieldsDetailsArray[key].map((item, index) => {
          if (index > 0) {
            item.checked = true;
            if (this.selectedFieldsArray.findIndex(x => x.property === item.property) === -1) {
              this.selectedFieldsArray.push({ property: item.property, visible: this.selectedFieldsArray.length < 7 ? true : false });
            }
          }
        })
      } else {
        this.fieldsDetailsArray[key].map((item, index) => {
          if (index > 0) {
            item.checked = false;
            const index = this.selectedFieldsArray.findIndex(x => x.property === item.property);
            this.selectedFieldsArray.splice(index, 1);
          }
        })
      }
    } else {
      if (event.checked) {
        if (key) {
          const [, ...dataWithoutfirstIndex] = this.fieldsDetailsArray[key];
          if (dataWithoutfirstIndex.every(x => x.checked)) {
            this.fieldsDetailsArray[key][0].checked = true;
            // this.selectedFieldsArray.push(this.fieldsDetailsArray[key][0].property);
          }
          this.selectedFieldsArray.push({ property: type, visible: this.selectedFieldsArray.length < 7 ? true : false });

        } else {
          this.selectedFieldsArray.push({ property: type, visible: this.selectedFieldsArray.length < 7 ? true : false });
        }
      } else {
        if (key) {
          this.fieldsDetailsArray[key][0].checked = false;
          const index = this.selectedFieldsArray.findIndex(x => x.property === type);
          this.selectedFieldsArray.splice(index, 1);
        } else {
          const index = this.selectedFieldsArray.findIndex(x => x.property === type);
          this.selectedFieldsArray.splice(index, 1);
        }
      }
    }
  }

  toggleColumnVisibility(column, event) {
    event.stopPropagation();
    event.stopImmediatePropagation();
  }

}
