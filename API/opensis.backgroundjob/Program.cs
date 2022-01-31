//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using opensis.data.Interface;
//using opensis.data.Models;
//using opensis.data.ViewModels.StudentSchedule;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using opensis.backgroundjob.Models;
using opensis.backgroundjob.ViewModels;
using System;

namespace opensis.backgroundjob
{
    public class Program
    {
        //private CRMContext context;

        static void Main(string[] args)
        {
            Console.WriteLine("process started.");
            UpdateStudentCoursesectionScheduleDropDate();
            UpdateStudentEnrollmmentDropDate();
            Console.WriteLine("process completed.");
        }


        private static void UpdateStudentCoursesectionScheduleDropDate()
        {
            using (var context = new TableContext())
            {
                int i = 0;
                int? Id = 1;

                var scheduledJobsData = context.ScheduledJobs.Where(x => x.IsActive == true && x.JobScheduleDate!.Value.Date <= DateTime.UtcNow.Date && x.ApiTitle == "GroupDropForScheduledStudent").ToList();

                foreach (var scheduledJob in scheduledJobsData)
                {
                    using (var transaction = context?.Database.BeginTransaction())
                    {
                        var scheduledStudentDropModel = JsonConvert.DeserializeObject<ScheduledStudentDropModel>(scheduledJob.TaskJson!);

                        if (scheduledStudentDropModel != null)
                        {
                            if (i == 0)
                            {
                                var dataExits = context?.ScheduledJobHistories.Where(x => x.TenantId == scheduledStudentDropModel.TenantId);
                                if (dataExits?.Any() == true)
                                {
                                    var scheduledJobData = context?.ScheduledJobHistories.Where(x => x.TenantId == scheduledStudentDropModel.TenantId).Max(x => x.JobRunId);
                                    if (scheduledJobData != null)
                                    {
                                        Id = scheduledJobData + 1;
                                    }
                                }
                                i++;
                            }

                            try
                            {
                                List<StudentCoursesectionSchedule> studentCoursesectionScheduleList = new List<StudentCoursesectionSchedule>();

                                if (!string.IsNullOrEmpty(scheduledStudentDropModel.StudentId.ToString()) && scheduledStudentDropModel.StudentId > 0)
                                {
                                    foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
                                    {
                                        var studentData = context?.StudentCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudentDropModel.SchoolId && x.TenantId == scheduledStudentDropModel.TenantId && x.StudentId == scheduledStudentDropModel.StudentId && x.CourseSectionId == scheduledStudent.CourseSectionId);

                                        if (studentData != null)
                                        {
                                            studentData.IsDropped = true;

                                            studentCoursesectionScheduleList.Add(studentData);

                                        }
                                    }
                                }
                                else
                                {
                                    foreach (var scheduledStudent in scheduledStudentDropModel.studentCoursesectionScheduleList)
                                    {
                                        var studentData = context?.StudentCoursesectionSchedule.Include(c => c.CourseSection).FirstOrDefault(x => x.SchoolId == scheduledStudent.SchoolId && x.TenantId == scheduledStudent.TenantId && x.StudentId == scheduledStudent.StudentId && x.CourseSectionId == scheduledStudentDropModel.CourseSectionId);

                                        if (studentData != null)
                                        {
                                            studentData.IsDropped = true;
                                            
                                            studentCoursesectionScheduleList.Add(studentData);
                                        }
                                    }
                                }

                                context?.StudentCoursesectionSchedule.UpdateRange(studentCoursesectionScheduleList);
                                context?.SaveChanges();
                                transaction?.Commit();

                                //update job status
                                scheduledJob.LastRunStatus = true;
                                scheduledJob.LastRunTime = DateTime.UtcNow;
                                scheduledJob.IsActive = false;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = true,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //update job status
                                scheduledJob.LastRunStatus = false;
                                scheduledJob.LastRunTime = DateTime.UtcNow;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = false,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                        }
                    }
                }
            }
        }



        private static void UpdateStudentEnrollmmentDropDate()
        {
            using (var context = new TableContext())
            {
                int i = 0;
                int? Id = 1;

                var scheduledJobsData = context.ScheduledJobs.Where(x => x.IsActive == true && x.JobScheduleDate!.Value.Date <= DateTime.UtcNow.Date && x.ApiTitle == "UpdateStudentEnrollment").ToList();

                foreach (var scheduledJob in scheduledJobsData)
                {
                    using (var transaction = context?.Database.BeginTransaction())
                    {                      
                        var studentEnrollmentListModel = JsonConvert.DeserializeObject<StudentEnrollmentListModel>(scheduledJob.TaskJson!);

                        if (studentEnrollmentListModel != null)
                        {
                            if (i == 0)
                            {
                                var dataExits = context?.ScheduledJobHistories.Where(x => x.TenantId == studentEnrollmentListModel.TenantId);
                                if (dataExits?.Any() == true)
                                {
                                    var scheduledJobData = context?.ScheduledJobHistories.Where(x => x.TenantId == studentEnrollmentListModel.TenantId).Max(x => x.JobRunId);
                                    if (scheduledJobData != null)
                                    {
                                        Id = scheduledJobData + 1;
                                    }
                                }
                                i++;
                            }

                            try
                            {
                                foreach (var studentEnrollmentList in studentEnrollmentListModel.studentEnrollments)
                                {
                                    if (studentEnrollmentList.EnrollmentId > 0)
                                    {
                                        if (studentEnrollmentList.ExitCode != null)
                                        {                                          
                                            var studentExitCode = context?.StudentEnrollmentCode.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.EnrollmentCode.ToString() == studentEnrollmentList.ExitCode); //fetching enrollemnt code type 

                                            if (studentExitCode!.Type!.ToLower() == "Drop (Transfer)".ToLower())
                                            {
                                                var StudentMasterData = context?.StudentMaster.Where(x => x.TenantId == studentEnrollmentList.TenantId && x.StudentGuid == studentEnrollmentListModel.StudentGuid).ToList();

                                                if (StudentMasterData?.Any() == true)
                                                {
                                                    var studentCurrentSchool = StudentMasterData.FirstOrDefault(x => x.SchoolId == studentEnrollmentList.TransferredSchoolId);
                                                    if (studentCurrentSchool != null)
                                                    {
                                                        studentCurrentSchool.IsActive = true;

                                                        var studentEnrollmentUpdate = context?.StudentEnrollment.FirstOrDefault(x => x.TenantId == studentEnrollmentList.TenantId && x.SchoolId == studentEnrollmentList.SchoolId && x.StudentId == studentEnrollmentList.StudentId && x.EnrollmentId == studentEnrollmentList.EnrollmentId);
                                                        if (studentEnrollmentUpdate != null)
                                                        {
                                                            studentEnrollmentUpdate.IsActive = false;
                                                        }
                                                       

                                                        if (studentCurrentSchool.StudentPortalId != null)
                                                        {
                                                            var userMasterData = context?.UserMaster.FirstOrDefault(x => x.EmailAddress == studentCurrentSchool.StudentPortalId && x.TenantId == studentCurrentSchool.TenantId);
                                                            if (userMasterData != null)
                                                            {
                                                                context?.UserMaster.Remove(userMasterData);

                                                                UserMaster userMaster = new UserMaster();
                                                                userMaster.TenantId = studentCurrentSchool.TenantId;
                                                                userMaster.SchoolId = studentCurrentSchool.SchoolId;
                                                                userMaster.UserId = studentCurrentSchool.StudentId;
                                                                userMaster.Name = userMasterData.Name;
                                                                userMaster.EmailAddress = userMasterData.EmailAddress;
                                                                userMaster.PasswordHash = userMasterData.PasswordHash;
                                                                userMaster.LangId = userMasterData.LangId;
                                                                var membershipsId = context?.Membership.Where(x => x.SchoolId == studentCurrentSchool.SchoolId && x.TenantId == studentEnrollmentList.TenantId && x.Profile == "Student").Select(x => x.MembershipId).FirstOrDefault();
                                                                userMaster.MembershipId = (int)membershipsId!;
                                                                userMaster.UpdatedOn = DateTime.UtcNow;
                                                                userMaster.UpdatedBy = studentEnrollmentList.UpdatedBy;
                                                                userMaster.IsActive = true;
                                                                context?.UserMaster.Add(userMaster);

                                                            }
                                                        }
                                                        //context?.SaveChanges();
                                                    }

                                                    var studentOldSchool = StudentMasterData.Where(x => x.SchoolId != studentEnrollmentList.TransferredSchoolId).ToList();
                                                    if (studentOldSchool?.Any() == true)
                                                    {
                                                        studentOldSchool.ForEach(x => x.IsActive = false);
                                                    }
                                                    context?.SaveChanges();
                                                }
                                            }
                                            else
                                            {
                                                if (studentExitCode.Type.ToLower() == "Drop".ToLower())
                                                {
                                                    context?.StudentMaster.Where(x => x.StudentGuid == studentEnrollmentList.StudentGuid && x.SchoolId == studentEnrollmentList.SchoolId).ToList().ForEach(x => x.IsActive = false);


                                                    context?.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }
                                transaction?.Commit();

                                //update job status
                                scheduledJob.LastRunStatus = true;
                                scheduledJob.LastRunTime = DateTime.UtcNow;
                                scheduledJob.IsActive = false;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = true,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                //update job status
                                scheduledJob.LastRunStatus = false;
                                scheduledJob.LastRunTime = DateTime.UtcNow;

                                var scheduledJobHistory = new ScheduledJobHistory
                                {
                                    TenantId = scheduledJob.TenantId,
                                    SchoolId = scheduledJob.SchoolId,
                                    JobId = scheduledJob.JobId,
                                    JobRunId = (int)Id,
                                    ScheduledDate = scheduledJob.JobScheduleDate,
                                    JobStatus = false,
                                    RunTime = DateTime.UtcNow
                                };
                                context?.ScheduledJobHistories.Add(scheduledJobHistory);
                                Id++;
                                context?.SaveChanges();
                            }
                        }
                    }
                }
            }
        }
    }
}
    

                   
               
    

