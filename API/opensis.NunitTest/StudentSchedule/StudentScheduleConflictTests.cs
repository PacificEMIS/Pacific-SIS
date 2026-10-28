using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using opensis.data.Helper;
using opensis.data.Models;

namespace opensis.NunitTest.StudentSchedule
{
    /// <summary>
    /// Covers the period-conflict rules and the conflict comment built for the
    /// scheduling report. Rows mirror all_course_section_view, one per schedule row.
    /// </summary>
    [TestFixture]
    public class StudentScheduleConflictTests
    {
        private const string MonToFri = "Monday|Tuesday|Wednesday|Thursday|Friday";

        private static readonly IReadOnlyDictionary<int, string> Periods = new Dictionary<int, string>
        {
            { 35, "1-2 Period 1" },
            { 36, "1-2 Period 2" },
            { 43, "Period 1" },
        };

        private static readonly IReadOnlyDictionary<int, string> Rooms = new Dictionary<int, string>
        {
            { 17, "Room#1- G1" },
            { 18, "Room#1- G2" },
        };

        private static AllCourseSectionView Fixed(int sectionId, string courseTitle, string sectionName, int periodId, string days, int? roomId = null)
        {
            return new AllCourseSectionView
            {
                CourseSectionId = sectionId,
                CourseTitle = courseTitle,
                CourseSectionName = sectionName,
                FixedPeriodId = periodId,
                FixedDays = days,
                FixedRoomId = roomId,
            };
        }

        private static AllCourseSectionView Variable(int sectionId, int periodId, string day, int? roomId = null)
        {
            return new AllCourseSectionView
            {
                CourseSectionId = sectionId,
                CourseTitle = "Var course",
                CourseSectionName = "Var section",
                VarPeriodId = periodId,
                VarDay = day,
                VarRoomId = roomId,
            };
        }

        private static AllCourseSectionView Block(int sectionId, int blockId, int periodId, int roomId)
        {
            return new AllCourseSectionView
            {
                CourseSectionId = sectionId,
                CourseTitle = "Block course",
                CourseSectionName = "Block section",
                BlockId = blockId,
                BlockPeriodId = periodId,
                BlockRoomId = roomId,
            };
        }

        [Test]
        public void MeetsAtSameTime_FixedSamePeriodSameDays_IsConflict()
        {
            var target = Fixed(137, "G2 English Reading", "English Reading G2", 35, MonToFri, 18);
            var existing = Fixed(136, "G1 English Reading", "English Reading G1", 35, MonToFri, 17);

            Assert.IsTrue(StudentScheduleConflict.MeetsAtSameTime(target, existing));
        }

        [Test]
        public void MeetsAtSameTime_FixedSamePeriodOneSharedDay_IsConflict()
        {
            var target = Fixed(1, "A", "A1", 35, "Tuesday|Thursday");
            var existing = Fixed(2, "B", "B1", 35, "Monday|Wednesday|Thursday");

            Assert.IsTrue(StudentScheduleConflict.MeetsAtSameTime(target, existing));
        }

        [Test]
        public void MeetsAtSameTime_FixedSamePeriodDisjointDays_IsNotConflict()
        {
            var target = Fixed(1, "A", "A1", 35, "Tuesday|Thursday");
            var existing = Fixed(2, "B", "B1", 35, "Monday|Wednesday|Friday");

            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(target, existing));
        }

        [Test]
        public void MeetsAtSameTime_FixedDifferentPeriod_IsNotConflict()
        {
            var target = Fixed(1, "A", "A1", 35, MonToFri);
            var existing = Fixed(2, "B", "B1", 36, MonToFri);

            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(target, existing));
        }

        [Test]
        public void MeetsAtSameTime_FixedTargetAgainstVariableExisting_MatchesOnDay()
        {
            var target = Fixed(1, "A", "A1", 35, "Monday|Wednesday|Friday");

            Assert.IsTrue(StudentScheduleConflict.MeetsAtSameTime(target, Variable(2, 35, "Wednesday")));
            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(target, Variable(2, 35, "Tuesday")));
        }

        [Test]
        public void MeetsAtSameTime_VariableTargetAgainstFixedExisting_MatchesOnDay()
        {
            var existing = Fixed(2, "B", "B1", 35, "Monday|Wednesday|Friday");

            Assert.IsTrue(StudentScheduleConflict.MeetsAtSameTime(Variable(1, 35, "Friday"), existing));
            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(Variable(1, 35, "Thursday"), existing));
        }

        [Test]
        public void MeetsAtSameTime_Block_RequiresSameBlockPeriodAndRoom()
        {
            var target = Block(1, 5, 43, 22);

            Assert.IsTrue(StudentScheduleConflict.MeetsAtSameTime(target, Block(2, 5, 43, 22)));
            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(target, Block(2, 5, 43, 23)));
            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(target, Block(2, 6, 43, 22)));
        }

        [Test]
        public void MeetsAtSameTime_NoPeriodOnTarget_IsNotConflict()
        {
            var target = new AllCourseSectionView { CourseSectionId = 1 };
            var existing = Fixed(2, "B", "B1", 35, MonToFri);

            Assert.IsFalse(StudentScheduleConflict.MeetsAtSameTime(target, existing));
        }

        [Test]
        public void BuildPeriodConflictComment_NamesSectionPeriodDaysAndRoom()
        {
            var existing = Fixed(136, "G1 English Reading", "English Reading G1", 35, MonToFri, 17);

            var comment = StudentScheduleConflict.BuildPeriodConflictComment(new[] { existing }, Periods, Rooms);

            Assert.AreEqual("Period conflict with English Reading G1 (1-2 Period 1, Mon-Fri, Room#1- G1)", comment);
        }

        [Test]
        public void BuildPeriodConflictComment_ListsEachConflictingSectionOnce()
        {
            var reading = Fixed(136, "G1 English Reading", "English Reading G1", 35, MonToFri, 17);
            var readingAgain = Fixed(136, "G1 English Reading", "English Reading G1", 35, MonToFri, 17);
            var speaking = Fixed(144, "G1 Listening & Speaking", "listening and Speaking", 36, "Tuesday|Thursday", 17);

            var comment = StudentScheduleConflict.BuildPeriodConflictComment(new[] { reading, readingAgain, speaking }, Periods, Rooms);

            Assert.AreEqual(
                "Period conflict with English Reading G1 (1-2 Period 1, Mon-Fri, Room#1- G1); " +
                "listening and Speaking (1-2 Period 2, Tue, Thu, Room#1- G1)",
                comment);
        }

        [Test]
        public void BuildPeriodConflictComment_FallsBackToIdsWhenTitlesUnknown()
        {
            var existing = Fixed(1, "Science", "Sci A", 99, "Friday", 77);

            var comment = StudentScheduleConflict.BuildPeriodConflictComment(new[] { existing }, Periods, Rooms);

            Assert.AreEqual("Period conflict with Sci A (Period 99, Fri, Room 77)", comment);
        }

        [Test]
        public void BuildPeriodConflictComment_UsesCourseTitleWhenSectionNameIsBlank()
        {
            var existing = Fixed(1, "G1 Health", " ", 35, MonToFri, 17);

            var comment = StudentScheduleConflict.BuildPeriodConflictComment(new[] { existing }, Periods, Rooms);

            Assert.AreEqual("Period conflict with G1 Health (1-2 Period 1, Mon-Fri, Room#1- G1)", comment);
        }

        [Test]
        public void BuildPeriodConflictComment_WithoutConflictRows_KeepsGenericMessage()
        {
            var comment = StudentScheduleConflict.BuildPeriodConflictComment(Enumerable.Empty<AllCourseSectionView>(), Periods, Rooms);

            Assert.AreEqual("There is a period conflict", comment);
        }

        [Test]
        public void BuildPeriodConflictComment_FitsTheColumn()
        {
            var many = Enumerable.Range(1, 20)
                .Select(i => Fixed(i, "Course " + i, "A rather long section name number " + i, 35, MonToFri, 17))
                .ToList();

            var comment = StudentScheduleConflict.BuildPeriodConflictComment(many, Periods, Rooms);

            Assert.AreEqual(StudentScheduleConflict.MaxCommentLength, comment.Length);
            StringAssert.EndsWith("...", comment);
        }

        [Test]
        public void BuildPeriodConflictComment_NeverContainsTheReportSeparator()
        {
            // The report joins "Scheduled | comment" and the UI splits on " | ".
            var existing = Fixed(1, "A", "A1", 35, MonToFri, 17);

            var comment = StudentScheduleConflict.BuildPeriodConflictComment(new[] { existing }, Periods, Rooms);

            StringAssert.DoesNotContain(" | ", comment);
        }

        [TestCase(MonToFri, "Mon-Fri")]
        [TestCase("Friday|Thursday|Wednesday|Tuesday|Monday", "Mon-Fri")]
        [TestCase("Tuesday|Thursday", "Tue, Thu")]
        [TestCase("Friday", "Fri")]
        [TestCase("Monday|Tuesday|Wednesday|Thursday", "Mon, Tue, Wed, Thu")]
        [TestCase("", null)]
        [TestCase(null, null)]
        public void ShortDays_AbbreviatesInWeekOrder(string? days, string? expected)
        {
            Assert.AreEqual(expected, StudentScheduleConflict.ShortDays(days));
        }
    }
}
