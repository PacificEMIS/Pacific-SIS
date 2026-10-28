using System;
using System.Collections.Generic;

namespace opensis.data.ViewModels.CourseManager
{
    /// <summary>
    /// Request and result of copying the course sections of one academic year into
    /// another. Every section is reported, either as cloned or as skipped with a reason.
    /// </summary>
    public class CloneCourseSectionsViewModel : CommonFields
    {
        public Guid TenantId { get; set; }
        public int SchoolId { get; set; }

        /// <summary>Academic year the sections are copied from.</summary>
        public decimal? SourceAcademicYear { get; set; }

        /// <summary>Academic year the sections are created in.</summary>
        public decimal? TargetAcademicYear { get; set; }

        /// <summary>
        /// Optional course in the target year. When set, only sections of the source
        /// course with the same title are cloned.
        /// </summary>
        public int? CourseId { get; set; }

        /// <summary>Also assign the teachers who taught each source section.</summary>
        public bool IncludeStaff { get; set; } = true;

        /// <summary>Report what would happen without saving anything.</summary>
        public bool DryRun { get; set; }

        public string? CreatedBy { get; set; }

        public int ClonedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<CourseSectionCloneResult> Results { get; set; } = new();
    }

    public class CourseSectionCloneResult
    {
        public string? CourseTitle { get; set; }
        public string? CourseSectionName { get; set; }
        public int SourceCourseSectionId { get; set; }
        public int? NewCourseSectionId { get; set; }
        public bool Cloned { get; set; }

        /// <summary>Why the section was skipped; null when cloned.</summary>
        public string? Reason { get; set; }

        /// <summary>References that could not be carried over exactly.</summary>
        public List<string> Warnings { get; set; } = new();
    }
}
