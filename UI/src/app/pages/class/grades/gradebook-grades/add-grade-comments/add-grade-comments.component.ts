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

import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import icClose from '@iconify/icons-ic/twotone-close';
import { DefaultValuesService } from 'src/app/common/default-values.service';

@Component({
  selector: 'vex-add-grade-comments',
  templateUrl: './add-grade-comments.component.html',
  styleUrls: ['./add-grade-comments.component.scss']
})
export class AddGradeCommentsComponent implements OnInit {
  icClose = icClose;
  isThisCurrentYeare;
  constructor(
    private dialogRef: MatDialogRef<AddGradeCommentsComponent>,
     public translateService:TranslateService,
    public defaultValuesService: DefaultValuesService,
     @Inject(MAT_DIALOG_DATA) public data,
     ) { 
    // translateService.use('en');
    this.isThisCurrentYeare=defaultValuesService.checkAcademicYear()
  }

  ngOnInit(): void {
  }

  submitComment() {
    this.dialogRef.close(this.data.comment);
  }

}
