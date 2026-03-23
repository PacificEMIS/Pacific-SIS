-- =============================================================================
-- Cleanup: remove duplicate missing attendance records
-- =============================================================================
-- The background job had a bug where missingSet was not updated within a run,
-- allowing duplicates when AllCourseSectionView returned multiple rows for the
-- same (SchoolId, CourseSectionId, PeriodId, Date).
--
-- This script keeps the record with the lowest MissingAttendanceId for each
-- unique combination and deletes the rest.
-- =============================================================================

-- Step 1: Preview — count duplicates per school
SELECT
    sma.school_id,
    COUNT(*) AS duplicate_rows
FROM student_missing_attendance sma
INNER JOIN (
    SELECT
        tenant_id,
        school_id,
        course_section_id,
        period_id,
        missing_attendance_date,
        MIN(missing_attendance_id) AS keep_id
    FROM student_missing_attendance
    GROUP BY tenant_id, school_id, course_section_id, period_id, missing_attendance_date
    HAVING COUNT(*) > 1
) dups
    ON sma.tenant_id = dups.tenant_id
    AND sma.school_id = dups.school_id
    AND sma.course_section_id = dups.course_section_id
    AND sma.period_id <=> dups.period_id
    AND sma.missing_attendance_date <=> dups.missing_attendance_date
    AND sma.missing_attendance_id != dups.keep_id
GROUP BY sma.school_id;

-- Step 2: Delete duplicates (keep lowest missing_attendance_id per group)
-- Uncomment when ready to run after verifying the preview above.
/*
DELETE sma
FROM student_missing_attendance sma
INNER JOIN (
    SELECT
        tenant_id,
        school_id,
        course_section_id,
        period_id,
        missing_attendance_date,
        MIN(missing_attendance_id) AS keep_id
    FROM student_missing_attendance
    GROUP BY tenant_id, school_id, course_section_id, period_id, missing_attendance_date
    HAVING COUNT(*) > 1
) dups
    ON sma.tenant_id = dups.tenant_id
    AND sma.school_id = dups.school_id
    AND sma.course_section_id = dups.course_section_id
    AND sma.period_id <=> dups.period_id
    AND sma.missing_attendance_date <=> dups.missing_attendance_date
    AND sma.missing_attendance_id != dups.keep_id;
*/
