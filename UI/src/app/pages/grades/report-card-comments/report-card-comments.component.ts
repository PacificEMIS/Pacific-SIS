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
import { TranslateService } from '@ngx-translate/core';
import icEdit from '@iconify/icons-ic/twotone-edit';
import icDelete from '@iconify/icons-ic/twotone-delete';
import { GetAllCourseListModel } from '../../../models/course-manager.model';
import { CourseManagerService } from '../../../services/course-manager.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ReportCardService } from '../../../services/report-card.service';
import { DefaultValuesService } from '../../../common/default-values.service';
import { AddCourseCommentCategoryModel, CommentModel, DeleteCourseCommentCategoryModel, DistinctGetAllReportCardModel, DistinctReportCardModel, GetAllCourseCommentCategoryModel, ReportCardCommentModel, UpdateSortOrderForCourseCommentCategoryModel } from '../../../models/report-card.model';
import { CdkDragDrop } from "@angular/cdk/drag-drop";
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../shared-module/confirm-dialog/confirm-dialog.component';
import { LoaderService } from '../../../services/loader.service';
import { PageRolesPermission } from '../../../common/page-roles-permissions.service';
import { Permissions } from '../../../models/roll-based-access.model';
import { CommonService } from 'src/app/services/common.service';


@Component({
  selector: 'vex-report-card-comments',
  templateUrl: './report-card-comments.component.html',
  styleUrls: ['./report-card-comments.component.scss']
})
export class ReportCardCommentsComponent implements OnInit {
  icEdit = icEdit;
  icDelete = icDelete;

  selectedCategoryIndex = -1;
  getAllCourseListModel: GetAllCourseListModel = new GetAllCourseListModel();
  getAllCommentsWithUniqueCategory: DistinctGetAllReportCardModel = new DistinctGetAllReportCardModel();
  selectedCoursesForCategory = [];
  allCommentsWithCategoryInAdd: DistinctGetAllReportCardModel = new DistinctGetAllReportCardModel();
  commentListBasedOnSelectedCategory=[];
  newComment: string;
  loading: boolean;
  selectedCourseId=null;
  permissions: Permissions;
  editMode: boolean;
  constructor(
    public translateService: TranslateService,
    private courseManagerService: CourseManagerService,
    private snackbar: MatSnackBar,
    private reportCardService: ReportCardService,
    public defaultValuesService: DefaultValuesService,
    private dialog: MatDialog,
    private loaderService:LoaderService,
    private pageRolePermissions: PageRolesPermission,
    private commonService: CommonService,
    ) {
    //translateService.use('en');
    this.loaderService.isLoading.subscribe((val) => {
      this.loading = val;
    });
  }

  ngOnInit(): void {
    this.getAllCourse();
    this.getAllCourseCommentCategories();
    this.permissions = this.pageRolePermissions.checkPageRolePermission('/school/settings/grade-settings/report-card-comments')
  }

  onCourseChange(index){
    this.selectedCategoryIndex=index;
    this.editMode=false;
    this.newComment='';
    if(this.selectedCategoryIndex!==-1){
      this.selectedCourseId=this.getAllCommentsWithUniqueCategory.courseCommentCategories[this.selectedCategoryIndex].courseId;

      this.commentListBasedOnSelectedCategory= JSON.parse(JSON.stringify(this.getAllCommentsWithUniqueCategory.courseCommentCategories[this.selectedCategoryIndex].comments)) ;
      this.commentListBasedOnSelectedCategory.sort((a, b) =>a.sortOrder < b.sortOrder ? -1 : 1 )
    }else{
      this.selectedCourseId=null;
      let nullCourseCategoryIndex = this.getAllCommentsWithUniqueCategory.courseCommentCategories.findIndex(item=>item.courseId==null);
      if(nullCourseCategoryIndex!==-1){
        this.commentListBasedOnSelectedCategory=JSON.parse(JSON.stringify(this.getAllCommentsWithUniqueCategory.courseCommentCategories[nullCourseCategoryIndex].comments));
      this.commentListBasedOnSelectedCategory.sort((a, b) =>a.sortOrder < b.sortOrder ? -1 : 1 )
      }else{
        this.commentListBasedOnSelectedCategory=[];
      }
    }
  }

  getAllCourse() {
    this.courseManagerService.GetAllCourseList(this.getAllCourseListModel).subscribe(res => {
      if (res) {
      if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.getAllCourseListModel.courseViewModelList = [];
          if (!res.courseViewModelList) {
            this.snackbar.open(res._message, '', {
              duration: 1000
            });
          }
        } else {
          this.getAllCourseListModel.courseViewModelList = res.courseViewModelList;
        }
      } else {
        this.snackbar.open(this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  getAllCourseCommentCategories(){
    this.getAllCommentsWithUniqueCategory.courseCommentCategories=[];
    this.selectedCoursesForCategory=[];
    const getAllCommentsWithCategory: GetAllCourseCommentCategoryModel = new GetAllCourseCommentCategoryModel();
    getAllCommentsWithCategory.isListView=true;
    this.reportCardService.getAllCourseCommentCategory(getAllCommentsWithCategory).subscribe((res)=>{
      if(res){
       if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          
          if(res.courseCommentCategories){
            this.renderComments(res);
          }else{
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
            this.getAllCommentsWithUniqueCategory.courseCommentCategories=[];
            this.commentListBasedOnSelectedCategory=[];
          }
        }else{
          this.renderComments(res);
          this.editMode=false;
        }
      }else{
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    });
  }

  renderComments(res){
    res.courseCommentCategories.map((item)=>{
      this.createDatasetForView(item,item.courseId)
  })
  this.getAllCommentsWithUniqueCategory.courseCommentCategories.map((item)=>{
    this.selectedCoursesForCategory.push(item.courseId);
  });

  this.getAllCommentsWithUniqueCategory.courseCommentCategories.sort((a, b) =>a.courseId < b.courseId ? -1 : 1 );
  let index=this.getAllCommentsWithUniqueCategory.courseCommentCategories.findIndex(item=>item.courseId===this.selectedCourseId);
  if(index!==-1){
    if(this.getAllCommentsWithUniqueCategory.courseCommentCategories[index].applicableAllCourses){
      index=-1;
      this.onCourseChange(-1);
    }else{
      this.onCourseChange(index);
    }
  }else{
    this.onCourseChange(-1);
  }
  // if(this.selectedCategoryIndex<=this.getAllCommentsWithUniqueCategory.courseCommentCategories.length-1){
    
  //   this.onCourseChange(this.selectedCategoryIndex);
  // }else{
  //   this.onCourseChange(-1);
  // }
  }

  createDatasetForView(item,courseId){
    let existingCatIndex:number;
    if(courseId){
      existingCatIndex = this.getAllCommentsWithUniqueCategory.courseCommentCategories.findIndex(eachCat=>eachCat.courseId===item.courseId);
    }else{
      existingCatIndex = this.getAllCommentsWithUniqueCategory.courseCommentCategories.findIndex(eachCat=>eachCat.courseId===null);
    }
    if(existingCatIndex!==-1){
      let comment: CommentModel[] = [];
      comment.push({
        courseId:item.courseId,
        courseName:item.courseName,
        applicableAllCourses:item.applicableAllCourses,
        courseCommentId:item.courseCommentId,
        comment:item.comments,
        takeInput:false,
        sortOrder:item.sortOrder
      })
      this.getAllCommentsWithUniqueCategory.courseCommentCategories[existingCatIndex].comments.push(...comment)
    }else{
      let distinctReportCard: DistinctReportCardModel = new DistinctReportCardModel();
      let comment: CommentModel[] = [];
      comment.push({
        courseId:item.courseId,
        courseName:item.courseName,
        applicableAllCourses:item.applicableAllCourses,
        courseCommentId:item.courseCommentId,
        comment:item.comments,
        takeInput:false,
        sortOrder:item.sortOrder
      })
      distinctReportCard={
        static:false,

        courseCommentId:item.courseCommentId,
        courseId: item.courseId,
        courseName: item.courseName,
        applicableAllCourses:item.applicableAllCourses,
        comments:comment,
        sortOrder: item.sortOrder
      }
      this.getAllCommentsWithUniqueCategory.courseCommentCategories.push(distinctReportCard);
    }
  }

  onCourseSelect(courseDetails){
    this.selectedCoursesForCategory.push(courseDetails.courseId);
    let courseCategory: DistinctReportCardModel = new DistinctReportCardModel();
    this.defaultValuesService.getAllMandatoryVariable(courseCategory);
    courseCategory.static=true;
    courseCategory.courseId=courseDetails.courseId;
    courseCategory.courseName=courseDetails.courseTitle;
    courseCategory.comments=[];
    courseCategory.createdBy=this.defaultValuesService.getUserGuidId();
    courseCategory.updatedBy=this.defaultValuesService.getUserGuidId();
    this.getAllCommentsWithUniqueCategory.courseCommentCategories.push(courseCategory);
    this.selectedCategoryIndex=this.getAllCommentsWithUniqueCategory.courseCommentCategories.length-1;
    this.selectedCourseId=this.getAllCommentsWithUniqueCategory.courseCommentCategories[this.selectedCategoryIndex].courseId;
    this.onCourseChange(this.selectedCategoryIndex);
  }

  confirmDeleteGradeScale(details,type,index?) {
    // If type is 0, then its Category, neither its a Comment.
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      maxWidth: '400px',
      data: {
        title: 'Are you sure?',
        message: `You are about to delete a ${type===0?'Comment Category':'Comment'}.`
      }
    });
    dialogRef.afterClosed().subscribe(dialogResult => {
      if (dialogResult) {
        if(type===0){
          this.onDeleteCategory(details,index)
        }else{
          this.onCommentDelete(details)
        }
      }
    });
  }

  onCommentDelete(commentDetails){
    this.deleteCategoryFromServer(commentDetails,commentDetails.courseCommentId)
  }

  onDeleteCategory(courseDetails,index){
    if(courseDetails.static){
      this.getAllCommentsWithUniqueCategory.courseCommentCategories.splice(index,1);
      this.selectedCoursesForCategory=this.selectedCoursesForCategory.filter((item)=>item!==courseDetails.courseId);
    }else{
      this.deleteCategoryFromServer(courseDetails);
    }
  }

  deleteCategoryFromServer(courseDetails,commentId=null){
    const deleteCategory: DeleteCourseCommentCategoryModel = new DeleteCourseCommentCategoryModel();
    deleteCategory.courseId=courseDetails.courseId;
    if(commentId){
    deleteCategory.courseCommentId=commentId;
    }
    this.reportCardService.deleteCourseCommentCategory(deleteCategory).subscribe((res)=>{
      if(res){
       if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }else{
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
          this.getAllCourseCommentCategories();
        }
      }else{
      this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }

  onCommentEdit(commentDetails,index){
    this.commentListBasedOnSelectedCategory[index].takeInput = true;
    this.editMode=true;
  }

  addComments(){
    let addCommentWithCategoryModel: AddCourseCommentCategoryModel = new AddCourseCommentCategoryModel();
    this.commentListBasedOnSelectedCategory.map((item)=>{

      if(this.selectedCategoryIndex!==-1){
        let currentCategoryDetails=this.getAllCommentsWithUniqueCategory.courseCommentCategories[this.selectedCategoryIndex]
        let courseCategory: ReportCardCommentModel = new ReportCardCommentModel();
          this.defaultValuesService.getAllMandatoryVariable(courseCategory);
          courseCategory.courseId=currentCategoryDetails.courseId;
          courseCategory.courseName=currentCategoryDetails.courseName;
          courseCategory.courseCommentId=item.courseCommentId;
          courseCategory.comments=item.comment;
          courseCategory.createdBy=this.defaultValuesService.getUserGuidId();
          courseCategory.updatedBy=this.defaultValuesService.getUserGuidId();
          courseCategory.applicableAllCourses=false;
          addCommentWithCategoryModel.courseCommentCategory.push(courseCategory)

      }else{
        let courseCategory: ReportCardCommentModel = new ReportCardCommentModel();
          this.defaultValuesService.getAllMandatoryVariable(courseCategory);
          courseCategory.courseId=item.courseId;
          courseCategory.courseName=item.courseName;
          courseCategory.courseCommentId=item.courseCommentId;
          courseCategory.comments=item.comment;
          courseCategory.createdBy=this.defaultValuesService.getUserGuidId();
          courseCategory.updatedBy=this.defaultValuesService.getUserGuidId();
          courseCategory.applicableAllCourses=true;
          addCommentWithCategoryModel.courseCommentCategory.push(courseCategory)
      }
    })

    if(this.newComment.trim()){
      addCommentWithCategoryModel=this.pushNewCommentIfAny(addCommentWithCategoryModel);
    }

    this.addCommentsWithCategory(addCommentWithCategoryModel);
  }

  pushNewCommentIfAny(addCommentWithCategoryModel){

    if(this.selectedCategoryIndex!==-1){
      let currentCategoryDetails=this.getAllCommentsWithUniqueCategory.courseCommentCategories[this.selectedCategoryIndex]
      let courseCategory: ReportCardCommentModel = new ReportCardCommentModel();
        this.defaultValuesService.getAllMandatoryVariable(courseCategory);
        courseCategory.courseId=currentCategoryDetails.courseId;
        courseCategory.courseName=currentCategoryDetails.courseName;
        courseCategory.courseCommentId=0;
        courseCategory.comments=this.newComment;
        courseCategory.createdBy=this.defaultValuesService.getUserGuidId();
        courseCategory.updatedBy=this.defaultValuesService.getUserGuidId();
        courseCategory.applicableAllCourses=false;
        addCommentWithCategoryModel.courseCommentCategory.push(courseCategory)
    }else{
      let courseCategory: ReportCardCommentModel = new ReportCardCommentModel();
      this.defaultValuesService.getAllMandatoryVariable(courseCategory);
      courseCategory.courseId=null;
      courseCategory.courseName='allCourses';
      courseCategory.courseCommentId=0;
      courseCategory.comments=this.newComment;
      courseCategory.createdBy=this.defaultValuesService.getUserGuidId();
      courseCategory.updatedBy=this.defaultValuesService.getUserGuidId();
      courseCategory.applicableAllCourses=true;
      addCommentWithCategoryModel.courseCommentCategory.push(courseCategory)
    }
    return addCommentWithCategoryModel;
  }

  addCommentsWithCategory(addCommentWithCategoryModel){

    this.reportCardService.addCourseCommentCategory(addCommentWithCategoryModel).subscribe((res)=>{
      if(res){
       if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
          this.snackbar.open(res._message, '', {
            duration: 10000
          });
        }else{
          this.newComment='';
          this.getAllCourseCommentCategories();
        }
      }else{
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
      }
    })
  }
  

  onDrop(event: CdkDragDrop<string[]>){
    const updateCommentsSortOrder: UpdateSortOrderForCourseCommentCategoryModel = new UpdateSortOrderForCourseCommentCategoryModel();
    updateCommentsSortOrder.courseId = this.commentListBasedOnSelectedCategory[0].courseId;
    updateCommentsSortOrder.currentSortOrder = this.commentListBasedOnSelectedCategory[event.currentIndex].sortOrder;
    updateCommentsSortOrder.previousSortOrder = this.commentListBasedOnSelectedCategory[event.previousIndex].sortOrder
    this.reportCardService.updateSortOrderForCourseCommentCategory(updateCommentsSortOrder).subscribe(
      (res) => {
       if(res){
         if(res._failure){
        this.commonService.checkTokenValidOrNot(res._message);
            this.snackbar.open(res._message, '', {
              duration: 10000
            });
          }else{
            this.getAllCourseCommentCategories();
          }
       }else{
        this.snackbar.open('' + this.defaultValuesService.getHttpError(), '', {
          duration: 10000
        });
       }
      }
    );
  }

  


}
