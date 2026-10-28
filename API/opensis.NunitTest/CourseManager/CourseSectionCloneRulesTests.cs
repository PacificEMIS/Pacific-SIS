using System;
using NUnit.Framework;
using opensis.data.Helper;
using static opensis.data.Helper.CourseSectionClone;

namespace opensis.NunitTest.CourseManager
{
    /// <summary>
    /// Covers the EF-free rules used when cloning course sections across academic years.
    /// </summary>
    [TestFixture]
    public class CourseSectionCloneRulesTests
    {
        private static readonly DateTime Start2024 = new(2024, 1, 15), End2024 = new(2024, 12, 10);
        private static readonly DateTime Start2025 = new(2025, 1, 13), End2025 = new(2025, 12, 8);

        [TestCase("Fixed Schedule (1)", ScheduleKind.Fixed)]
        [TestCase("fixedschedule", ScheduleKind.Fixed)]
        [TestCase("Variable Schedule (2)", ScheduleKind.Variable)]
        [TestCase("variableschedule", ScheduleKind.Variable)]
        [TestCase("Calendar Schedule (3)", ScheduleKind.Calendar)]
        [TestCase("calendarschedule", ScheduleKind.Calendar)]
        [TestCase("Block Schedule (4)", ScheduleKind.Block)]
        [TestCase("blockschedule", ScheduleKind.Block)]
        [TestCase("", ScheduleKind.Unknown)]
        [TestCase(null, ScheduleKind.Unknown)]
        public void GetScheduleKind_ReadsStoredAndPostedForms(string? stored, ScheduleKind expected)
        {
            Assert.AreEqual(expected, GetScheduleKind(stored));
        }

        [Test]
        public void SameTitle_IgnoresCaseAndSurroundingSpaces()
        {
            Assert.IsTrue(SameTitle(" Room A", "room a "));
            Assert.IsFalse(SameTitle("Room A", "Room B"));
            Assert.IsFalse(SameTitle("", ""), "blank titles never match");
            Assert.IsFalse(SameTitle(null, null));
        }

        [Test]
        public void DaysOverlap_NeedsOneSharedDay()
        {
            Assert.IsTrue(DaysOverlap("Monday|Wednesday|Friday", "Friday"));
            Assert.IsTrue(DaysOverlap("monday", "Monday|Tuesday"));
            Assert.IsFalse(DaysOverlap("Monday|Wednesday", "Tuesday|Thursday"));
            Assert.IsFalse(DaysOverlap("", "Monday"));
            Assert.IsFalse(DaysOverlap(null, null));
        }

        [Test]
        public void DurationsOverlap_TreatsMissingBoundsAsOpen()
        {
            Assert.IsTrue(DurationsOverlap(Start2025, End2025, new DateTime(2025, 6, 1), new DateTime(2025, 6, 30)));
            Assert.IsTrue(DurationsOverlap(Start2025, new DateTime(2025, 6, 1), new DateTime(2025, 6, 1), End2025), "touching ranges overlap");
            Assert.IsFalse(DurationsOverlap(Start2025, new DateTime(2025, 5, 31), new DateTime(2025, 6, 1), End2025));
            Assert.IsTrue(DurationsOverlap(null, null, Start2025, End2025));
            Assert.IsTrue(DurationsOverlap(Start2025, null, new DateTime(2025, 6, 1), null));
        }

        [Test]
        public void MapDuration_WholeCalendarSectionFollowsTheNewCalendar()
        {
            var (start, end, adjusted) = MapDuration(Start2024, End2024, Start2024, End2024, Start2025, End2025, 1);

            Assert.AreEqual(Start2025, start);
            Assert.AreEqual(End2025, end);
            Assert.IsFalse(adjusted);
        }

        [Test]
        public void MapDuration_PartialRangeMovesForwardByWholeYears()
        {
            var (start, end, adjusted) = MapDuration(new DateTime(2024, 2, 5), new DateTime(2024, 6, 28), Start2024, End2024, Start2025, End2025, 1);

            Assert.AreEqual(new DateTime(2025, 2, 5), start);
            Assert.AreEqual(new DateTime(2025, 6, 28), end);
            Assert.IsFalse(adjusted);
        }

        [Test]
        public void MapDuration_SkippingAYearMovesByTwo()
        {
            var (start, _, _) = MapDuration(new DateTime(2023, 2, 5), new DateTime(2023, 6, 28), new DateTime(2023, 1, 16), new DateTime(2023, 12, 11), Start2025, End2025, 2);
            Assert.AreEqual(new DateTime(2025, 2, 5), start);
        }

        [Test]
        public void MapDuration_ClipsToTheNewCalendarAndSaysSo()
        {
            var (start, end, adjusted) = MapDuration(new DateTime(2024, 1, 2), new DateTime(2024, 12, 20), Start2024, End2024, Start2025, End2025, 1);

            Assert.AreEqual(Start2025, start, "2025-01-02 is before the calendar opens");
            Assert.AreEqual(End2025, end, "2025-12-20 is after the calendar closes");
            Assert.IsTrue(adjusted);
        }

        [Test]
        public void MapDuration_RangeEntirelyOutsideCalendarFallsBackToCalendar()
        {
            var (start, end, adjusted) = MapDuration(new DateTime(2024, 12, 15), new DateTime(2024, 12, 20), Start2024, End2024, Start2025, End2025, 1);

            Assert.AreEqual(Start2025, start);
            Assert.AreEqual(End2025, end);
            Assert.IsTrue(adjusted);
        }

        [Test]
        public void MapDuration_MissingDatesTakeTheCalendar()
        {
            var (start, end, adjusted) = MapDuration(null, null, Start2024, End2024, Start2025, End2025, 1);

            Assert.AreEqual(Start2025, start);
            Assert.AreEqual(End2025, end);
            Assert.IsFalse(adjusted);
        }

        [Test]
        public void MapDuration_LeapDayLandsOnTheLastDayOfFebruary()
        {
            var (start, _, _) = MapDuration(new DateTime(2024, 2, 29), new DateTime(2024, 6, 28), Start2024, End2024, Start2025, End2025, 1);
            Assert.AreEqual(new DateTime(2025, 2, 28), start);
        }

        [Test]
        public void WithinCalendar_IncludesBothBounds()
        {
            Assert.IsTrue(WithinCalendar(Start2025, Start2025, End2025));
            Assert.IsTrue(WithinCalendar(End2025, Start2025, End2025));
            Assert.IsFalse(WithinCalendar(End2025.AddDays(1), Start2025, End2025));
            Assert.IsTrue(WithinCalendar(End2025.AddDays(1), Start2025, null), "open-ended calendar");
        }
    }
}
