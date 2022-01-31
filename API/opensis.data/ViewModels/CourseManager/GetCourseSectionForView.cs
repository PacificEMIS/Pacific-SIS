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

using opensis.data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.ViewModels.CourseManager
{
    public class GetCourseSectionForView
    {
        public GetCourseSectionForView()
        {
            CourseVariableSchedule = new List<CourseVariableSchedule>();
            CourseBlockSchedule = new List<CourseBlockSchedule>();
            CourseCalendarSchedule = new List<CourseCalendarSchedule>();
            BellScheduleList = new List<BellSchedule>();
            
        }
        public CourseSection? CourseSection { get; set; }
        public CourseFixedSchedule? CourseFixedSchedule { get; set; }
        public List<CourseVariableSchedule> CourseVariableSchedule { get; set; }
        public List<CourseCalendarSchedule> CourseCalendarSchedule { get; set; }
        public List<CourseBlockSchedule> CourseBlockSchedule { get; set; }
        public string? MarkingPeriod { get; set; }
        public string? StandardGradeScaleName { get; set; }
        public int? AvailableSeat { get; set; }
        public int? TotalStudentSchedule { get; set; }
        public int? TotalStaffSchedule { get; set; }
        public string? StaffName { get; set; }
        public List<DateTime>? HolidayList { get; set; }
        public List<BellSchedule> BellScheduleList { get; set; }
    }
}
