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

import { AfterViewChecked, ChangeDetectionStrategy, ChangeDetectorRef, Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import icAdd from '@iconify/icons-ic/baseline-add';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import icMoreVertical from '@iconify/icons-ic/baseline-more-vert';
import icRemoveCircle from '@iconify/icons-ic/remove-circle';
import icBack from '@iconify/icons-ic/baseline-arrow-back';
import icInfo from '@iconify/icons-ic/info';
import icClose from '@iconify/icons-ic/twotone-close';
import icCheckboxChecked from '@iconify/icons-ic/check-box';
import icCheckboxUnchecked from '@iconify/icons-ic/check-box-outline-blank';
import { TranslateService } from '@ngx-translate/core';
import { MatDialog } from '@angular/material/dialog';
import { EditCourseSectionComponent } from '../edit-course-section/edit-course-section.component';
import { CourseSectionService } from '../../../../services/course-section.service';
import { CourseSectionAddViewModel, CourseSectionDataTransferModel, GetAllCourseSectionModel } from '../../../../models/course-section.model';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoaderService } from '../../../../services/loader.service';
import { takeUntil } from 'rxjs/operators';
import { CalendarEvent, CalendarMonthViewBeforeRenderEvent, CalendarView } from 'angular-calendar';
import { Subject } from 'rxjs';
import { SchoolPeriodService } from '../../../../services/school-period.service';
import { BlockListViewModel } from '../../../../models/school-period.model';
import { ConfirmDialogComponent } from '../../../shared-module/confirm-dialog/confirm-dialog.component';
import { CryptoService } from '../../../../services/Crypto.service';
import { Permissions, RolePermissionListViewModel, RolePermissionViewModel } from '../../../../models/roll-based-access.model';
import { GradeScaleModel } from '../../../../models/grades.model';
import { stagger40ms } from '../../../../../@vex/animations/stagger.animation';
import { PageRolesPermission } from '../../../../common/page-roles-permissions.service';
import { CommonService } from 'src/app/services/common.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';
@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'vex-course-section',
  templateUrl: './course-section.component.html',
  styleUrls: ['./course-section.component.scss'],
  styles: [
    `
     .cal-month-view .bg-aqua,
      .cal-week-view .cal-day-columns .bg-aqua,
      .cal-day-view .bg-aqua {
        background-color: #ffdee4 !important;
      }
    `,
  ],
  animations: [
    stagger40ms
  ]
})
export class CourseSectionComponent implements OnInit,OnDestroy,AfterViewChecked{
  icAdd = icAdd;
  icEdit = icEdit;
  icDelete = icDelete;
  icMoreVertical = icMoreVertical;
  icRemoveCircle = icRemoveCircle;
  icInfo = icInfo;
  icBack = icBack;
  icClose = icClose;
  icCheckboxChecked = icCheckboxChecked;
  icCheckboxUnchecked = icCheckboxUnchecked;

  courseDetails = 0;
  selectedTab = 'overview';
  @Input() courseDetailsFromParent;
  @Input() staffCount;
  @Output() backToCourseFromCourseSection = new EventEmitter<CourseSectionDataTransferModel>();
  getAllCourseSectionModel:GetAllCourseSectionModel=new GetAllCourseSectionModel();
  cloneGetAllCourseSectionModel;
  selectedSectionDetails:CourseSectionAddViewModel;
  selectedCourseSection:number=0;
  nameSearch:string=''
  classRoomName:string;
  classPeriodName:string;
  classtakeAttendance:string;
  classdate:string;
  color=['bg-deep-orange','bg-red','bg-green','bg-teal','bg-cyan','bg-deep-purple','bg-pink','bg-blue'];   
  calendarDayDetails=0;
  courseSectionDetails = 0;
  destroySubject$: Subject<void> = new Subject();
  loading:boolean;
  weekendDays: number[];
  filterDays = [];
  events: CalendarEvent[] = [];
  refresh: Subject<any> = new Subject();
  view: CalendarView = CalendarView.Month;
  viewDate: Date = new Date();
  cssClass: string;
  standardGradeTitle=[];
  courseSectionAddViewModel: CourseSectionAddViewModel = new CourseSectionAddViewModel();
  permissions: Permissions;

  constructor(public translateService:TranslateService,
    private dialog: MatDialog,
    private courseSectionService:CourseSectionService,
    private snackbar: MatSnackBar,
    private loaderService:LoaderService,
    private cdr: ChangeDetectorRef,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    public defaultValuesService: DefaultValuesService
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
      this.loading = val;
    });
    this.courseSectionService.callCourseSection.pipe(takeUntil(this.destroySubject$)).subscribe((res)=>{
      if(res){
        this.getAllCourseSection();
      }
    })
   }
 
  ngOnInit(): void {
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/course-manager')
    this.getAllCourseSection();    
  }
 

  backToCourse() {
    this.backToCourseFromCourseSection.emit({courseSectionCount:this.getAllCourseSectionModel.getCourseSectionForView?.length,showCourse:true,courseId:this.courseDetailsFromParent.courseId});
  }
  
  addCourseSection() {
    this.dialog.open(EditCourseSectionComponent, {
      data: {
        courseDetails: this.courseDetailsFromParent,
        editMode: false
      },
      width: '900px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getAllCourseSection();
      }
    });
  }

  ngAfterViewChecked(){
    this.cdr.detectChanges();
 }

  changeTab(tab) {
    this.selectedTab = tab;
  }
  viewEvent(event){

    this.classPeriodName= event.title;
    this.classRoomName = event.meta.scheduleDetails.rooms.title;
    this.classdate= event.meta.scheduleDetails.date;
    this.classtakeAttendance= event.meta.scheduleDetails.takeAttendance ? 'Yes':'No';
    this.calendarDayDetails=1;
  }
  closeDetails(){
    this.calendarDayDetails=0;
  }
  closeCourseSectionDetails() {
    this.courseSectionDetails = 0;
  }
  
  getAllCourseSection(){  
    this.getAllCourseSectionModel.courseId=this.courseDetailsFromParent.courseId;
    this.getAllCourseSectionModel.academicYear =this.defaultValuesService.getAcademicYear();
    this.courseSectionService.getAllCourseSection(this.getAllCourseSectionModel).subscribe(res => {
      if (typeof (res) == 'undefined') {       
        this.snackbar.open('Course Section Failed ' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
      else {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);          
          this.getAllCourseSectionModel.getCourseSectionForView=[]
          if(!this.getAllCourseSectionModel.getCourseSectionForView){
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }
        } else {
          this.getAllCourseSectionModel = res;
          this.findGradeScaleNameById();
          this.cloneGetAllCourseSectionModel=JSON.stringify(res);
          this.findMarkingPeriodTitle();
          this.selectedSectionDetails = res.getCourseSectionForView[this.selectedCourseSection];
          if(this.selectedSectionDetails?.courseSection?.mpStartDate){
            this.viewDate = new Date(this.selectedSectionDetails?.courseSection?.mpStartDate);
          }
          else{
            this.viewDate = new Date(this.selectedSectionDetails?.courseSection?.durationStartDate);
          }
          let days = this.selectedSectionDetails.courseSection.schoolCalendars.days;
          if (days !== null && days !== undefined) {
            this.getDays(days);
          }
          this.renderCalendarPeriods(this.selectedSectionDetails);
        }
      }
    });
  }

  findGradeScaleNameById(){
    this.getAllCourseSectionModel.getCourseSectionForView.map((item)=>{
      if(!item.courseSection.gradeScaleId){
        item.courseSection.gradeScale=new GradeScaleModel()
        if(item.courseSection.gradeScaleType.toLowerCase()=='Ungraded'.toLowerCase()){
          item.courseSection.gradeScale.gradeScaleName='Not Graded';
        }else if(item.courseSection.gradeScaleType.toLowerCase()=='Numeric'.toLowerCase()){
          item.courseSection.gradeScale.gradeScaleName='Numeric';
        }else if(item.courseSection.gradeScaleType.toLowerCase()=='Teacher_Scale'.toLowerCase()){
          item.courseSection.gradeScale.gradeScaleName='Teacher own Grade Scale';
        }
        
      }
    })
  }

  //render weekends
  getDays(days: string) {
    const calendarDays = days;
    let allDays = [0, 1, 2, 3, 4, 5, 6];
    let splitDays = calendarDays.split('').map(x => +x);
    this.filterDays = allDays.filter(f => !splitDays.includes(f));
    this.weekendDays = this.filterDays;
    this.cssClass = 'bg-aqua';
    this.refresh.next();
  }

  //render calendar periods
  renderCalendarPeriods(selectedSectionDetails) {
    this.events = [];
    for (let schedule of selectedSectionDetails.courseCalendarSchedule) {
     let random=Math.floor((Math.random() * 7) + 0);
      this.events.push({
        start: new Date(schedule.date),
        end: new Date(schedule.date),
        title: schedule.blockPeriod.periodTitle,
        color: null,
        actions: null,
        allDay: schedule.takeAttendance,
        resizable: {
          beforeStart: true,
          afterEnd: true,
        },
        draggable: false,
        meta: {scheduleDetails:schedule,randomColor:this.color[random]}
      });
      this.refresh.next();
    }
  }

  //for render weekends
  beforeMonthViewRender(renderEvent: CalendarMonthViewBeforeRenderEvent): void {
      renderEvent.body.forEach((day) => {
      const dayOfMonth = day.date.getDay();
      if (this.filterDays.includes(dayOfMonth)) {
        day.cssClass = this.cssClass;
      }
    });
        
  }
 
  // Find Marking Period Title from code
  findMarkingPeriodTitle(){
    this.getAllCourseSectionModel.getCourseSectionForView?.map((item)=>{
      if(item.courseSection.durationBasedOnPeriod){
        if(item.courseSection.quarters!=null){
          item.courseSection.mpTitle=item.courseSection.quarters.title;
          item.courseSection.mpStartDate=item.courseSection.quarters.startDate;
          item.courseSection.mpEndDate=item.courseSection.quarters.endDate;
        }else if(item.courseSection.schoolYears!=null){
          item.courseSection.mpTitle=item.courseSection.schoolYears.title;
          item.courseSection.mpStartDate=item.courseSection.schoolYears.startDate;
          item.courseSection.mpEndDate=item.courseSection.schoolYears.endDate;
        }else if(item.courseSection.progressPeriods!=null){
          item.courseSection.mpTitle=item.courseSection.progressPeriods.title;
          item.courseSection.mpStartDate=item.courseSection.progressPeriods.startDate;
          item.courseSection.mpEndDate=item.courseSection.progressPeriods.endDate;
        } else{
          item.courseSection.mpTitle=item.courseSection.semesters.title;
          item.courseSection.mpStartDate=item.courseSection.semesters.startDate;
          item.courseSection.mpEndDate=item.courseSection.semesters.endDate;
        }
      }
    })
  }

  // On courseSection Change
  changeRightSectionDetails(sectionDetails: CourseSectionAddViewModel, index) {
    this.courseSectionDetails = 1;
    this.calendarDayDetails = 0;
    this.selectedSectionDetails = sectionDetails;
    this.selectedCourseSection = index;
    let days = this.selectedSectionDetails.courseSection.schoolCalendars.days;
      if (days !== null && days !== undefined) {
        this.getDays(days);
      }
    this.renderCalendarPeriods(this.selectedSectionDetails);
    this.viewDate = new Date(this.selectedSectionDetails?.courseSection?.durationStartDate);
  }

  editCourseSection(sectionDetails) {
    this.dialog.open(EditCourseSectionComponent, {
      data: {
        courseDetails: this.courseDetailsFromParent,
        editMode: true,
        courseSectionDetails: sectionDetails
      },
      width: '900px'
    }).afterClosed().subscribe((res) => {
      if (res) {
        this.getAllCourseSection();
      }else{
        this.getAllCourseSectionModel=JSON.parse(this.cloneGetAllCourseSectionModel);
        this.findMarkingPeriodTitle();
        this.selectedSectionDetails = this.getAllCourseSectionModel.getCourseSectionForView[this.selectedCourseSection];
      }
    });
  }

  openUrl(href: string) {
    let hrefCopy = href;
    if (hrefCopy.includes('http:') == false && hrefCopy.includes('https:')==false) {
        hrefCopy = 'http://' + href;
    }
    window.open(hrefCopy);
  }

  confirmDelete(selectedCourseSection){
    // call our modal window
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: "400px",
      data: {
          title: "Are you sure?",
          message: "You are about to delete "+selectedCourseSection.courseSection.courseSectionName+"."}
    });
    // listen to response
    dialogRef.afterClosed().subscribe(dialogResult => {
      // if user pressed yes dialogResult will be true, 
      // if user pressed no - it will be false
      if(dialogResult){
        this.deleteCourseSection(selectedCourseSection);
      }
   });
  }

  deleteCourseSection(selectedCourseSection){
    this.courseSectionAddViewModel.courseSection.courseId=selectedCourseSection.courseSection.courseId;
    this.courseSectionAddViewModel.courseSection.courseSectionId=selectedCourseSection.courseSection.courseSectionId;
    this.courseSectionService.deleteCourseSection(this.courseSectionAddViewModel).subscribe(data => {
      if (typeof (data) == 'undefined') {
        this.snackbar.open('Course Section Delete failed. ' + this.defaultValuesService.getHttpError(), '', {
          duration: 5000
        });
      }
      else {
       if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
          this.snackbar.open(data._message, '', {
            duration: 5000
          });
        } else {
          this.snackbar.open(data._message, '', {
            duration: 5000
          });
          if(this.selectedSectionDetails==selectedCourseSection){
            this.selectedCourseSection=0;
            this.selectedSectionDetails=this.getAllCourseSectionModel.getCourseSectionForView[0];
          }
          
          this.getAllCourseSection();
        }
      }
    });
}
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }

}
