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

import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DasboardService } from '../../services/dasboard.service';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-class',
  templateUrl: './class.component.html',
  styleUrls: ['./class.component.scss']
})
export class ClassComponent implements OnInit {

  currentTab: string;
  selectedCourseSection;

  constructor(public translateService: TranslateService,
    private dashboardService: DasboardService,
    private router: Router,
    private defaultValuesService: DefaultValuesService) {
      this.dashboardService.selectedCourseSectionDetails.subscribe((res) => {
        if (!res) {
          this.router.navigateByUrl('/school/teacher/dashboards');
        }
      });
    //translateService.use('en');
    this.selectedCourseSection = this.defaultValuesService.getSelectedCourseSection();
  }

  ngOnInit(): void {
    this.currentTab = 'overview';
  }


  changeTab(status) {
    this.currentTab = status;
  }

}
