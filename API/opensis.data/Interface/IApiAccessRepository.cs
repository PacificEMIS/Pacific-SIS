using opensis.data.ViewModels.ApiAccess;
using opensis.data.ViewModels.School;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.data.Interface
{
    public interface IApiAccessRepository
    {
        public ApiAccessSchoolListViewModel GetAllSchoolList();
        public ApiAccessStaffListViewModel GetAllStaffList();
        public ApiAccessStudentListViewModel GetAllStudentList(decimal? academicYear);
        public ApiAccessSchoolViewModel GetSchoolDetails(decimal academicYear);
    }
}
