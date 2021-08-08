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

import { COMMA, ENTER } from '@angular/cdk/keycodes';
import { Component, Input, OnInit } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';
import { MatDialog } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';

export interface Comment {
  name: string;
}

@Component({
  selector: 'vex-input-final-grades',
  templateUrl: './input-final-grades.component.html',
  styleUrls: ['./input-final-grades.component.scss']
})
export class InputFinalGradesComponent implements OnInit {
  @Input() currentTab:string;

  viewDetailsModal = 0;
  visible = true;
  selectable = true;
  removable = true;
  addOnBlur = true;
  readonly separatorKeysCodes: number[] = [ENTER, COMMA];
  comments: Comment[] = [
    {name: 'Shows positive attitute'},
    {name: 'Respectful towards Staff'},
    {name: 'Keeps desk clean'},
  ];

  add(event: MatChipInputEvent): void {
    const input = event.input;
    const value = event.value;

    // Add our fruit
    if ((value || '').trim()) {
      this.comments.push({name: value.trim()});
    }

    // Reset the input value
    if (input) {
      input.value = '';
    }
  }

  remove(comment: Comment): void {
    const index = this.comments.indexOf(comment);

    if (index >= 0) {
      this.comments.splice(index, 1);
    }
  }

  constructor(
    public translateService: TranslateService,
    private dialog: MatDialog
  ) {
    translateService.use("en");
  }

  ngOnInit(): void {
  }

  viewDetails(){
    if (this.viewDetailsModal === 0) {
      this.viewDetailsModal = 1;
    } else {
      this.viewDetailsModal = 0;
    }
  }

  closeDetailsModal() {
    this.viewDetailsModal = 0;
  }

}
