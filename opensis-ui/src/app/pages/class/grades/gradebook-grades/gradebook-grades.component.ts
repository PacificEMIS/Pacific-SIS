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

import { Component, Input, OnInit } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";
import icSearch from "@iconify/icons-ic/search";
import { MatDialog } from "@angular/material/dialog";
import { AddGradeCommentsComponent } from "./add-grade-comments/add-grade-comments.component";

@Component({
  selector: "vex-gradebook-grades",
  templateUrl: "./gradebook-grades.component.html",
  styleUrls: ["./gradebook-grades.component.scss"],
})
export class GradebookGradesComponent implements OnInit {
  icSearch = icSearch;
  currentComponent: string;
  classGrade = true;
  categoryDetails = false;
  @Input() currentTab:string;

  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog
  ) {
    translateService.use("en");
  }

  ngOnInit(): void {
    this.currentTab = "gradebook";
    this.currentComponent = "gradebookGrade";
  }

  changeComponent(step) {
    this.currentComponent = step;
  }

  showCategoryList() {
    this.classGrade = false;
    this.categoryDetails = true;
  }

  backTogradeList() {
    this.categoryDetails = false;
    this.classGrade = true;
  }
  
  addGradeComment() {
    this.dialog.open(AddGradeCommentsComponent, {
      width: "500px",
    });
  }
}
