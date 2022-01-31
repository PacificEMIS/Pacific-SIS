using opensis.core.ApiAccess.Interfaces;
using opensis.data.Interface;
using opensis.data.ViewModels.ApiAccess;
using opensis.data.ViewModels.School;
using opensis.data.ViewModels.Staff;
using opensis.data.ViewModels.Student;
using System;
using System.Collections.Generic;
using System.Text;

namespace opensis.core.ApiAccess.Services
{
    public class ApiAccessService : IApiAccessService
    {

        //private static string SUCCESS = "success";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public IApiAccessRepository apiAccessRepository;
        public ApiAccessService(IApiAccessRepository apiAccessRepository)
        {
            this.apiAccessRepository = apiAccessRepository;

        }


        public ApiAccessSchoolListViewModel GetAllSchoolList()
        {
            ApiAccessSchoolListViewModel schoolListModel = new ApiAccessSchoolListViewModel();
            try
            {
               
                schoolListModel = this.apiAccessRepository.GetAllSchoolList();
            }
            catch (Exception es)
            {
                schoolListModel._failure = true;
                schoolListModel._message = es.Message;
            }

            return schoolListModel;
        }

        public ApiAccessSchoolViewModel GetSchoolDetails(decimal academicYear)
        {
            ApiAccessSchoolViewModel schoolDetailsModel = new();
            try
            {
                schoolDetailsModel = this.apiAccessRepository.GetSchoolDetails(academicYear);
            }
            catch (Exception es)
            {
                schoolDetailsModel._failure = true;
                schoolDetailsModel._message = es.Message;
            }

            return schoolDetailsModel;
        }

        public ApiAccessStaffListViewModel GetAllStaffList()
        {
            ApiAccessStaffListViewModel staffListModel = new ApiAccessStaffListViewModel();
            try
            {
                staffListModel = this.apiAccessRepository.GetAllStaffList();
            }
            catch (Exception es)
            {
                staffListModel._failure = true;
                staffListModel._message = es.Message;
            }

            return staffListModel;
        }

       
        public ApiAccessStudentListViewModel GetAllStudentList(decimal? academicYear)
        {
            ApiAccessStudentListViewModel studentListModel = new ApiAccessStudentListViewModel();
            try
            {
                studentListModel = this.apiAccessRepository.GetAllStudentList(academicYear);
            }
            catch (Exception es)
            {
                studentListModel._failure = true;
                studentListModel._message = es.Message;
            }

            return studentListModel;
        }

       
    }
}
