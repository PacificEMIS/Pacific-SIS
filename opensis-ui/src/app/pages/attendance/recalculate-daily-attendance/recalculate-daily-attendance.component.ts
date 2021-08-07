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

import { Component, OnInit } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import icCheckCircle from "@iconify/icons-ic/check-circle";
import { StudentRecalculateDailyAttendance } from "../../../models/student-recalculate-attendance.model";
import { DefaultValuesService } from "../../../common/default-values.service";
import { DatePipe } from "@angular/common";
import { StudentAttendanceService } from "../../../services/student-attendance.service";
import { MatSnackBar } from '@angular/material/snack-bar';
import { ValidationService } from '../../shared/validation.service';
@Component({
  selector: "vex-recalculate-daily-attendance",
  templateUrl: "./recalculate-daily-attendance.component.html",
  styleUrls: ["./recalculate-daily-attendance.component.scss"],
})
export class RecalculateDailyAttendanceComponent implements OnInit {
  dateVal:Date;
  studentRecalculateDailyAttendance = new StudentRecalculateDailyAttendance();
  fromDataValue: string;
  toDataValue: string;
  isShowMainView:boolean=true;
  isShowLoadingView:boolean=false;
  isShowSuccessView:boolean=false;
  icCheckCircle = icCheckCircle;
  constructor(
    private snackbar: MatSnackBar,
    public translateService: TranslateService,
    private defaultValueService: DefaultValuesService,
    private datepipe: DatePipe,
    private studentAttendanceService: StudentAttendanceService
  ) {
    translateService.use("en");
  }

  ngOnInit(): void {}
  dateCompare(fromDataValue){
    this.dateVal=new Date(this.datepipe.transform(fromDataValue.value,"yyyy,MM,dd"));
  }
  onSubmit(fromDataValue, toDataValue) {
    this.fromDataValue = fromDataValue.value;
    this.toDataValue = toDataValue.value;
    if(this.fromDataValue!=''&&this.toDataValue!=''){
      this.isShowMainView=false;
      this.isShowLoadingView=true;
      this.studentRecalculateDailyAttendance.tenantId =this.defaultValueService.getTenantID();
      this.studentRecalculateDailyAttendance.schoolId =this.defaultValueService.getSchoolID();
      this.studentRecalculateDailyAttendance.fromDate =this.datepipe.transform(this.fromDataValue,"yyyy-MM-dd");
      this.studentRecalculateDailyAttendance.toDate = this.datepipe.transform(this.toDataValue,"yyyy-MM-dd");
      this.studentRecalculateDailyAttendance._userName = this.toDataValue;
      this.studentRecalculateDailyAttendance._tenantName =this.defaultValueService.getUserName();
      this.studentRecalculateDailyAttendance._token =this.defaultValueService.getTenent();
      // this.studentRecalculateDailyAttendance._tokenExpiry="";
      this.studentRecalculateDailyAttendance._failure = false;
      this.studentRecalculateDailyAttendance._message = "";
      this.studentAttendanceService
      .recalculateDailyAttendence(this.studentRecalculateDailyAttendance)
      .subscribe((response) => {
        if(response._message=="The daily attendance between given timeframe has been recalculated"){
          setTimeout(() => {
           this.isShowLoadingView=false;
           this.isShowSuccessView=true;
          }, 5000);
        }else if(response._message=="No Record Found"){
          setTimeout(() => {
            this.isShowMainView=true;
            this.isShowLoadingView=false;
            this.snackbar.open(response._message, '', {
            duration: 10000
          });
          }, 5000);
        }
      });
    }else if(this.fromDataValue==''&&this.toDataValue==''){
      this.snackbar.open("Choose From date and To date", '', {
        duration: 10000
      });
    }else if(this.fromDataValue!=''&&this.toDataValue==''||this.fromDataValue==''&&this.toDataValue!=''){
      this.snackbar.open("Choose both From date and To date", '', {
        duration: 10000
      });
    }
    
  }
}
