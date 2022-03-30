import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ReplaySubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SchoolService } from '../../../../app/services/school.service';
import { AllSchoolListModel, OnlySchoolListModel } from '../../../../app/models/get-all-school.model';
import { Router } from '@angular/router';
import { MarkingPeriodService } from '../../../../app/services/marking-period.service';
import { GetAcademicYearListModel, GetMarkingPeriodTitleListModel } from '../../../../app/models/marking-period.model';
import { DasboardService } from '../../../../app/services/dasboard.service';
import { DefaultValuesService } from '../../../../app/common/default-values.service';
import { CommonService } from 'src/app/services/common.service';
import { SchoolAddViewModel } from '../../../../app/models/school-master.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import * as moment from "moment";
import { ImpersonateServices } from 'src/app/services/impersonate.service';
@Component({
  selector: 'vex-select-bar',
  templateUrl: './select-bar.component.html',
  styleUrls: ['./select-bar.component.scss']
})
export class SelectBarComponent implements OnInit {
  getSchoolList: OnlySchoolListModel = new OnlySchoolListModel();
  schools=[];
  getAcademicYears: GetAcademicYearListModel = new GetAcademicYearListModel();
  markingPeriodTitleLists: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  academicYears = [];
  periods = [];
  checkForAnyNewSchool: boolean = false;
  nullValueForDropdown="Please Select";
  schoolCtrl: FormControl;
  schoolControl = 0;
  /** control for the selected Year */
  public academicYearsCtrl: FormControl = new FormControl();
  filteredSchoolsForDrop;
  /** control for the selected Part */
  public periodCtrl: FormControl = new FormControl();
  schoolFilterCtrl: FormControl;

  /** list of schools filtered by search keyword */
  public filteredSchools: ReplaySubject<AllSchoolListModel[]> = new ReplaySubject<AllSchoolListModel[]>(1);
  impersonateSubjectForSelectBar:boolean=false;

  /** Subject that emits when the component has been destroyed. */
  protected onDestroy = new Subject<void>();
  schoolAddViewModel:SchoolAddViewModel=new SchoolAddViewModel();
  constructor(private schoolService: SchoolService,
    private router: Router,
    private markingPeriodService: MarkingPeriodService,
    private dasboardService:DasboardService,
    private defaultValuesService: DefaultValuesService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private impersonateServices:ImpersonateServices
  ) {
    this.schoolService.currentMessage.pipe(takeUntil(this.onDestroy)).subscribe((res) => {
      if (res) {
        this.checkForAnyNewSchool = res;
        this.callAllSchool(); 
        this.callAcademicYearsOnSchoolSelect();
      }
    })
    this.impersonateServices.impersonateSubjectForSelectBar.subscribe(x=>{
      if(x) {
        this.impersonateSelectBarOnInit();
        this.impersonateSubjectForSelectBar=true;
      }
    })
  }

  ngOnInit() {
    this.impersonateSelectBarOnInit();
  }

  impersonateSelectBarOnInit() {
    this.callAllSchool(); //Initial call of School list
    this.markingPeriodService.currentY.subscribe((res) => {
      if (res) {
        this.callAcademicYearsOnSchoolSelect();
      }
    })
    if(this.impersonateSubjectForSelectBar){
      this.impersonateServices.impersonateSubjectForSelectBar.next(false);
      this.impersonateSubjectForSelectBar=false;
    }
  }
  callAllSchool() {

    this.getSchoolList.emailAddress= this.defaultValuesService.getEmailId();
    this.schoolService.GetAllSchools(this.getSchoolList).subscribe((data) => {
      if(data){
        if(data._failure){
          this.commonService.checkTokenValidOrNot(data._message);
        }
        this.schools = data.getSchoolForView;
        this.defaultValuesService.setSchoolCount((data?.getSchoolForView?.length)?.toString());
        /** control for the selected School */
        this.schoolCtrl = new FormControl();
        this.schoolFilterCtrl = new FormControl();
        // set initial selection
        // this.schoolCtrl.setValue(+this.defaultValuesService.getSchoolID() ? +this.defaultValuesService.getSchoolID() : this.schools[0]);
        const index = this.schools.findIndex((x) => {
          return x.schoolId === +this.defaultValuesService.getSchoolID()
        });
        this.schoolCtrl.setValue(this.schools[index === -1 ? 0 : index]);
        this.filteredSchools.next(this.schools.slice());
        /** control for the MatSelect filter keyword */
        this.schoolFilterCtrl.valueChanges
          .pipe(takeUntil(this.onDestroy))
          .subscribe(() => {
            this.filterSchools(); 
          });
        if (this.checkForAnyNewSchool) {  // If there is addition of new school then true block will be called.
          this.selectNewSchoolOnAddSchool();
          this.checkForAnyNewSchool = false;
        } else {
          this.selectSchoolOnLoad();
        }
        // based on condition different APIs will called
        this.schoolService.changeSchoolListStatus({schoolLoaded:true,schoolChanged:false,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
      }else{

      }
    });
  }
  selectSchoolOnLoad() {
    if (!this.defaultValuesService.getSchoolID()) {
      const index = this.schools.findIndex((x) => {
        return x.schoolId === +this.defaultValuesService.getSchoolID()
      });
      // this.schoolCtrl.setValue(this.schools[index === -1 ? 0 : index]);
      this.defaultValuesService.setSchoolID(this.schools[index === -1 ? 0 : index].schoolId);
      this.defaultValuesService.setSchoolOpened(this.schools[index === -1 ? 0 : index].dateSchoolOpened);
      this.defaultValuesService.setSchoolClosed(this.schools[index === -1 ? 0 : index].dateSchoolClosed);
      this.callAcademicYearsOnSchoolSelect();
    } else {
      this.setSchool();
    }
  }

  selectNewSchoolOnAddSchool() {
    this.setSchool();
  }

  setSchool() {
    let id = this.defaultValuesService.getSchoolID();
    let index = this.schools.findIndex((x) => {
      return x.schoolId === id
    });
    if (index != -1) {
      this.schoolCtrl.setValue(this.schools[index]);
      this.defaultValuesService.setSchoolOpened(this.schools[index].dateSchoolOpened);
      this.defaultValuesService.setSchoolClosed(this.schools[index].dateSchoolClosed);
    } else {
      const index = this.schools.findIndex((x) => {
        return x.schoolId === +this.defaultValuesService.getSchoolID()
      });
      this.schoolCtrl.setValue(this.schools[index === -1 ? 0 : index]);
      this.defaultValuesService.setSchoolID(this.schools[index === -1 ? 0 : index].schoolId);
      // this.schoolCtrl.setValue(+this.defaultValuesService.getSchoolID() ? +this.defaultValuesService.getSchoolID() : this.schools[0]);
      this.defaultValuesService.setSchoolOpened(this.schools[index === -1 ? 0 : index].dateSchoolOpened);
      this.defaultValuesService.setSchoolClosed(this.schools[index === -1 ? 0 : index].dateSchoolClosed);
    }
    if(!this.checkForAnyNewSchool){
      this.callAcademicYearsOnSchoolSelect();
    }
  }

  changeSchool(details) {
    this.defaultValuesService.setSchoolID(details.schoolId);
    this.defaultValuesService.setSchoolOpened(details.dateSchoolOpened);
    this.defaultValuesService.setSchoolClosed(details.dateSchoolClosed);
    this.defaultValuesService.setuserMembershipName(details.membershipType)
    this.defaultValuesService.setUserMembershipType(details.membershipType)
    this.defaultValuesService.setUserMembershipID(details.membershipId)
    this.callAcademicYearsOnSchoolSelect();
    if(this.defaultValuesService.getuserMembershipName() === 'Teacher' || this.defaultValuesService.getuserMembershipName() === 'Homeroom Teacher'){
      this.router.navigateByUrl("/school/teacher/dashboards");
    }
    else{
      this.router.navigateByUrl("/school/dashboards");
    }
    this.dasboardService.sendPageLoadEvent(true);
    this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:true,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:false});
    this.updateLastUsedSchoolId(); 
  }

  updateLastUsedSchoolId(){
    this.schoolAddViewModel.lastUsedSchoolId=this.defaultValuesService.getSchoolID();
    this.schoolService.updateLastUsedSchoolId(this.schoolAddViewModel).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
        } 
      }
      else {
        this.snackbar.open('Dashboard View failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  callAcademicYearsOnSchoolSelect() {
    this.getAcademicYears.schoolId = this.defaultValuesService.getSchoolID();
    this.markingPeriodService.getAcademicYearList(this.getAcademicYears).subscribe((res) => {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      }
      this.academicYears = res.academicYears;
      // set initial selection
      if (this.academicYears?.length > 0) {
        const academicIndex =  this.defaultValuesService.getAcademicYear() ? this.academicYears.findIndex(item => item.academyYear === this.defaultValuesService.getAcademicYear()) : this.academicYears.findIndex(item => moment(new Date()).isBetween(item.startDate, item.endDate));        
        if(academicIndex >= 0) {
        this.academicYearsCtrl.setValue(this.academicYears[academicIndex]);
        } else {
        this.academicYearsCtrl.setValue(this.academicYears[this.academicYears.length - 1]);
        }
        // this.academicYearsCtrl.setValue(this.academicYears[this.academicYears.length - 1]);
        this.defaultValuesService.setAcademicYear(this.academicYearsCtrl.value.academyYear);
        this.defaultValuesService.setFullAcademicYear(this.academicYearsCtrl.value.year);
        this.defaultValuesService.setFullYearStartDate(this.academicYearsCtrl.value.startDate);
        this.defaultValuesService.setFullYearEndDate(this.academicYearsCtrl.value.endDate);
      } else {
        this.academicYearsCtrl.setValue(this.nullValueForDropdown);
        this.defaultValuesService.setAcademicYear(null);
        this.defaultValuesService.setFullAcademicYear(null);
        this.defaultValuesService.setFullYearStartDate(null);
        this.defaultValuesService.setFullYearEndDate(null);
      }
      if(this.academicYearsCtrl.value==this.nullValueForDropdown){
        this.defaultValuesService.setAcademicYear(null);
        this.defaultValuesService.setFullAcademicYear(null);
        this.defaultValuesService.setFullYearStartDate(null);
        this.defaultValuesService.setFullYearEndDate(null);
         this.periods=[]
        this.callMarkingPeriodTitleList();
        }else{    
          this.defaultValuesService.setAcademicYear(this.academicYearsCtrl.value.academyYear); 
          this.defaultValuesService.setFullAcademicYear(this.academicYearsCtrl.value.year);
          this.defaultValuesService.setFullYearStartDate(this.academicYearsCtrl.value.startDate);
          this.defaultValuesService.setFullYearEndDate(this.academicYearsCtrl.value.endDate);
          this.callMarkingPeriodTitleList();
        }
        this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:false,dataFromUserLogin:false,academicYearChanged:false,academicYearLoaded:true});

    })
  }

  changeYear(event) {
    if(event.value==this.nullValueForDropdown){
    this.defaultValuesService.setAcademicYear(null);
    this.defaultValuesService.setFullAcademicYear(null);
    this.defaultValuesService.setFullYearStartDate(null);
    this.defaultValuesService.setFullYearEndDate(null);
    this.callMarkingPeriodTitleList();
    }else{
      this.defaultValuesService.setAcademicYear(event.value.academyYear);
      this.defaultValuesService.setFullAcademicYear(event.value.year);
      this.defaultValuesService.setFullYearStartDate(event.value.startDate);
      this.defaultValuesService.setFullYearEndDate(event.value.endDate);
      this.callMarkingPeriodTitleList();
    }
    if(this.defaultValuesService.getuserMembershipName()=== 'Teacher'){
      this.router.navigateByUrl("/school/teacher/dashboards");
    }
    else{
      this.router.navigateByUrl("/school/dashboards");
    }
    this.schoolService.changeSchoolListStatus({schoolLoaded:false,schoolChanged:false,dataFromUserLogin:false,academicYearChanged:true,academicYearLoaded:false});

  }

  callMarkingPeriodTitleList() {
       /*  If there is any marking period then particular marking period 
        will be selected which is based on current date neither select the first one. */
    if (this.defaultValuesService.getAcademicYear() !== null) {
      this.markingPeriodTitleLists.schoolId = this.defaultValuesService.getSchoolID();
      this.markingPeriodTitleLists.academicYear = this.defaultValuesService.getAcademicYear();
      this.markingPeriodService.getMarkingPeriodTitleList(this.markingPeriodTitleLists).subscribe((res) => {
        if(res._failure){
          this.commonService.checkTokenValidOrNot(res._message);
        }
        this.periods = res.period;
        if (this.periods?.length > 0) {
          for (let i = 0; i < this.periods.length; i++) {
            let today = new Date().setHours(0, 0, 0, 0);
            let startDate = new Date(this.periods[i]?.startDate).setHours(0, 0, 0, 0);
            let endDate = new Date(this.periods[i]?.endDate).setHours(0, 0, 0, 0);
            if (today <= endDate && today >= startDate) {
              this.periodCtrl.setValue(this.periods[i]);
              this.defaultValuesService.setMarkingPeriodId(this.periods[i].markingPeriodId);
              this.defaultValuesService.setMarkingPeriodStartDate(this.periods[i].startDate);
              this.defaultValuesService.setMarkingPeriodEndDate(this.periods[i].endDate);
              this.defaultValuesService.setMarkingPeriodTitle(this.periods[i].periodTitle);
              break;
            } else {
              this.periodCtrl.setValue(this.periods[0]);
              this.defaultValuesService.setMarkingPeriodId(this.periods[0].markingPeriodId);
              this.defaultValuesService.setMarkingPeriodStartDate(this.periods[0].startDate);
              this.defaultValuesService.setMarkingPeriodEndDate(this.periods[i].endDate);
              this.defaultValuesService.setMarkingPeriodTitle(this.periods[0].periodTitle);
            }
          }
        } else {
          this.periodCtrl.setValue(this.nullValueForDropdown);
          this.defaultValuesService.setMarkingPeriodId(null);
          this.defaultValuesService.setMarkingPeriodStartDate(null);
          this.defaultValuesService.setMarkingPeriodEndDate(null);
          this.defaultValuesService.setMarkingPeriodTitle(null);
        }
      })
    } else {
      this.periodCtrl.setValue(this.nullValueForDropdown);
      this.defaultValuesService.setMarkingPeriodId(null);
      this.defaultValuesService.setMarkingPeriodStartDate(null);
      this.defaultValuesService.setMarkingPeriodEndDate(null);
      this.defaultValuesService.setMarkingPeriodTitle(null);

    }
  }

  toggleSchoolControl() {
    if(this.schoolControl === 0){
      this.schoolControl = 1;
    } else {
      this.schoolControl = 0;
    }
  }

  changePeriod(event) {
    if(event.value==this.nullValueForDropdown){
      this.defaultValuesService.setMarkingPeriodId(null);
      this.defaultValuesService.setMarkingPeriodStartDate(null);
      this.defaultValuesService.setMarkingPeriodEndDate(null);
      this.defaultValuesService.setMarkingPeriodTitle(null);
      }else{
      this.defaultValuesService.setMarkingPeriodId(event.value.markingPeriodId);
      this.defaultValuesService.setMarkingPeriodStartDate(event.value.startDate);
      this.defaultValuesService.setMarkingPeriodEndDate(event.value.endDate);
      this.defaultValuesService.setMarkingPeriodTitle(event.value.periodTitle);
      }

    if(this.defaultValuesService.getuserMembershipName()=== 'Teacher'){
      this.router.navigateByUrl("/school/teacher/dashboards");
    }
    else{
      this.router.navigateByUrl("/school/dashboards");
    }
  }

  protected filterSchools() {
    if (!this.schools) {
      return;
    }
    // get the search keyword
    let search = this.schoolFilterCtrl.value;
    if (!search) {
      this.filteredSchools.next(this.schools.slice());
      return;
    } else {
      search = search.toLowerCase();
    }
    // filter the school
    this.filteredSchools.next(
      this.schools.filter(school => school.schoolName.toLowerCase().indexOf(search) > -1)
    );
  }

  ngOnDestroy() {
    this.onDestroy.next();
    this.onDestroy.complete();
    this.schoolService.changeMessage(false);
  }
}

