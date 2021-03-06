import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import icMenuBook from '@iconify/icons-ic/twotone-menu-book';
import { Router } from '@angular/router';
import { DefaultValuesService } from 'src/app/common/default-values.service';
import { CourseManagerService } from 'src/app/services/course-manager.service';
import { GetAllCourseListModel } from 'src/app/models/course-manager.model';
import { CommonService } from 'src/app/services/common.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { LoaderService } from 'src/app/services/loader.service';
import { takeUntil } from 'rxjs/operators';
import { Subject } from 'rxjs';

@Component({
  selector: 'vex-schedule-report',
  templateUrl: './schedule-report.component.html',
  styleUrls: ['./schedule-report.component.scss']
})
export class ScheduleReportComponent implements OnInit , OnDestroy{

  icMenuBook = icMenuBook;
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel(); 
  courseList:any;
  loading:boolean = false;
  destroySubject$: Subject<void> = new Subject();
  schoolDetails:object;
  constructor(
    public translateService: TranslateService, 
    private router: Router,
    private commonService: CommonService,
    private courseManager: CourseManagerService,
    private loaderService: LoaderService,
    private snackbar: MatSnackBar,
    private defaultValuesService:DefaultValuesService,
    ) { 
      this.defaultValuesService.setReportCompoentTitle.next("Schedule Report");
      this.loaderService.isLoading.pipe(takeUntil(this.destroySubject$)).subscribe((val) => {
        this.loading = val;
      });
    }

  ngOnInit(): void {
    this.getAllCourse();
  }

  viewDetails(course:any) {
    this.router.navigate(['/school', 'reports', 'schedule', 'schedule-report', 'schedule-report-details'],
      {state:{courseId:course.course.courseId,courseTitle:course.course.courseTitle}});
  }
  getAllCourse(){
    this.getAllCourseListModel.scheduleReport=true;
    this.courseManager.GetAllCourseList(this.getAllCourseListModel).subscribe((data:any) => {
     if(data._failure){
        this.commonService.checkTokenValidOrNot(data._message);
        if(data.courseViewModelList){
          
        }else{
          this.snackbar.open(data._message, '', {
            duration: 1000
          }); 
        }
      }else{      
        this.courseList=data.courseViewModelList;
      }
    });
  }
  ngOnDestroy() {
    this.destroySubject$.next();
    this.destroySubject$.complete();
  }
}
