-- Verification queries for Lewetik Elementary School after cleanup
-- Run against the tenant DB

SET @sid = (SELECT school_id FROM school_master WHERE school_name LIKE '%Lewetik%' LIMIT 1);

-- 1. School info
SELECT school_id, school_name FROM school_master WHERE school_id = @sid;

-- 2. Marking period hierarchy
SELECT 'YEAR' AS level, sy.marking_period_id, sy.title, sy.start_date, sy.end_date
FROM school_years sy WHERE sy.school_id = @sid
UNION ALL
SELECT 'SEMESTER', s.marking_period_id, s.title, s.start_date, s.end_date
FROM semesters s WHERE s.school_id = @sid
UNION ALL
SELECT 'QUARTER', q.marking_period_id, q.title, q.start_date, q.end_date
FROM quarters q WHERE q.school_id = @sid
UNION ALL
SELECT 'PROGRESS', pp.marking_period_id, pp.title, pp.start_date, pp.end_date
FROM progress_periods pp WHERE pp.school_id = @sid
ORDER BY start_date, level;

-- 3. Remaining missing attendance — dates should all be within quarter ranges
SELECT
    sma.missing_attendance_date,
    sma.course_section_id,
    cs.course_section_name,
    CASE
        WHEN cs.yr_marking_period_id IS NOT NULL THEN 'Year'
        WHEN cs.smstr_marking_period_id IS NOT NULL THEN 'Semester'
        WHEN cs.qtr_marking_period_id IS NOT NULL THEN 'Quarter'
        WHEN cs.prgrsprd_marking_period_id IS NOT NULL THEN 'ProgressPeriod'
        ELSE 'None'
    END AS mp_level
FROM student_missing_attendance sma
JOIN course_section cs
    ON cs.tenant_id = sma.tenant_id AND cs.school_id = sma.school_id
    AND cs.course_section_id = sma.course_section_id
WHERE sma.school_id = @sid
ORDER BY sma.missing_attendance_date;

-- 4a. Missing attendance NOT in any quarter range (should be 0)
SELECT COUNT(*) AS missing_outside_quarters
FROM student_missing_attendance sma
WHERE sma.school_id = @sid
    AND NOT EXISTS (
        SELECT 1 FROM quarters q
        WHERE q.school_id = sma.school_id AND q.tenant_id = sma.tenant_id
        AND sma.missing_attendance_date >= q.start_date
        AND sma.missing_attendance_date <= q.end_date
    );

-- 4b. Actual attendance NOT in any quarter range (should be 0)
SELECT COUNT(*) AS attendance_outside_quarters
FROM student_attendance sa
WHERE sa.school_id = @sid
    AND NOT EXISTS (
        SELECT 1 FROM quarters q
        WHERE q.school_id = sa.school_id AND q.tenant_id = sa.tenant_id
        AND sa.attendance_date >= q.start_date
        AND sa.attendance_date <= q.end_date
    );

-- 5. Remaining totals
SELECT 'missing_attendance' AS record_type, COUNT(*) AS total
FROM student_missing_attendance WHERE school_id = @sid
UNION ALL
SELECT 'actual_attendance', COUNT(*)
FROM student_attendance WHERE school_id = @sid;

-- 6. Attendance by course section (students x days = records)
SELECT
    sa.course_section_id,
    cs.course_section_name,
    COUNT(*) AS records,
    COUNT(DISTINCT sa.student_id) AS students,
    COUNT(DISTINCT sa.attendance_date) AS days
FROM student_attendance sa
JOIN course_section cs
    ON cs.tenant_id = sa.tenant_id AND cs.school_id = sa.school_id
    AND cs.course_section_id = sa.course_section_id
WHERE sa.school_id = @sid
GROUP BY sa.course_section_id, cs.course_section_name
ORDER BY records DESC;

-- 7. Attendance by month (spot-check: break months should have no records)
SELECT
    YEAR(sa.attendance_date) AS yr,
    MONTH(sa.attendance_date) AS mo,
    COUNT(*) AS records,
    COUNT(DISTINCT sa.student_id) AS students
FROM student_attendance sa
WHERE sa.school_id = @sid
GROUP BY YEAR(sa.attendance_date), MONTH(sa.attendance_date)
ORDER BY yr, mo;

-- 8. Quick math: students x sections x days
SELECT
    COUNT(DISTINCT sa.student_id) AS total_students,
    COUNT(DISTINCT sa.course_section_id) AS total_sections,
    COUNT(DISTINCT sa.attendance_date) AS total_days,
    COUNT(*) AS total_records
FROM student_attendance sa
WHERE sa.school_id = @sid;
