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
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import icClose from '@iconify/icons-ic/twotone-close';
import { fadeInUp400ms } from '../../../../../@vex/animations/fade-in-up.animation';
import { stagger60ms } from '../../../../../@vex/animations/stagger.animation';
import { RoomAddView } from '../../../../models/room.model';
import { RoomService } from '../../../../services/room.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationService } from '../../../shared/validation.service';
import { CommonService } from 'src/app/services/common.service';

@Component({
  selector: 'vex-edit-room',
  templateUrl: './edit-room.component.html',
  styleUrls: ['./edit-room.component.scss'],
  animations: [
    stagger60ms,
    fadeInUp400ms
  ]
})
export class EditRoomComponent implements OnInit {
  roomTitle;
  icClose = icClose;
  roomAddViewModel:RoomAddView= new RoomAddView();
  roomStoreViewModel:RoomAddView= new RoomAddView();
    form:FormGroup;
    editMode=false;

  constructor(
    private dialogRef: MatDialogRef<EditRoomComponent>, 
    @Inject(MAT_DIALOG_DATA) public data:any,
    private fb: FormBuilder, 
    private snackbar:MatSnackBar,
    private roomService:RoomService,
    private commonService: CommonService,
    ) {
      this.form=fb.group({
        roomId:[0],
        title:['',[ValidationService.noWhitespaceValidator]],
        capacity:[,[ValidationService.noWhitespaceValidator,Validators.min(0),Validators.max(999)]],
        sortorder:[,[ValidationService.noWhitespaceValidator,Validators.min(1)]],
        description:[],
        isActive:[true]
  
      })
      if(data==null){
        this.roomTitle="addRoom";
      }
      else{
        this.editMode=true;
        this.roomTitle="editRoom";
        this.roomAddViewModel.tableRoom=data
        this.form.controls.roomId.patchValue(data.roomId)
        this.form.controls.title.patchValue(data.title)
        this.form.controls.capacity.patchValue(data.capacity)
        this.form.controls.sortorder.patchValue(data.sortOrder)
        this.form.controls.description.patchValue(data.description)
        this.form.controls.isActive.patchValue(data.isActive)
      }    

   }

  ngOnInit(): void {

  }
  submit(){
    this.form.markAllAsTouched();
    if (this.form.valid) {
    if (this.form.controls.roomId.value === 0){
      this.roomAddViewModel.tableRoom.title = this.form.controls.title.value;
      this.roomAddViewModel.tableRoom.capacity = this.form.controls.capacity.value;
      this.roomAddViewModel.tableRoom.sortOrder = this.form.controls.sortorder.value;
      this.roomAddViewModel.tableRoom.description = this.form.controls.description.value;
      this.roomAddViewModel.tableRoom.isActive = this.form.controls.isActive.value;
      this.roomService.addRoom(this.roomAddViewModel).subscribe(
        (res) => {
          if(typeof(res) === 'undefined'){
            this.snackbar.open('Room list failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
          else{
          if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            } 
            else {
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
              this.dialogRef.close('submited');
            }
          }
        }
      );
        
    }
    else{
      this.roomAddViewModel.tableRoom.roomId=this.form.controls.roomId.value
      this.roomAddViewModel.tableRoom.title=this.form.controls.title.value
      this.roomAddViewModel.tableRoom.capacity=this.form.controls.capacity.value
      this.roomAddViewModel.tableRoom.sortOrder=this.form.controls.sortorder.value
      this.roomAddViewModel.tableRoom.description=this.form.controls.description.value
      this.roomAddViewModel.tableRoom.isActive=this.form.controls.isActive.value
      this.roomService.updateRoom(this.roomAddViewModel).subscribe(
        (res)=>{
          if(typeof(res)=='undefined'){
            this.snackbar.open('Room list failed. ' + sessionStorage.getItem("httpError"), '', {
              duration: 10000
            });
          }
          else{
          if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
            } 
            else { 
              this.snackbar.open( res._message, '', {
                duration: 10000
              });
              this.dialogRef.close('submited');
            }
          }
        }
      )
    }
    }
  }
  cancel(){
    this.dialogRef.close();
  }


}
