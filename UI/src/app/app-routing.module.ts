import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CustomLayoutComponent } from './custom-layout/custom-layout.component';
import {AuthGuard as AuthGuard} from '../app/common/auth.guard';
import { RolePermissionGuard } from './common/role-permission.guard';
const routes: Routes = [
  // { path: '', redirectTo: '/login', pathMatch: 'full' },
  {
    path: '',
    loadChildren: () => import('./pages/auth/login/login.module').then(m => m.LoginModule),
  },
  // {
  //   path: ':id',
  //   loadChildren: () => import('./pages/auth/login/login.module').then(m => m.LoginModule),
  // },
  {
    path: 'error',
    loadChildren: () => import('./errors/errors.module').then(m => m.ErrorsModule),
  },  
  {
    path: 'school',
    component: CustomLayoutComponent,
    children: [
      {
        path: 'dashboards',
        loadChildren: () => import('./pages/dashboards/dashboard-analytics/dashboard-analytics.module').then(m => m.DashboardAnalyticsModule),
        canActivate: [AuthGuard,RolePermissionGuard]
      },
      {
    path: 'error',
    loadChildren: () => import('./errors/errors.module').then(m => m.ErrorsModule),
  }, 
      {
        path: 'teacher',
        children:[
          {
            path:'dashboards',
            loadChildren: () => import('./pages/dashboards/teacher-dashboard/teacher-dashboard.module').then(m => m.TeacherDashboardModule),
            canActivate: [AuthGuard]
          }
        ]
        
      },
      {
        path: '',
        children: [
          {
            path: 'schoolinfo',
            loadChildren: () => import('./pages/school/school-details/school-details/school-details.module').then(m => m.SchoolDetailsModule),
            canActivate: [AuthGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'marking-periods',
            loadChildren: () => import('./pages/school/marking-periods/marking-periods.module').then(m => m.MarkingPeriodsModule),
            canActivate: [AuthGuard,RolePermissionGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'schoolcalendars',
            loadChildren: () => import('./pages/school/calendar/calendar.module').then(m => m.CalendarModule),
            canActivate: [AuthGuard,RolePermissionGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'notices',
            loadChildren: () => import('./pages/school/notices/notices.module').then(m => m.NoticesModule),
            canActivate: [AuthGuard,RolePermissionGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'students',
            loadChildren: () => import('./pages/student/student.module').then(m => m.StudentModule),
            canActivate: [AuthGuard]
            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'tools/student-bulk-data-import',
            loadChildren: () => import('./pages/student/student-data-import/student-data-import.module').then(m => m.StudentDataImportModule),
            //canActivate: [AuthGuard,RolePermissionGuard]
            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'parents',
            loadChildren: () => import('./pages/parent/parentinfo/parentinfo.module').then(m => m.ParentinfoModule),
            canActivate: [AuthGuard,RolePermissionGuard]            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'staff',
            loadChildren: () => import('./pages/staff/saff.module').then(m => m.StaffModule),
            canActivate: [AuthGuard]          
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'tools/staff-bulk-data-import',
            loadChildren: () => import('./pages/staff/staff-data-import/staff-data-import.module').then(m => m.StaffDataImportModule),
            //canActivate: [AuthGuard,RolePermissionGuard]
            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'course-manager',
            loadChildren: () => import('./pages/courses/course-manager/course-manager.module').then(m => m.CourseManagerModule),
            canActivate: [AuthGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'schedule-teacher',
            loadChildren: () => import('./pages/scheduling/schedule-teacher/schedule-teacher.module').then(m => m.ScheduleTeacherModule),
            canActivate: [AuthGuard,RolePermissionGuard]           
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'settings',
            loadChildren: () => import('./pages/settings/settings.module').then(m => m.SettingsModule),
            canActivate: [AuthGuard,RolePermissionGuard]            
            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'schedule-student',
            loadChildren: () => import('./pages/scheduling/schedule-student/schedule-student.module').then(m => m.ScheduleStudentModule),
            canActivate: [AuthGuard,RolePermissionGuard]            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'group-drop',
            loadChildren: () => import('./pages/scheduling/group-drop/group-drop.module').then(m => m.GroupDropModule),
            canActivate: [AuthGuard,RolePermissionGuard]            
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'teacher-reassignment',
            loadChildren: () => import('./pages/scheduling/teacher-reassignment/teacher-reassignment.module').then(m => m.TeacherReassignmentModule),
            canActivate: [AuthGuard,RolePermissionGuard]            
          }
        ]
      },
      {
        path: '',
        children: [
          // {
          //   path: 'take-attendance',
          //   loadChildren: () => import('./pages/attendance/teacher-function/take-attendance/take-attendance.module').then(m => m.TakeAttendanceModule),
          //   // canActivate: [AuthGuard]            
          // }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'student/student-re-enroll',
            loadChildren: () => import('./pages/student/student-re-enroll/student-re-enroll.module').then(m => m.StudentReEnrollModule),
            canActivate: [AuthGuard]          
          }
        ]
      },
      {
        path: '',
        children: [
          // {
          //   path: 'staff/teacher-functions',
          //   loadChildren: () => import('./pages/attendance/teacher-function/teacher-function.module').then(m => m.TeacherFunctionModule),
            // canActivate: [AuthGuard]         
          // }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'attendance/administration',
            loadChildren: () => import('./pages/attendance/administration/administration.module').then(m => m.AdministrationModule),
            canActivate: [AuthGuard]       
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'attendance/add-absences',
            loadChildren: () => import('./pages/attendance/add-absences/add-absences.module').then(m => m.AddAbsencesModule),
            canActivate: [AuthGuard]       
          }
        ]
      },  
      {
        path: '',
        children: [
          {
            path: 'attendance/recalculate-daily-attendance',
            loadChildren: () => import('./pages/attendance/recalculate-daily-attendance/recalculate-daily-attendance.module').then(m => m.RecalculateDailyAttendanceModule),
            canActivate: [AuthGuard]    
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'attendance/missing-attendance',
            loadChildren: () => import('./pages/attendance/missing-attendance/missing-attendance.module').then(m => m.MissingAttendanceModule),
            canActivate: [AuthGuard]   
          }
        ]
      }, 
      {
        path: '',
        children: [
          {
            path: 'teacher-dashboard',
            loadChildren: () => import('./pages/dashboards/teacher-dashboard/teacher-dashboard.module').then(m => m.TeacherDashboardModule),
            canActivate: [AuthGuard]
          }
        ]
      }, 
      {
        path: '',
        children: [
          {
            path: 'class',
            loadChildren: () => import('./pages/class/class.module').then(m => m.ClassModule),
            canActivate: [AuthGuard]
          }
        ]
      }, 
      {
        path: '',
        children: [
          {
            path: 'grades/report-cards',
            loadChildren: () => import('./pages/grades/report-cards/report-cards.module').then(m => m.ReportCardsModule),
            canActivate: [AuthGuard]
          }
        ]
      }, 
      {
        path: '',
        children: [
          {
            path: 'grades/transcripts',
            loadChildren: () => import('./pages/grades/transcripts/transcripts.module').then(m => m.TranscriptsModule),
            canActivate: [AuthGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'grades/input-effort-grades',
            loadChildren: () => import('./pages/grades/input-effort-grades/input-effort-grades.module').then(m => m.InputEffortGradesModule),
            canActivate: [AuthGuard]
          }
        ]
      },
      {
        path: '',
        children: [
          {
            path: 'teacher/schedule',
            loadChildren: () => import('./pages/scheduling/teacher-view-schedule/teacher-view-schedule.module').then(m => m.TeacherViewScheduleModule),
            canActivate: [AuthGuard]
          }
        ]
      }, 
      {
        path: '',
        children: [
          {
            path: 'reports',
            loadChildren: () => import('./pages/reports/reports.module').then(m => m.ReportsModule),
            canActivate: [AuthGuard]
          }
        ]
      }, 
      {
        path: '',
        children: [
          {
            path: 'tools/access-log',
            loadChildren: () => import('./pages/tools/access-log/access-log.module').then(m => m.AccessLogModule),
            canActivate: [AuthGuard]
          }
        ]
      },
      
      {
        path: '',
        children: [
          {
            path: 'tools/database-backup',
            loadChildren: () => import('./pages/tools/database-backup/database-backup.module').then(m => m.DatabaseBackupModule),
            canActivate: [AuthGuard]
          }
        ]
      },
      
      {
        path: '',
        children: [
          {
            path: 'student/group-assign-student-info',
            loadChildren: () => import('./pages/student/group-assign-student-info/group-assign-student-info.module').then(m => m.GroupAssignStudentInfoModule),
            canActivate: [AuthGuard]
          }
        ]
      },  
      
      {
        path: '',
        children: [
          {
            path: 'grades/administration',
            loadChildren: () => import('./pages/grades/administration/administration.module').then(m => m.AdministrationModule),
            canActivate: [AuthGuard]
          }
        ]
      },

      {
        path: '',
        children: [
          {
            path: 'attendance/teacher-missing-attendance',
            loadChildren: () => import('./pages/attendance/teacher-missing-attendance/teacher-missing-attendance.module').then(m => m.TeacherMissingAttendanceModule),
            canActivate: [AuthGuard]
          }
        ]
      },

      {
        path: '',
        children: [
          {
            path: 'courses/course-catalog',
            loadChildren: () => import('./pages/courses/course-catalog/course-catalog.module').then(m => m.CourseCatalogModule),
            canActivate: [AuthGuard]
          }
        ]
      },

      {
        path: '',
        children: [
          {
            path: 'courses/my-classes',
            loadChildren: () => import('./pages/courses/my-classes/my-classes.module').then(m => m.MyClassesModule),
            canActivate: [AuthGuard]
          }
        ]
      },

      {
        path: '',
        children: [
          {
            path: 'tools/api',
            loadChildren: () => import('./pages/tools/api-keys/api-keys.module').then(m => m.ApiKeysModule),
            canActivate: [AuthGuard]
          }
        ]
      },

      {
        path: '',
        children: [
          {
            path: 'tools/rollover',
            loadChildren: () => import('./pages/tools/rollover/rollover.module').then(m => m.RolloverModule),
            canActivate: [AuthGuard]
          }
        ]
      },

      {
        path: '',
        children: [
          {
            path: 'scheduling/group-delete',
            loadChildren: () => import('./pages/scheduling/group-delete/group-delete.module').then(m => m.GroupDeleteModule),
            canActivate: [AuthGuard]
          }
        ]
      },
    ]
  },
  { path: '**', redirectTo: '/error' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    // preloadingStrategy: PreloadAllModules,
    scrollPositionRestoration: 'enabled',
    relativeLinkResolution: 'corrected',
    anchorScrolling: 'enabled'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule {
}
