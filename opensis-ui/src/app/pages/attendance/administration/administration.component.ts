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
import { MatDialog } from "@angular/material/dialog";
import icSearch from "@iconify/icons-ic/search";
import { TranslateService } from "@ngx-translate/core";
import { AddCommentsComponent } from "./add-comments/add-comments.component";
import { StudentAttendanceCommentComponent } from "./student-attendance-comment/student-attendance-comment.component";

export interface StudentList {
  studentName: string;
  studentId: number;
  grade: string;
  section: string;
  present: string;
  attendance: string;
  comment: string;
}

export const studentList: StudentList[] = [
  {studentName: 'Arthur Boucher', studentId: 15, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Sophia Brown', studentId: 378, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Wang Fang', studentId: 62, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Clare Garcia', studentId: 94, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Amelia Jones', studentId: 37, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Arthur Boucher', studentId: 122, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Amelia Jones', studentId: 78, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Arthur Boucher', studentId: 92, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Clare Garcia', studentId: 61, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
  {studentName: 'Sophia Brown', studentId: 54, grade: 'Grade 10', section: 'Section A', present: 'Full-Day', attendance: '', comment: ''},
];

@Component({
  selector: "vex-administration",
  templateUrl: "./administration.component.html",
  styleUrls: ["./administration.component.scss"],
})
export class AdministrationComponent implements OnInit {
  icSearch = icSearch;

  displayedColumns: string[] = ['studentName', 'studentId', 'grade', 'section', 'present', 'attendance', 'comment'];
  studentList = studentList;

  constructor(
    private dialog: MatDialog,
    public translateService: TranslateService
  ) {
    translateService.use("en");
  }

  ngOnInit(): void {}

  addComments(){
    this.dialog.open(AddCommentsComponent, {
      width: '600px'
    })
  }

  openStudentAttendanceComment(){
    this.dialog.open(StudentAttendanceCommentComponent, {
      width: '900px'
    })
  }

}
