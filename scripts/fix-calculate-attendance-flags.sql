-- =============================================================================
-- Fix misconfigured CalculateAttendance flags and recalculate AttendanceMinutes
--
-- Run against each tenant database.
--
-- Part 1: Fix CalculateAttendance flags — if attendance has been taken on a
-- period, it should count toward daily attendance calculation.
--
-- Part 2: Recalculate StudentDailyAttendance.AttendanceMinutes using only
-- periods where CalculateAttendance = true.
--
-- Wrapped in a transaction — ROLLBACK if anything looks wrong, COMMIT when satisfied.
-- =============================================================================

START TRANSACTION;

-- =============================================================================
-- Part 1: Review and fix CalculateAttendance flags
-- =============================================================================

-- 1a: Show periods where attendance has been taken but CalculateAttendance is false,
-- including which course sections are affected
SELECT DISTINCT
    sm.school_id,
    sm.school_name,
    bp.block_id,
    bp.period_id,
    bp.period_title,
    bp.calculate_attendance,
    bp.period_start_time,
    bp.period_end_time,
    cs.course_section_id,
    cs.course_section_name
FROM block_period bp
JOIN school_master sm ON sm.tenant_id = bp.tenant_id AND sm.school_id = bp.school_id
JOIN student_attendance sa
    ON sa.tenant_id = bp.tenant_id AND sa.school_id = bp.school_id
    AND sa.block_id = bp.block_id AND sa.period_id = bp.period_id
JOIN course_section cs
    ON cs.tenant_id = sa.tenant_id AND cs.school_id = sa.school_id
    AND cs.course_section_id = sa.course_section_id
WHERE (bp.calculate_attendance = 0 OR bp.calculate_attendance IS NULL)
ORDER BY sm.school_name, bp.block_id, bp.period_id, cs.course_section_name;

-- 1b: Fix — set CalculateAttendance = true for periods with attendance data
UPDATE block_period bp
SET bp.calculate_attendance = 1
WHERE EXISTS (
    SELECT 1 FROM student_attendance sa
    WHERE sa.tenant_id = bp.tenant_id AND sa.school_id = bp.school_id
    AND sa.block_id = bp.block_id AND sa.period_id = bp.period_id
)
AND (bp.calculate_attendance = 0 OR bp.calculate_attendance IS NULL);

-- 1c: Verify — should return 0
SELECT COUNT(*) AS remaining_misconfigured
FROM block_period bp
WHERE EXISTS (
    SELECT 1 FROM student_attendance sa
    WHERE sa.tenant_id = bp.tenant_id AND sa.school_id = bp.school_id
    AND sa.block_id = bp.block_id AND sa.period_id = bp.period_id
)
AND (bp.calculate_attendance = 0 OR bp.calculate_attendance IS NULL);

-- =============================================================================
-- Part 2: Recalculate AttendanceMinutes for all StudentDailyAttendance records
--
-- Recomputes the sum of period minutes for non-absent attendance records,
-- only including periods where CalculateAttendance = true.
-- =============================================================================

-- 2a: Preview — show records that would change
SELECT
    sda.school_id,
    sda.student_id,
    sda.attendance_date,
    sda.attendance_minutes AS current_minutes,
    COALESCE(recalc.new_minutes, 0) AS new_minutes
FROM student_daily_attendance sda
LEFT JOIN (
    SELECT
        sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date,
        SUM(
            CASE
                WHEN ac.state_code != 'absent' AND bp.calculate_attendance = 1
                THEN TIMESTAMPDIFF(MINUTE,
                    CAST(CONCAT('2000-01-01 ', bp.period_start_time) AS DATETIME),
                    CAST(CONCAT('2000-01-01 ', bp.period_end_time) AS DATETIME))
                ELSE 0
            END
        ) AS new_minutes
    FROM student_attendance sa
    JOIN block_period bp
        ON bp.tenant_id = sa.tenant_id AND bp.school_id = sa.school_id
        AND bp.block_id = sa.block_id AND bp.period_id = sa.period_id
    JOIN attendance_code ac
        ON ac.tenant_id = sa.tenant_id AND ac.school_id = sa.school_id
        AND ac.attendance_code = sa.attendance_code
        AND ac.attendance_category_id = sa.attendance_category_id
    GROUP BY sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date
) recalc
    ON recalc.tenant_id = sda.tenant_id AND recalc.school_id = sda.school_id
    AND recalc.student_id = sda.student_id AND recalc.attendance_date = sda.attendance_date
WHERE sda.attendance_minutes != COALESCE(recalc.new_minutes, 0)
ORDER BY sda.school_id, sda.student_id, sda.attendance_date
LIMIT 100;

-- 2b: Count how many records would change
SELECT COUNT(*) AS records_to_update
FROM student_daily_attendance sda
LEFT JOIN (
    SELECT
        sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date,
        SUM(
            CASE
                WHEN ac.state_code != 'absent' AND bp.calculate_attendance = 1
                THEN TIMESTAMPDIFF(MINUTE,
                    CAST(CONCAT('2000-01-01 ', bp.period_start_time) AS DATETIME),
                    CAST(CONCAT('2000-01-01 ', bp.period_end_time) AS DATETIME))
                ELSE 0
            END
        ) AS new_minutes
    FROM student_attendance sa
    JOIN block_period bp
        ON bp.tenant_id = sa.tenant_id AND bp.school_id = sa.school_id
        AND bp.block_id = sa.block_id AND bp.period_id = sa.period_id
    JOIN attendance_code ac
        ON ac.tenant_id = sa.tenant_id AND ac.school_id = sa.school_id
        AND ac.attendance_code = sa.attendance_code
        AND ac.attendance_category_id = sa.attendance_category_id
    GROUP BY sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date
) recalc
    ON recalc.tenant_id = sda.tenant_id AND recalc.school_id = sda.school_id
    AND recalc.student_id = sda.student_id AND recalc.attendance_date = sda.attendance_date
WHERE sda.attendance_minutes != COALESCE(recalc.new_minutes, 0);

-- 2c: Update AttendanceMinutes
UPDATE student_daily_attendance sda
JOIN (
    SELECT
        sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date,
        SUM(
            CASE
                WHEN ac.state_code != 'absent' AND bp.calculate_attendance = 1
                THEN TIMESTAMPDIFF(MINUTE,
                    CAST(CONCAT('2000-01-01 ', bp.period_start_time) AS DATETIME),
                    CAST(CONCAT('2000-01-01 ', bp.period_end_time) AS DATETIME))
                ELSE 0
            END
        ) AS new_minutes
    FROM student_attendance sa
    JOIN block_period bp
        ON bp.tenant_id = sa.tenant_id AND bp.school_id = sa.school_id
        AND bp.block_id = sa.block_id AND bp.period_id = sa.period_id
    JOIN attendance_code ac
        ON ac.tenant_id = sa.tenant_id AND ac.school_id = sa.school_id
        AND ac.attendance_code = sa.attendance_code
        AND ac.attendance_category_id = sa.attendance_category_id
    GROUP BY sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date
) recalc
    ON recalc.tenant_id = sda.tenant_id AND recalc.school_id = sda.school_id
    AND recalc.student_id = sda.student_id AND recalc.attendance_date = sda.attendance_date
SET sda.attendance_minutes = recalc.new_minutes
WHERE sda.attendance_minutes != recalc.new_minutes;

-- 2d: Verify — should return 0
SELECT COUNT(*) AS remaining_mismatched
FROM student_daily_attendance sda
LEFT JOIN (
    SELECT
        sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date,
        SUM(
            CASE
                WHEN ac.state_code != 'absent' AND bp.calculate_attendance = 1
                THEN TIMESTAMPDIFF(MINUTE,
                    CAST(CONCAT('2000-01-01 ', bp.period_start_time) AS DATETIME),
                    CAST(CONCAT('2000-01-01 ', bp.period_end_time) AS DATETIME))
                ELSE 0
            END
        ) AS new_minutes
    FROM student_attendance sa
    JOIN block_period bp
        ON bp.tenant_id = sa.tenant_id AND bp.school_id = sa.school_id
        AND bp.block_id = sa.block_id AND bp.period_id = sa.period_id
    JOIN attendance_code ac
        ON ac.tenant_id = sa.tenant_id AND ac.school_id = sa.school_id
        AND ac.attendance_code = sa.attendance_code
        AND ac.attendance_category_id = sa.attendance_category_id
    GROUP BY sa.tenant_id, sa.school_id, sa.student_id, sa.attendance_date
) recalc
    ON recalc.tenant_id = sda.tenant_id AND recalc.school_id = sda.school_id
    AND recalc.student_id = sda.student_id AND recalc.attendance_date = sda.attendance_date
WHERE sda.attendance_minutes != COALESCE(recalc.new_minutes, 0);

-- =============================================================================
-- Review results above. If everything looks correct:
--   COMMIT;
-- If something looks wrong:
--   ROLLBACK;
-- =============================================================================
