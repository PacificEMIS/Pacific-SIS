import { Component, OnInit } from '@angular/core';
import { TranslateService } from "@ngx-translate/core";
import icSearch from "@iconify/icons-ic/search";
import icRemoveCircle from '@iconify/icons-ic/twotone-remove-circle';
import { MatDialog } from "@angular/material/dialog";
import { AddGradeCommentsComponent } from '../add-grade-comments/add-grade-comments.component';
import { GetMarkingPeriodTitleListModel } from 'src/app/models/marking-period.model';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { MarkingPeriodService } from 'src/app/services/marking-period.service';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AllScheduledCourseSectionForStaffModel } from 'src/app/models/teacher-schedule.model';
import { TeacherScheduleService } from 'src/app/services/teacher-schedule.service';
import { debounceTime, distinctUntilChanged, map, takeUntil } from 'rxjs/operators';
import { FinalGradeService } from 'src/app/services/final-grade.service';
import { Router } from '@angular/router';
import { FormControl } from '@angular/forms';
import { AddGradebookGradeByAssignmentTypeModel, AddGradebookGradeModel, ViewGradebookGradeByAssignmentTypeModel, ViewGradebookGradeModel } from 'src/app/models/gradebook-grades.model';
import { GradeBookConfigurationService } from 'src/app/services/gradebook-configuration.service';
import { ExcelService } from 'src/app/services/excel.service';
import { Subject } from 'rxjs';
import { DatePipe } from '@angular/common';
import { LoaderService } from 'src/app/services/loader.service';
import { GradebookConfigurationAddViewModel } from '../../../../../models/gradebook-configuration.model';


@Component({
  selector: 'vex-gradebook-grade-list',
  templateUrl: './gradebook-grade-list.component.html',
  styleUrls: ['./gradebook-grade-list.component.scss'],
  providers: [DatePipe]
})
export class GradebookGradeListComponent implements OnInit {
  icSearch = icSearch;
  icRemoveCircle = icRemoveCircle;
  currentComponent: string;
  gradebookGradeList = true;
  categoryDetails = false;
  getMarkingPeriodTitleListModel: GetMarkingPeriodTitleListModel = new GetMarkingPeriodTitleListModel();
  allScheduledCourseSectionBasedOnTeacher: AllScheduledCourseSectionForStaffModel = new AllScheduledCourseSectionForStaffModel();
  staffDetails;
  searchCtrl: FormControl;
  searchCtrlForAssignmentType: FormControl;
  gradeData: any;
  classGrade: boolean = true;
  assignmentTypeDetails: any;
  viewGradebookGradeByAssignmentTypeModel: ViewGradebookGradeByAssignmentTypeModel = new ViewGradebookGradeByAssignmentTypeModel();
  addGradebookGradeByAssignmentTypeModel: AddGradebookGradeByAssignmentTypeModel = new AddGradebookGradeByAssignmentTypeModel();
  assignmentListByAssignmentType: any;
  viewGradebookGradeModel: ViewGradebookGradeModel = new ViewGradebookGradeModel();
  addGradebookGradeModel: AddGradebookGradeModel =  new AddGradebookGradeModel();
  studentListForGenerateExcel: any[];
  destroySubject$: Subject<void> = new Subject();
  loading: boolean;
  gradebookConfigurationAddViewModel: GradebookConfigurationAddViewModel = new GradebookConfigurationAddViewModel();
  selectedCourseSection;
  getCourseSectionId=[]
  changeParcentageCalculationValue
  markingPeriodId;
  isWeightedSection: boolean;
  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog,
    public defaultValuesService: DefaultValuesService,
    private markingPeriodService: MarkingPeriodService,
    private commonService: CommonService,
    private snackbar: MatSnackBar,
    private teacherScheduleService: TeacherScheduleService,
    private finalGradeService: FinalGradeService,
    private router: Router,
    private gradeBookConfigurationService: GradeBookConfigurationService,
    private excelService: ExcelService,
    private datePipe: DatePipe,
    private loaderService: LoaderService
  ) {
    // translateService.use("en");
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.staffDetails = this.finalGradeService.getStaffDetails();

    this.allScheduledCourseSectionBasedOnTeacher.staffId = this.staffDetails.staffId;
    if (!this.allScheduledCourseSectionBasedOnTeacher.staffId) {
      this.router.navigate(['/school', 'staff', 'teacher-functions', 'gradebook-grades']);
    }
  }

  ngOnInit(): void {
    this.currentComponent = "gradebookGrade";
    this.searchCtrl = new FormControl();
    this.searchCtrlForAssignmentType = new FormControl();
    this.getAllScheduledCourseSectionBasedOnTeacher();
  }

  getAllScheduledCourseSectionBasedOnTeacher() {
    //this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = null;
    this.teacherScheduleService.getAllScheduledCourseSectionForStaff(this.allScheduledCourseSectionBasedOnTeacher).pipe(
      map((res) => {
        res._userName = this.defaultValuesService.getUserName();
        return res;
      })
    ).subscribe((res) => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = [];
          if (!res.courseSectionViewList) {
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.allScheduledCourseSectionBasedOnTeacher= res;
          this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList = this.allScheduledCourseSectionBasedOnTeacher.courseSectionViewList.filter(x => x.gradeScaleType !== 'Ungraded');
          res.courseSectionViewList.map((x,index)=>{
            this.getCourseSectionId.push({courseId:x.courseId,courseSectionId:x.courseSectionId})
          })
        }
      }
      else {
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });

      }
    })
  }

  ngAfterViewInit() {
    this.searchCtrl.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        this.getGradebookGrade(term);
      } else {
        this.getGradebookGrade();
      }
    });

    this.searchCtrlForAssignmentType.valueChanges.pipe(debounceTime(500), distinctUntilChanged()).subscribe((term) => {
      if (term !== '') {
        this.getGradebookGradeByAssignmentType(term);
      } else {
        this.getGradebookGradeByAssignmentType();
      }
    });
  }


  changeComponent(step, data?) {
    this.currentComponent = step;
    this.gradeData = data;
  }

  showCategoryList(data) {
    this.classGrade = false;
    this.categoryDetails = true;
    this.assignmentTypeDetails = data;
    this.getGradebookGradeByAssignmentType();
  }


  getGradebookGradeByAssignmentType(searchValue?, includeInactive?) {
    this.viewGradebookGradeByAssignmentTypeModel.assignmentTpyeId = this.assignmentTypeDetails.assignmentTypeId;
    this.viewGradebookGradeByAssignmentTypeModel.courseSectionId = this.defaultValuesService.getSelectedCourseSection().courseSectionId;
    this.viewGradebookGradeByAssignmentTypeModel.SearchValue = searchValue ? searchValue : null;
    this.viewGradebookGradeByAssignmentTypeModel.includeInactive = includeInactive ? includeInactive.checked : false;
    // this.viewGradebookGradeByAssignmentTypeModel.courseSectionId = this.defaultValuesService.getSelectedCourseSection().courseSectionId;
    // return;
    this.gradeBookConfigurationService.viewGradebookGradeByAssignmentType(this.viewGradebookGradeByAssignmentTypeModel).subscribe((res: any)=>{
      if(res._failure){
        this.addGradebookGradeByAssignmentTypeModel = res;
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.addGradebookGradeByAssignmentTypeModel = res;
        this.assignmentListByAssignmentType = res
      }
    })
  }

  submitGradesBookByAssignmentType() {
    delete this.addGradebookGradeByAssignmentTypeModel.academicYear;
    this.addGradebookGradeByAssignmentTypeModel.markingPeriodId = this.markingPeriodId;
    this.gradeBookConfigurationService.addGradebookGradeByAssignmentType(this.addGradebookGradeByAssignmentTypeModel).subscribe((res)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.getGradebookGradeByAssignmentType();
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        // this.assignmentList = res
      }
    })
  }

  backTogradeList() {
    this.categoryDetails = false;
    this.classGrade = true;
  }
  
  addGradeComment(grade) {
    this.dialog.open(AddGradeCommentsComponent, {
      width: "500px",
      data : {comment: grade.comment}
    }).afterClosed().subscribe((res)=>{
      if(res) {
        grade.comment = res;
      }
    });
  }

  findMarkingPeriodTitleById(coursesectionDetails) {
    let markingPeriodId;
    if (coursesectionDetails.yrMarkingPeriodId) {
      markingPeriodId = '0_' + coursesectionDetails.yrMarkingPeriodId;
    } else if (coursesectionDetails.smstrMarkingPeriodId) {
      markingPeriodId = '1_' + coursesectionDetails.smstrMarkingPeriodId;
    } else if (coursesectionDetails.qtrMarkingPeriodId) {
      markingPeriodId = '2_' + coursesectionDetails.qtrMarkingPeriodId;
    } else if (coursesectionDetails.prgrsprdMarkingPeriodId) {
      markingPeriodId = '3_' + coursesectionDetails.prgrsprdMarkingPeriodId;
    } else {
      markingPeriodId = null;
    }
    return markingPeriodId;
  }

  getGradebookGrade(SearchValue?, includeInactive?, courseSection?) {    
    this.defaultValuesService.setSelectedCourseSection(courseSection?.value);
    this.selectedCourseSection = this.defaultValuesService.getSelectedCourseSection();
    this.markingPeriodId = this.findMarkingPeriodTitleById(this.selectedCourseSection);

    // return;
    this.viewGradebookGradeModel.courseSectionId = courseSection ? courseSection.value.courseSectionId : this.viewGradebookGradeModel.courseSectionId;
    this.viewGradebookGradeModel.SearchValue = SearchValue ? SearchValue : null;
    this.viewGradebookGradeModel.includeInactive = includeInactive ? includeInactive.checked : false;

    // return;
    this.gradeBookConfigurationService.viewGradebookGrade(this.viewGradebookGradeModel).subscribe((res: any)=>{
      if(res._failure){
        this.addGradebookGradeModel = res;
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseSectionId=res.courseSectionId;
        this.getCourseSectionId.map((x,index)=>{
          if(x.courseSectionId === this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseSectionId){
            this.gradebookConfigurationAddViewModel.gradebookConfiguration.courseId = x.courseId;
          }
        })
        this.gradeBookConfigurationService.viewGradebookConfiguration(this.gradebookConfigurationAddViewModel).subscribe(
          (data: GradebookConfigurationAddViewModel) => {
            if(data._failure){
              this.commonService.checkTokenValidOrNot(data._message);
            }else{
              this.changeParcentageCalculationValue=data.gradebookConfiguration.scoreRounding;
              this.isWeightedSection = data?.gradebookConfiguration?.general?.includes('weightGrades') ? true : false;

              res.assignmentsListViewModels?.map( value1 => {
                value1.studentsListViewModels.map( value => {
                    if( this.changeParcentageCalculationValue === 'up' ){
                      value.runningAvg = Math.ceil(value.runningAvg)
                      value.percentage = Math.ceil(value.percentage)
                    } else if ( this.changeParcentageCalculationValue === 'down'){
                      value.runningAvg = Math.floor(value.runningAvg)
                      value.percentage = Math.floor(value.percentage)
                    } else if ( this.changeParcentageCalculationValue === 'normal' ){
                      value.runningAvg = Math.round(value.runningAvg)
                      value.percentage = Math.round(value.percentage)
                    }
                })
              })
              this.addGradebookGradeModel = res;
            }
          }
        );
        
        this.createDataSetForExcel();
        };

      })
  }

  submitGradesBook() {
    delete this.addGradebookGradeModel.academicYear;
    this.addGradebookGradeModel.markingPeriodId = this.markingPeriodId;
    this.gradeBookConfigurationService.addGradebookGrade(this.addGradebookGradeModel).subscribe((res)=>{
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
      } else{
        this.getGradebookGrade();
        this.snackbar.open(res._message, '', {
          duration: 10000
        });
        // this.assignmentList = res
      }
    })
  }

  createDataSetForExcel() {
    this.studentListForGenerateExcel = [];
    this.addGradebookGradeModel.assignmentsListViewModels?.map((item)=>{
      item.studentsListViewModels.map((subItem)=>{
      let middleName = subItem.middleName==null ? ' ' : ' ' + subItem.middleName + ' ';

      this.studentListForGenerateExcel.push(
          {
            [this.defaultValuesService.translateKey('name')]: subItem.firstGivenName + middleName + subItem.lastFamilyName,
            [this.defaultValuesService.translateKey('studentInternalId')]: subItem.studentInternalId,
            [this.defaultValuesService.translateKey('avgAndLetterGrade')]: `${subItem.runningAvg ? subItem.runningAvg + '%' : ''} ${subItem.runningAvgGrade ? '[ '+ subItem.runningAvgGrade + ' ]' : '' }`,
            [this.defaultValuesService.translateKey('assignmentType')]: item.title,
            [this.defaultValuesService.translateKey('assignments')]: item.assignmentTitle,
            [this.defaultValuesService.translateKey('weightage')]: item.weightage,
            [this.defaultValuesService.translateKey('dueDate')]: this.datePipe.transform(item.dueDate),
            [this.defaultValuesService.translateKey('marksGiven')]: subItem.allowedMarks ? subItem.allowedMarks : 0,
            [this.defaultValuesService.translateKey('totalMarks')]: subItem.points,            
          }
        )
      });
    });
  }

  generateGradebookExcel() {
    this.excelService.exportAsExcelFile(this.studentListForGenerateExcel,'Gradebook_grades_');
  }

  showHideUngraded(status, item) {
    if(item.assignmentsListViewModels) {
      item.assignmentsListViewModels?.map((item)=>{
        item.studentsListViewModels.map((subItem)=>{
          subItem.isShowUngraded = subItem.allowedMarks && status.checked ? true : false;
        });
      });
      }

  }

  ngOnDestroy(){
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
