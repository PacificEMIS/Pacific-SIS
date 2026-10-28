using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NUnit.Framework;
using opensis.data.Interface;
using opensis.data.Models;
using opensis.data.Repository;
using opensis.data.ViewModels.CourseManager;

namespace opensis.NunitTest.CourseManager
{
    /// <summary>
    /// Exercises CourseManagerRepository.CloneCourseSections against an in-memory tenant
    /// database. Each test starts from a school with two academic years, 2024 and 2025,
    /// where the 2025 lookups were created by a rollover and therefore carry new ids.
    /// </summary>
    [TestFixture]
    public class CloneCourseSectionsTests
    {
        private static readonly Guid Tenant = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private const int School = 1;
        private const decimal Y2024 = 2024;
        private const decimal Y2025 = 2025;

        // 2024 ids
        private const int Cal2024 = 1, SportsCal2024 = 3;
        private const int MathCourse2024 = 1, ScienceCourse2024 = 2, ArtCourse2024 = 3, HistoryCourse2024 = 6;
        private const int RoomA2024 = 1, RoomB2024 = 2;
        private const int Block2024 = 1, Period1Of2024 = 1, Period2Of2024 = 2;
        private const int Scale2024 = 1, AttCat2024 = 1, Year2024 = 1, Semester2024 = 1, Quarter2024 = 1;

        // 2025 ids
        private const int Cal2025 = 2;
        private const int MathCourse2025 = 4, ScienceCourse2025 = 5, HistoryCourse2025 = 7;
        private const int RoomA2025 = 3, RoomC2025 = 4;
        private const int Block2025 = 2, Period1Of2025 = 3, Period2Of2025 = 4;
        private const int Scale2025 = 2, AttCat2025 = 2, Year2025 = 2, Semester2025 = 2;

        // sections
        private const int MathA = 101, MathB = 102, ScienceA = 103, ArtA = 104, ScienceB = 105, ScienceC = 106, HistoryA = 107, Sports = 108, Quarterly = 109;
        private const int ExistingHistoryA = 201;

        private const int ActiveStaff = 1, LeftStaff = 2;
        private static readonly Guid ActiveStaffGuid = Guid.NewGuid();
        private static readonly Guid LeftStaffGuid = Guid.NewGuid();

        private static readonly DateTime Start2024 = new(2024, 1, 15), End2024 = new(2024, 12, 10);
        private static readonly DateTime Start2025 = new(2025, 1, 13), End2025 = new(2025, 12, 8);

        private sealed class InMemoryFactory : IDbContextFactory
        {
            private readonly DbContextOptions options;
            public InMemoryFactory(string name)
            {
                var builder = new DbContextOptionsBuilder();
                builder.UseInMemoryDatabase(name)
                       .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
                options = builder.Options;
            }
            public string? TenantName { get; set; }
            public string? ApiKeyValue { get; set; }
            public CRMContext? Create() => new CRMContextMySQL(options);
        }

        private InMemoryFactory factory = null!;

        [SetUp]
        public void Seed()
        {
            factory = new InMemoryFactory(Guid.NewGuid().ToString());
            using var db = factory.Create()!;

            db.SchoolCalendars.AddRange(
                Calendar(Cal2024, "Default Calendar", Y2024, Start2024, End2024, session: true),
                Calendar(SportsCal2024, "Sports Calendar", Y2024, Start2024, End2024, session: false),
                Calendar(Cal2025, "Default Calendar", Y2025, Start2025, End2025, session: true));

            db.Course.AddRange(
                Course(MathCourse2024, "Mathematics", Y2024),
                Course(ScienceCourse2024, "Science", Y2024),
                Course(ArtCourse2024, "Art", Y2024),
                Course(HistoryCourse2024, "History", Y2024),
                Course(MathCourse2025, "mathematics", Y2025),
                Course(ScienceCourse2025, "Science", Y2025),
                Course(HistoryCourse2025, "History", Y2025));

            db.Rooms.AddRange(
                new Rooms { TenantId = Tenant, SchoolId = School, RoomId = RoomA2024, Title = "Room A", Capacity = 30, AcademicYear = Y2024 },
                new Rooms { TenantId = Tenant, SchoolId = School, RoomId = RoomB2024, Title = "Room B", Capacity = 30, AcademicYear = Y2024 },
                new Rooms { TenantId = Tenant, SchoolId = School, RoomId = RoomA2025, Title = "Room A", Capacity = 30, AcademicYear = Y2025 },
                new Rooms { TenantId = Tenant, SchoolId = School, RoomId = RoomC2025, Title = "Room C", Capacity = 30, AcademicYear = Y2025 });

            db.Block.AddRange(
                new Block { TenantId = Tenant, SchoolId = School, BlockId = Block2024, BlockTitle = "Block 1", AcademicYear = Y2024 },
                new Block { TenantId = Tenant, SchoolId = School, BlockId = Block2025, BlockTitle = "Block 1", AcademicYear = Y2025 });

            db.BlockPeriod.AddRange(
                Period(Block2024, Period1Of2024, "Period 1", Y2024, calculateAttendance: true),
                Period(Block2024, Period2Of2024, "Period 2", Y2024, calculateAttendance: false),
                Period(Block2025, Period1Of2025, "Period 1", Y2025, calculateAttendance: true),
                Period(Block2025, Period2Of2025, "Period 2", Y2025, calculateAttendance: true));

            db.GradeScale.AddRange(
                new GradeScale { TenantId = Tenant, SchoolId = School, GradeScaleId = Scale2024, GradeScaleName = "Standard", AcademicYear = Y2024 },
                new GradeScale { TenantId = Tenant, SchoolId = School, GradeScaleId = Scale2025, GradeScaleName = "Standard", AcademicYear = Y2025 });

            db.AttendanceCodeCategories.AddRange(
                new AttendanceCodeCategories { TenantId = Tenant, SchoolId = School, AttendanceCategoryId = AttCat2024, Title = "Daily", AcademicYear = Y2024 },
                new AttendanceCodeCategories { TenantId = Tenant, SchoolId = School, AttendanceCategoryId = AttCat2025, Title = "Daily", AcademicYear = Y2025 });

            db.SchoolYears.AddRange(
                new SchoolYears { TenantId = Tenant, SchoolId = School, MarkingPeriodId = Year2024, Title = "Full Year", AcademicYear = Y2024, StartDate = Start2024, EndDate = End2024 },
                new SchoolYears { TenantId = Tenant, SchoolId = School, MarkingPeriodId = Year2025, Title = "Full Year", AcademicYear = Y2025, StartDate = Start2025, EndDate = End2025 });

            db.Semesters.AddRange(
                new Semesters { TenantId = Tenant, SchoolId = School, MarkingPeriodId = Semester2024, YearId = Year2024, Title = "Semester 1", AcademicYear = Y2024, StartDate = Start2024, EndDate = new DateTime(2024, 6, 30) },
                new Semesters { TenantId = Tenant, SchoolId = School, MarkingPeriodId = Semester2025, YearId = Year2025, Title = "Semester 1", AcademicYear = Y2025, StartDate = Start2025, EndDate = new DateTime(2025, 6, 27) });

            // Quarter 1 exists only in 2024
            db.Quarters.Add(new Quarters { TenantId = Tenant, SchoolId = School, MarkingPeriodId = Quarter2024, SemesterId = Semester2024, Title = "Quarter 1", AcademicYear = Y2024, StartDate = Start2024, EndDate = new DateTime(2024, 3, 31) });

            db.StaffMaster.AddRange(
                new StaffMaster { TenantId = Tenant, SchoolId = School, StaffId = ActiveStaff, StaffGuid = ActiveStaffGuid, FirstGivenName = "Ann", LastFamilyName = "Teacher", IsActive = true },
                new StaffMaster { TenantId = Tenant, SchoolId = School, StaffId = LeftStaff, StaffGuid = LeftStaffGuid, FirstGivenName = "Gone", LastFamilyName = "Away", IsActive = false });

            // --- 2024 sections -------------------------------------------------------
            var mathA = Section(MathA, MathCourse2024, "Math A", "Fixed Schedule (1)", Cal2024);
            mathA.MeetingDays = "Monday|Wednesday|Friday";
            mathA.DurationBasedOnPeriod = true;
            mathA.YrMarkingPeriodId = Year2024;
            mathA.DurationStartDate = Start2024;
            mathA.DurationEndDate = End2024;
            mathA.AttendanceTaken = true;
            mathA.Seats = 25;
            mathA.CreditHours = 1.5m;
            mathA.OnlineClassroomUrl = "https://meet.example.org/math-a";
            db.CourseSection.Add(mathA);
            db.CourseFixedSchedule.Add(new CourseFixedSchedule { TenantId = Tenant, SchoolId = School, CourseId = MathCourse2024, CourseSectionId = MathA, Serial = 1, RoomId = RoomA2024, BlockId = Block2024, PeriodId = Period1Of2024 });
            db.StaffCoursesectionSchedule.AddRange(
                StaffRow(ActiveStaff, ActiveStaffGuid, MathCourse2024, MathA, primary: true),
                StaffRow(LeftStaff, LeftStaffGuid, MathCourse2024, MathA, primary: false));

            var mathB = Section(MathB, MathCourse2024, "Math B", "Variable Schedule (2)", Cal2024);
            mathB.DurationBasedOnPeriod = true;
            mathB.SmstrMarkingPeriodId = Semester2024;
            mathB.DurationStartDate = Start2024;
            mathB.DurationEndDate = new DateTime(2024, 6, 30);
            db.CourseSection.Add(mathB);
            db.CourseVariableSchedule.AddRange(
                new CourseVariableSchedule { TenantId = Tenant, SchoolId = School, CourseId = MathCourse2024, CourseSectionId = MathB, Serial = 1, Day = "Monday", RoomId = RoomA2024, BlockId = Block2024, PeriodId = Period1Of2024, TakeAttendance = true },
                new CourseVariableSchedule { TenantId = Tenant, SchoolId = School, CourseId = MathCourse2024, CourseSectionId = MathB, Serial = 2, Day = "Tuesday", RoomId = RoomB2024, BlockId = Block2024, PeriodId = Period2Of2024, TakeAttendance = false });

            var scienceA = Section(ScienceA, ScienceCourse2024, "Science A", "Fixed Schedule (1)", Cal2024);
            scienceA.IsActive = false;
            db.CourseSection.Add(scienceA);

            db.CourseSection.Add(Section(ArtA, ArtCourse2024, "Art A", "Fixed Schedule (1)", Cal2024));

            var scienceB = Section(ScienceB, ScienceCourse2024, "Science B", "Calendar Schedule (3)", Cal2024);
            scienceB.DurationBasedOnPeriod = false;
            scienceB.DurationStartDate = new DateTime(2024, 3, 1);
            scienceB.DurationEndDate = new DateTime(2024, 12, 20);
            db.CourseSection.Add(scienceB);
            db.CourseCalendarSchedule.AddRange(
                new CourseCalendarSchedule { TenantId = Tenant, SchoolId = School, CourseId = ScienceCourse2024, CourseSectionId = ScienceB, Serial = 1, Date = new DateTime(2024, 3, 4), RoomId = RoomA2024, BlockId = Block2024, PeriodId = Period1Of2024, TakeAttendance = true },
                new CourseCalendarSchedule { TenantId = Tenant, SchoolId = School, CourseId = ScienceCourse2024, CourseSectionId = ScienceB, Serial = 2, Date = new DateTime(2024, 12, 20), RoomId = RoomA2024, BlockId = Block2024, PeriodId = Period1Of2024, TakeAttendance = true });

            var scienceC = Section(ScienceC, ScienceCourse2024, "Science C", "Block Schedule (4)", Cal2024);
            scienceC.DurationBasedOnPeriod = false;
            scienceC.DurationStartDate = Start2024;
            scienceC.DurationEndDate = End2024;
            db.CourseSection.Add(scienceC);
            db.CourseBlockSchedule.Add(new CourseBlockSchedule { TenantId = Tenant, SchoolId = School, CourseId = ScienceCourse2024, CourseSectionId = ScienceC, Serial = 1, BlockId = Block2024, PeriodId = Period2Of2024, RoomId = RoomA2024, TakeAttendance = false });

            db.CourseSection.Add(Section(HistoryA, HistoryCourse2024, "History A", "Fixed Schedule (1)", Cal2024));

            var sports = Section(Sports, HistoryCourse2024, "History Sports", "Fixed Schedule (1)", SportsCal2024);
            db.CourseSection.Add(sports);

            var quarterly = Section(Quarterly, HistoryCourse2024, "History Q1", "Fixed Schedule (1)", Cal2024);
            quarterly.DurationBasedOnPeriod = true;
            quarterly.QtrMarkingPeriodId = Quarter2024;
            quarterly.DurationStartDate = Start2024;
            quarterly.DurationEndDate = new DateTime(2024, 3, 31);
            db.CourseSection.Add(quarterly);

            // --- 2025: one section already created by hand ------------------------
            var existing = Section(ExistingHistoryA, HistoryCourse2025, "history a", "Fixed Schedule (1)", Cal2025);
            existing.AcademicYear = Y2025;
            db.CourseSection.Add(existing);

            db.SaveChanges();
        }

        // ---- seed helpers -------------------------------------------------------

        private static SchoolCalendars Calendar(int id, string title, decimal year, DateTime start, DateTime end, bool session) =>
            new() { TenantId = Tenant, SchoolId = School, CalenderId = id, Title = title, AcademicYear = year, StartDate = start, EndDate = end, SessionCalendar = session, DefaultCalender = session, Days = "12345" };

        private static Course Course(int id, string title, decimal year) =>
            new() { TenantId = Tenant, SchoolId = School, CourseId = id, CourseTitle = title, AcademicYear = year, IsCourseActive = true };

        private static BlockPeriod Period(int blockId, int periodId, string title, decimal year, bool calculateAttendance) =>
            new() { TenantId = Tenant, SchoolId = School, BlockId = blockId, PeriodId = periodId, PeriodTitle = title, AcademicYear = year, CalculateAttendance = calculateAttendance };

        private static CourseSection Section(int id, int courseId, string name, string scheduleType, int calendarId) =>
            new()
            {
                TenantId = Tenant,
                SchoolId = School,
                CourseId = courseId,
                CourseSectionId = id,
                AcademicYear = Y2024,
                CourseSectionName = name,
                ScheduleType = scheduleType,
                CalendarId = calendarId,
                GradeScaleId = Scale2024,
                AttendanceCategoryId = AttCat2024,
                Seats = 20,
                IsActive = true,
                DurationBasedOnPeriod = false,
                DurationStartDate = Start2024,
                DurationEndDate = End2024
            };

        private static StaffCoursesectionSchedule StaffRow(int staffId, Guid staffGuid, int courseId, int sectionId, bool primary) =>
            new() { TenantId = Tenant, SchoolId = School, StaffId = staffId, StaffGuid = staffGuid, CourseId = courseId, CourseSectionId = sectionId, AcademicYear = Y2024, IsAssigned = true, IsPrimaryStaff = primary, CreatedOn = DateTime.UtcNow };

        private CourseManagerRepository Repo() => new(factory);

        private static CloneCourseSectionsViewModel Request(decimal source = 2024, decimal target = 2025, int? courseId = null, bool includeStaff = true, bool dryRun = false) =>
            new() { TenantId = Tenant, SchoolId = School, SourceAcademicYear = source, TargetAcademicYear = target, CourseId = courseId, IncludeStaff = includeStaff, DryRun = dryRun, CreatedBy = "tester" };

        private CloneCourseSectionsViewModel Clone(CloneCourseSectionsViewModel? request = null)
        {
            var result = Repo().CloneCourseSections(request ?? Request());
            Assert.IsFalse(result._failure, result._message);
            return result;
        }

        private static CourseSectionCloneResult ResultFor(CloneCourseSectionsViewModel result, int sourceSectionId) =>
            result.Results.Single(r => r.SourceCourseSectionId == sourceSectionId);

        private CourseSection NewSection(CloneCourseSectionsViewModel result, int sourceSectionId)
        {
            using var db = factory.Create()!;
            var id = ResultFor(result, sourceSectionId).NewCourseSectionId!.Value;
            return db.CourseSection.Single(s => s.CourseSectionId == id);
        }

        // ---- validation -----------------------------------------------------------

        [Test]
        public void Clone_RequiresDifferentYears()
        {
            var result = Repo().CloneCourseSections(Request(source: 2025, target: 2025));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_SAME_YEAR, result._message);
        }

        [Test]
        public void Clone_RequiresASourceYear()
        {
            var request = Request();
            request.SourceAcademicYear = null;
            var result = Repo().CloneCourseSections(request);
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_YEAR_REQUIRED, result._message);
        }

        [Test]
        public void Clone_FailsWhenSourceYearDoesNotExist()
        {
            var result = Repo().CloneCourseSections(Request(source: 2019));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_NO_SOURCE_YEAR, result._message);
        }

        [Test]
        public void Clone_FailsWhenTargetYearHasNoSessionCalendar()
        {
            var result = Repo().CloneCourseSections(Request(target: 2026));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_NO_TARGET_CALENDAR, result._message);
        }

        [Test]
        public void Clone_FailsWhenSourceYearHasNoSections()
        {
            using (var db = factory.Create()!)
            {
                db.SchoolCalendars.Add(Calendar(9, "Default Calendar", 2023, new DateTime(2023, 1, 16), new DateTime(2023, 12, 11), session: true));
                db.SaveChanges();
            }
            var result = Repo().CloneCourseSections(Request(source: 2023));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_NO_SOURCE_SECTIONS, result._message);
        }

        [Test]
        public void Clone_ScopedToUnknownCourse_Fails()
        {
            var result = Repo().CloneCourseSections(Request(courseId: 999));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_COURSE_NOT_FOUND, result._message);
        }

        // ---- the happy path -------------------------------------------------------

        [Test]
        public void Clone_ReportsEverySourceSectionOnce()
        {
            var result = Clone();

            Assert.AreEqual(9, result.Results.Count);
            Assert.AreEqual(6, result.ClonedCount);
            Assert.AreEqual(3, result.SkippedCount);
            Assert.AreEqual("6 course section(s) cloned, 3 skipped.", result._message);
        }

        [Test]
        public void Clone_FixedSection_RemapsEveryReferenceByTitle()
        {
            var result = Clone();
            var report = ResultFor(result, MathA);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, MathA);
            Assert.AreEqual(Y2025, section.AcademicYear);
            Assert.AreEqual(MathCourse2025, section.CourseId, "course matched on title ignoring case");
            Assert.AreEqual("Math A", section.CourseSectionName);
            Assert.AreEqual(Cal2025, section.CalendarId);
            Assert.AreEqual(Scale2025, section.GradeScaleId);
            Assert.AreEqual(AttCat2025, section.AttendanceCategoryId);
            Assert.AreEqual(Year2025, section.YrMarkingPeriodId);
            Assert.IsNull(section.SmstrMarkingPeriodId);
            Assert.AreEqual(Start2025, section.DurationStartDate, "period-based duration follows the matched marking period");
            Assert.AreEqual(End2025, section.DurationEndDate);
            Assert.AreEqual("Monday|Wednesday|Friday", section.MeetingDays);
            Assert.AreEqual(true, section.AttendanceTaken);
            Assert.AreEqual(25, section.Seats);
            Assert.AreEqual(1.5m, section.CreditHours);
            Assert.AreEqual("https://meet.example.org/math-a", section.OnlineClassroomUrl);
            Assert.AreEqual(true, section.IsActive);
            Assert.AreEqual("tester", section.CreatedBy);
            Assert.IsNotNull(section.CreatedOn);

            using var db = factory.Create()!;
            var fixedRow = db.CourseFixedSchedule.Single(r => r.CourseSectionId == section.CourseSectionId);
            Assert.AreEqual(RoomA2025, fixedRow.RoomId);
            Assert.AreEqual(Block2025, fixedRow.BlockId);
            Assert.AreEqual(Period1Of2025, fixedRow.PeriodId);
            Assert.AreEqual(MathCourse2025, fixedRow.CourseId);
            Assert.AreEqual(Scale2025, fixedRow.GradeScaleId);
        }

        [Test]
        public void Clone_AssignsNewIdsAboveTheExistingMaximum()
        {
            var result = Clone();

            var ids = result.Results.Where(r => r.Cloned).Select(r => r.NewCourseSectionId!.Value).OrderBy(i => i).ToList();
            Assert.AreEqual(ExistingHistoryA + 1, ids.First());
            Assert.AreEqual(Enumerable.Range(ExistingHistoryA + 1, ids.Count).ToList(), ids, "ids are consecutive");

            using var db = factory.Create()!;
            var fixedSerials = db.CourseFixedSchedule.Where(r => r.CourseSectionId > ExistingHistoryA).Select(r => r.Serial).ToList();
            Assert.AreEqual(fixedSerials.Count, fixedSerials.Distinct().Count(), "schedule serials are unique");
            Assert.IsTrue(fixedSerials.All(s => s > 1), "serials continue after the existing rows");
        }

        [Test]
        public void Clone_AssignsActiveTeachersAndWarnsAboutThoseWhoLeft()
        {
            var result = Clone();
            var report = ResultFor(result, MathA);
            var section = NewSection(result, MathA);

            using var db = factory.Create()!;
            var staff = db.StaffCoursesectionSchedule.Where(s => s.CourseSectionId == section.CourseSectionId).ToList();
            Assert.AreEqual(1, staff.Count);
            Assert.AreEqual(ActiveStaff, staff[0].StaffId);
            Assert.AreEqual(ActiveStaffGuid, staff[0].StaffGuid);
            Assert.AreEqual(true, staff[0].IsPrimaryStaff);
            Assert.AreEqual(true, staff[0].IsAssigned);
            Assert.AreEqual(Y2025, staff[0].AcademicYear);
            Assert.AreEqual(MathCourse2025, staff[0].CourseId);
            Assert.AreEqual(Year2025, staff[0].YrMarkingPeriodId);
            Assert.AreEqual(Start2025, staff[0].DurationStartDate);
            Assert.AreEqual("Monday|Wednesday|Friday", staff[0].MeetingDays);
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Gone Away") && w.Contains("no longer active")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_WithoutStaff_AssignsNoTeachers()
        {
            var result = Clone(Request(includeStaff: false));
            var section = NewSection(result, MathA);

            using var db = factory.Create()!;
            Assert.IsFalse(db.StaffCoursesectionSchedule.Any(s => s.CourseSectionId == section.CourseSectionId));
            Assert.IsEmpty(ResultFor(result, MathA).Warnings);
        }

        // ---- skips ------------------------------------------------------------------

        [Test]
        public void Clone_SkipsSectionsWhoseCourseIsMissingInTargetYear()
        {
            var report = ResultFor(Clone(), ArtA);
            Assert.IsFalse(report.Cloned);
            Assert.IsNull(report.NewCourseSectionId);
            StringAssert.Contains("'Art'", report.Reason);
            StringAssert.Contains("2025", report.Reason);
        }

        [Test]
        public void Clone_SkipsInactiveSections()
        {
            var report = ResultFor(Clone(), ScienceA);
            Assert.IsFalse(report.Cloned);
            Assert.AreEqual(CourseManagerRepository.SKIP_INACTIVE, report.Reason);
        }

        [Test]
        public void Clone_SkipsSectionsWhoseNameAlreadyExistsInTargetYear()
        {
            var report = ResultFor(Clone(), HistoryA);
            Assert.IsFalse(report.Cloned);
            Assert.AreEqual(CourseManagerRepository.SKIP_NAME_EXISTS, report.Reason);
        }

        [Test]
        public void Clone_RunTwice_SecondRunChangesNothing()
        {
            var first = Clone();
            int sectionsAfterFirst, staffAfterFirst;
            using (var db = factory.Create()!)
            {
                sectionsAfterFirst = db.CourseSection.Count();
                staffAfterFirst = db.StaffCoursesectionSchedule.Count();
            }

            var second = Clone();

            Assert.AreEqual(0, second.ClonedCount);
            Assert.AreEqual(first.Results.Count, second.SkippedCount);
            Assert.IsTrue(first.Results.Where(r => r.Cloned).All(r => ResultFor(second, r.SourceCourseSectionId).Reason == CourseManagerRepository.SKIP_NAME_EXISTS));
            using var after = factory.Create()!;
            Assert.AreEqual(sectionsAfterFirst, after.CourseSection.Count());
            Assert.AreEqual(staffAfterFirst, after.StaffCoursesectionSchedule.Count());
        }

        [Test]
        public void Clone_SkipsSectionWhoseRoomIsAlreadyBookedInTargetYear()
        {
            using (var db = factory.Create()!)
            {
                var booked = Section(202, MathCourse2025, "Existing Fixed", "Fixed Schedule (1)", Cal2025);
                booked.AcademicYear = Y2025;
                booked.MeetingDays = "Monday|Tuesday";
                booked.DurationStartDate = Start2025;
                booked.DurationEndDate = End2025;
                db.CourseSection.Add(booked);
                db.CourseFixedSchedule.Add(new CourseFixedSchedule { TenantId = Tenant, SchoolId = School, CourseId = MathCourse2025, CourseSectionId = 202, Serial = 5, RoomId = RoomA2025, BlockId = Block2025, PeriodId = Period1Of2025 });
                db.SaveChanges();
            }

            var report = ResultFor(Clone(), MathA);

            Assert.IsFalse(report.Cloned);
            StringAssert.Contains("Room A", report.Reason);
            StringAssert.Contains("Period 1", report.Reason);
            StringAssert.Contains("Existing Fixed", report.Reason);
            using var check = factory.Create()!;
            Assert.IsFalse(check.CourseSection.Any(s => s.AcademicYear == Y2025 && s.CourseSectionName == "Math A"));
            Assert.IsFalse(check.StaffCoursesectionSchedule.Any(s => s.AcademicYear == Y2025), "nothing of the skipped section is written");
        }

        [Test]
        public void Clone_BookedRoomOnOtherDays_IsNotAConflict()
        {
            using (var db = factory.Create()!)
            {
                var booked = Section(202, MathCourse2025, "Existing Fixed", "Fixed Schedule (1)", Cal2025);
                booked.AcademicYear = Y2025;
                booked.MeetingDays = "Tuesday|Thursday";
                db.CourseSection.Add(booked);
                db.CourseFixedSchedule.Add(new CourseFixedSchedule { TenantId = Tenant, SchoolId = School, CourseId = MathCourse2025, CourseSectionId = 202, Serial = 5, RoomId = RoomA2025, BlockId = Block2025, PeriodId = Period1Of2025 });
                db.SaveChanges();
            }

            Assert.IsTrue(ResultFor(Clone(), MathA).Cloned);
        }

        [Test]
        public void Clone_SectionAllowingRoomConflicts_IsClonedDespiteBooking()
        {
            using (var db = factory.Create()!)
            {
                var booked = Section(202, MathCourse2025, "Existing Fixed", "Fixed Schedule (1)", Cal2025);
                booked.AcademicYear = Y2025;
                booked.MeetingDays = "Monday";
                db.CourseSection.Add(booked);
                db.CourseFixedSchedule.Add(new CourseFixedSchedule { TenantId = Tenant, SchoolId = School, CourseId = MathCourse2025, CourseSectionId = 202, Serial = 5, RoomId = RoomA2025, BlockId = Block2025, PeriodId = Period1Of2025 });
                db.CourseSection.Single(s => s.CourseSectionId == MathA).AllowRoomConflict = true;
                db.SaveChanges();
            }

            Assert.IsTrue(ResultFor(Clone(), MathA).Cloned);
        }

        // ---- schedule kinds -----------------------------------------------------------

        [Test]
        public void Clone_VariableSchedule_RemapsRowsAndAlignsAttendanceWithPeriod()
        {
            var result = Clone();
            var report = ResultFor(result, MathB);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, MathB);
            Assert.AreEqual(Semester2025, section.SmstrMarkingPeriodId);
            Assert.IsNull(section.YrMarkingPeriodId);
            Assert.AreEqual(Start2025, section.DurationStartDate);
            Assert.AreEqual(new DateTime(2025, 6, 27), section.DurationEndDate);

            using var db = factory.Create()!;
            var rows = db.CourseVariableSchedule.Where(r => r.CourseSectionId == section.CourseSectionId).OrderBy(r => r.Serial).ToList();
            Assert.AreEqual(2, rows.Count);

            Assert.AreEqual("Monday", rows[0].Day);
            Assert.AreEqual(RoomA2025, rows[0].RoomId);
            Assert.AreEqual(Period1Of2025, rows[0].PeriodId);
            Assert.AreEqual(Block2025, rows[0].BlockId);
            Assert.AreEqual(true, rows[0].TakeAttendance);

            Assert.AreEqual("Tuesday", rows[1].Day);
            Assert.IsNull(rows[1].RoomId, "Room B does not exist in 2025");
            Assert.AreEqual(Period2Of2025, rows[1].PeriodId);
            Assert.AreEqual(true, rows[1].TakeAttendance, "Period 2 now calculates attendance, so the row must take attendance");

            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Room 'Room B' not found")), string.Join(" / ", report.Warnings));
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Tuesday") && w.Contains("switched on")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_CalendarSchedule_MovesDatesForwardAndDropsThoseOutsideTheCalendar()
        {
            var result = Clone();
            var report = ResultFor(result, ScienceB);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, ScienceB);
            Assert.AreEqual(false, section.DurationBasedOnPeriod);
            Assert.AreEqual(new DateTime(2025, 3, 1), section.DurationStartDate, "shifted by one year");
            Assert.AreEqual(End2025, section.DurationEndDate, "clipped to the 2025 calendar");
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("clipped")), string.Join(" / ", report.Warnings));

            using var db = factory.Create()!;
            var rows = db.CourseCalendarSchedule.Where(r => r.CourseSectionId == section.CourseSectionId).ToList();
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual(new DateTime(2025, 3, 4), rows[0].Date);
            Assert.AreEqual(RoomA2025, rows[0].RoomId);
            Assert.AreEqual(Period1Of2025, rows[0].PeriodId);
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("2025-12-20") && w.Contains("outside the calendar")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_BlockSchedule_RemapsBlockPeriodAndRoom()
        {
            var result = Clone();
            var report = ResultFor(result, ScienceC);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, ScienceC);
            Assert.AreEqual(Start2025, section.DurationStartDate, "a whole-year section follows the new calendar");
            Assert.AreEqual(End2025, section.DurationEndDate);

            using var db = factory.Create()!;
            var row = db.CourseBlockSchedule.Single(r => r.CourseSectionId == section.CourseSectionId);
            Assert.AreEqual(Block2025, row.BlockId);
            Assert.AreEqual(Period2Of2025, row.PeriodId);
            Assert.AreEqual(RoomA2025, row.RoomId);
            Assert.AreEqual(true, row.TakeAttendance, "aligned with the period's calculate-attendance flag");
        }

        // ---- fallbacks ------------------------------------------------------------------

        [Test]
        public void Clone_FallsBackToDefaultCalendarWhenCalendarTitleIsMissing()
        {
            var result = Clone();
            var report = ResultFor(result, Sports);
            Assert.IsTrue(report.Cloned, report.Reason);
            Assert.AreEqual(Cal2025, NewSection(result, Sports).CalendarId);
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Sports Calendar") && w.Contains("Default Calendar")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_FallsBackToSchoolYearWhenMarkingPeriodIsMissing()
        {
            var result = Clone();
            var report = ResultFor(result, Quarterly);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, Quarterly);
            Assert.IsNull(section.QtrMarkingPeriodId);
            Assert.AreEqual(Year2025, section.YrMarkingPeriodId);
            Assert.AreEqual(true, section.DurationBasedOnPeriod);
            Assert.AreEqual(Start2025, section.DurationStartDate);
            Assert.AreEqual(End2025, section.DurationEndDate);
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Quarter 1") && w.Contains("Full Year")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_MapsSchoolYearByPositionWhenTitlesCarryTheYear()
        {
            using (var db = factory.Create()!)
            {
                db.SchoolYears.Single(y => y.MarkingPeriodId == Year2024).Title = "School Year 2024-2025";
                db.SchoolYears.Single(y => y.MarkingPeriodId == Year2025).Title = "School Year 2025-2026";
                db.SaveChanges();
            }

            var result = Clone();
            var report = ResultFor(result, MathA);
            Assert.IsTrue(report.Cloned, report.Reason);
            Assert.AreEqual(Year2025, NewSection(result, MathA).YrMarkingPeriodId);
            Assert.IsFalse(report.Warnings.Any(w => w.Contains("Marking period")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_MapsRenamedSemesterByPosition()
        {
            using (var db = factory.Create()!)
            {
                db.Semesters.Single(s => s.MarkingPeriodId == Semester2025).Title = "S1";
                db.SaveChanges();
            }

            var result = Clone();
            var report = ResultFor(result, MathB);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, MathB);
            Assert.AreEqual(Semester2025, section.SmstrMarkingPeriodId);
            Assert.AreEqual(Start2025, section.DurationStartDate);
            Assert.AreEqual(new DateTime(2025, 6, 27), section.DurationEndDate);
            Assert.IsFalse(report.Warnings.Any(w => w.Contains("Marking period")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_DoesNotMapByPositionWhenTheYearHasADifferentNumberOfSemesters()
        {
            using (var db = factory.Create()!)
            {
                db.Semesters.Single(s => s.MarkingPeriodId == Semester2025).Title = "Term 1";
                db.Semesters.Add(new Semesters { TenantId = Tenant, SchoolId = School, MarkingPeriodId = 99, YearId = Year2025, Title = "Term 2", AcademicYear = Y2025, StartDate = new DateTime(2025, 6, 28), EndDate = End2025 });
                db.SaveChanges();
            }

            var result = Clone();
            var report = ResultFor(result, MathB);
            Assert.IsTrue(report.Cloned, report.Reason);

            var section = NewSection(result, MathB);
            Assert.IsNull(section.SmstrMarkingPeriodId);
            Assert.AreEqual(Year2025, section.YrMarkingPeriodId);
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Semester 1") && w.Contains("Full Year")), string.Join(" / ", report.Warnings));
        }

        [Test]
        public void Clone_LeavesGradeScaleBlankWhenMissingInTargetYear()
        {
            using (var db = factory.Create()!)
            {
                db.GradeScale.Remove(db.GradeScale.Single(g => g.GradeScaleId == Scale2025));
                db.SaveChanges();
            }

            var result = Clone();
            var report = ResultFor(result, MathA);
            Assert.IsTrue(report.Cloned, report.Reason);
            Assert.IsNull(NewSection(result, MathA).GradeScaleId);
            Assert.IsTrue(report.Warnings.Any(w => w.Contains("Grade scale 'Standard' not found")), string.Join(" / ", report.Warnings));
        }

        // ---- scope and dry run ----------------------------------------------------------

        [Test]
        public void Clone_ScopedToCourse_OnlyClonesSectionsOfThatCourseTitle()
        {
            var result = Clone(Request(courseId: MathCourse2025));

            CollectionAssert.AreEquivalent(new[] { MathA, MathB }, result.Results.Select(r => r.SourceCourseSectionId));
            Assert.AreEqual(2, result.ClonedCount);
            using var db = factory.Create()!;
            Assert.AreEqual(1 + 2, db.CourseSection.Count(s => s.AcademicYear == Y2025));
        }

        [Test]
        public void Clone_ScopedToCourseWithoutSourceSections_Fails()
        {
            using (var db = factory.Create()!)
            {
                db.Course.Add(Course(8, "Music", Y2025));
                db.SaveChanges();
            }
            var result = Repo().CloneCourseSections(Request(courseId: 8));
            Assert.IsTrue(result._failure);
            Assert.AreEqual(CourseManagerRepository.CLONE_NO_SOURCE_SECTIONS_FOR_COURSE, result._message);
        }

        [Test]
        public void Clone_DryRun_ReportsWithoutWriting()
        {
            var result = Clone(Request(dryRun: true));

            Assert.AreEqual(6, result.ClonedCount);
            Assert.AreEqual(3, result.SkippedCount);
            StringAssert.StartsWith("Preview:", result._message);
            Assert.IsTrue(ResultFor(result, MathA).Cloned);
            Assert.IsTrue(ResultFor(result, MathA).Warnings.Any(w => w.Contains("Gone Away")), "warnings are reported in the preview too");

            using var db = factory.Create()!;
            Assert.AreEqual(1, db.CourseSection.Count(s => s.AcademicYear == Y2025));
            Assert.IsFalse(db.StaffCoursesectionSchedule.Any(s => s.AcademicYear == Y2025));
            Assert.IsFalse(db.CourseFixedSchedule.Any(r => r.CourseSectionId > ExistingHistoryA));
        }

        [Test]
        public void Clone_LeavesSourceYearUntouched()
        {
            int before;
            using (var db = factory.Create()!)
            {
                before = db.CourseSection.Count(s => s.AcademicYear == Y2024) + db.CourseFixedSchedule.Count(r => r.CourseSectionId < 200) + db.StaffCoursesectionSchedule.Count(s => s.AcademicYear == Y2024);
            }

            Clone();

            using var after = factory.Create()!;
            var count = after.CourseSection.Count(s => s.AcademicYear == Y2024) + after.CourseFixedSchedule.Count(r => r.CourseSectionId < 200) + after.StaffCoursesectionSchedule.Count(s => s.AcademicYear == Y2024);
            Assert.AreEqual(before, count);
            Assert.AreEqual(MathCourse2024, after.CourseSection.Single(s => s.CourseSectionId == MathA).CourseId);
        }
    }
}
