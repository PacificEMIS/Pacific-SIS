    -- =============================================================================
    -- Cleanup: attendance records outside marking period date ranges
    --
    -- Run against each tenant database. Run the SELECT queries first to review
    -- what will be affected before executing the DELETE.
    --
    -- Logic: For each course section, resolve its marking period assignment down
    -- to the leaf level (Year -> Semesters -> Quarters -> Progress Periods).
    -- A date is "outside" if it doesn't fall within any leaf-level range.
    -- =============================================================================

    -- Step 1: Build a temp table of valid date ranges per course section.
    -- Each row = one leaf-level marking period's (start_date, end_date).

    DROP TABLE IF EXISTS _cleanup_valid_ranges;

    CREATE TABLE _cleanup_valid_ranges (
        school_id INT,
        course_section_id INT,
        range_start DATE,
        range_end DATE,
        INDEX idx_lookup (school_id, course_section_id)
    );

    -- Case A: Course section assigned to progress_period level — it IS the leaf.
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, pp.start_date, pp.end_date
    FROM course_section cs
    JOIN progress_periods pp
        ON pp.tenant_id = cs.tenant_id AND pp.school_id = cs.school_id
        AND pp.marking_period_id = cs.prgrsprd_marking_period_id
    WHERE cs.prgrsprd_marking_period_id IS NOT NULL
        AND pp.start_date IS NOT NULL AND pp.end_date IS NOT NULL;

    -- Case B: Course section assigned to quarter level.
    -- Use child progress periods if they exist, otherwise the quarter itself.

    -- B1: Quarters that HAVE child progress periods
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, pp.start_date, pp.end_date
    FROM course_section cs
    JOIN progress_periods pp
        ON pp.tenant_id = cs.tenant_id AND pp.school_id = cs.school_id
        AND pp.quarter_id = cs.qtr_marking_period_id
    WHERE cs.qtr_marking_period_id IS NOT NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND pp.start_date IS NOT NULL AND pp.end_date IS NOT NULL;

    -- B2: Quarters with NO child progress periods — use the quarter's own dates
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, q.start_date, q.end_date
    FROM course_section cs
    JOIN quarters q
        ON q.tenant_id = cs.tenant_id AND q.school_id = cs.school_id
        AND q.marking_period_id = cs.qtr_marking_period_id
    WHERE cs.qtr_marking_period_id IS NOT NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND q.start_date IS NOT NULL AND q.end_date IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM progress_periods pp2
            WHERE pp2.tenant_id = cs.tenant_id AND pp2.school_id = cs.school_id
            AND pp2.quarter_id = cs.qtr_marking_period_id
        );

    -- Case C: Course section assigned to semester level.
    -- Walk down: semester -> quarters -> progress periods.

    -- C1: Semesters whose quarters have child progress periods
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, pp.start_date, pp.end_date
    FROM course_section cs
    JOIN quarters q
        ON q.tenant_id = cs.tenant_id AND q.school_id = cs.school_id
        AND q.semester_id = cs.smstr_marking_period_id
    JOIN progress_periods pp
        ON pp.tenant_id = cs.tenant_id AND pp.school_id = cs.school_id
        AND pp.quarter_id = q.marking_period_id
    WHERE cs.smstr_marking_period_id IS NOT NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND pp.start_date IS NOT NULL AND pp.end_date IS NOT NULL;

    -- C2: Semesters whose quarters have NO child progress periods — use quarter dates
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, q.start_date, q.end_date
    FROM course_section cs
    JOIN quarters q
        ON q.tenant_id = cs.tenant_id AND q.school_id = cs.school_id
        AND q.semester_id = cs.smstr_marking_period_id
    WHERE cs.smstr_marking_period_id IS NOT NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND q.start_date IS NOT NULL AND q.end_date IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM progress_periods pp3
            WHERE pp3.tenant_id = cs.tenant_id AND pp3.school_id = cs.school_id
            AND pp3.quarter_id = q.marking_period_id
        );

    -- C3: Semesters with NO child quarters — use the semester's own dates
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, s.start_date, s.end_date
    FROM course_section cs
    JOIN semesters s
        ON s.tenant_id = cs.tenant_id AND s.school_id = cs.school_id
        AND s.marking_period_id = cs.smstr_marking_period_id
    WHERE cs.smstr_marking_period_id IS NOT NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND s.start_date IS NOT NULL AND s.end_date IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM quarters q3
            WHERE q3.tenant_id = cs.tenant_id AND q3.school_id = cs.school_id
            AND q3.semester_id = cs.smstr_marking_period_id
        );

    -- Case D: Course section assigned to year level.
    -- Walk down: year -> semesters -> quarters -> progress periods.

    -- D1: Years whose semesters' quarters have child progress periods
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, pp.start_date, pp.end_date
    FROM course_section cs
    JOIN semesters s
        ON s.tenant_id = cs.tenant_id AND s.school_id = cs.school_id
        AND s.year_id = cs.yr_marking_period_id
    JOIN quarters q
        ON q.tenant_id = cs.tenant_id AND q.school_id = cs.school_id
        AND q.semester_id = s.marking_period_id
    JOIN progress_periods pp
        ON pp.tenant_id = cs.tenant_id AND pp.school_id = cs.school_id
        AND pp.quarter_id = q.marking_period_id
    WHERE cs.yr_marking_period_id IS NOT NULL
        AND cs.smstr_marking_period_id IS NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND pp.start_date IS NOT NULL AND pp.end_date IS NOT NULL;

    -- D2: Years whose semesters have quarters but NO progress periods — use quarter dates
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, q.start_date, q.end_date
    FROM course_section cs
    JOIN semesters s
        ON s.tenant_id = cs.tenant_id AND s.school_id = cs.school_id
        AND s.year_id = cs.yr_marking_period_id
    JOIN quarters q
        ON q.tenant_id = cs.tenant_id AND q.school_id = cs.school_id
        AND q.semester_id = s.marking_period_id
    WHERE cs.yr_marking_period_id IS NOT NULL
        AND cs.smstr_marking_period_id IS NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND q.start_date IS NOT NULL AND q.end_date IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM progress_periods pp4
            WHERE pp4.tenant_id = cs.tenant_id AND pp4.school_id = cs.school_id
            AND pp4.quarter_id = q.marking_period_id
        );

    -- D3: Years whose semesters have NO quarters — use semester dates
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, s.start_date, s.end_date
    FROM course_section cs
    JOIN semesters s
        ON s.tenant_id = cs.tenant_id AND s.school_id = cs.school_id
        AND s.year_id = cs.yr_marking_period_id
    WHERE cs.yr_marking_period_id IS NOT NULL
        AND cs.smstr_marking_period_id IS NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND s.start_date IS NOT NULL AND s.end_date IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM quarters q4
            WHERE q4.tenant_id = cs.tenant_id AND q4.school_id = cs.school_id
            AND q4.semester_id = s.marking_period_id
        );

    -- D4: Years with NO child semesters — use the year's own dates
    INSERT INTO _cleanup_valid_ranges (school_id, course_section_id, range_start, range_end)
    SELECT cs.school_id, cs.course_section_id, sy.start_date, sy.end_date
    FROM course_section cs
    JOIN school_years sy
        ON sy.tenant_id = cs.tenant_id AND sy.school_id = cs.school_id
        AND sy.marking_period_id = cs.yr_marking_period_id
    WHERE cs.yr_marking_period_id IS NOT NULL
        AND cs.smstr_marking_period_id IS NULL
        AND cs.qtr_marking_period_id IS NULL
        AND cs.prgrsprd_marking_period_id IS NULL
        AND sy.start_date IS NOT NULL AND sy.end_date IS NOT NULL
        AND NOT EXISTS (
            SELECT 1 FROM semesters s4
            WHERE s4.tenant_id = cs.tenant_id AND s4.school_id = cs.school_id
            AND s4.year_id = cs.yr_marking_period_id
        );

    -- =============================================================================
    -- Step 2: REVIEW — see what would be affected before deleting anything.
    -- =============================================================================

    -- 2a: Missing attendance records outside marking periods (will be deleted)
    SELECT
        sma.school_id,
        sma.course_section_id,
        sma.missing_attendance_date,
        sma.staff_id,
        sma.missing_attendance_id
    FROM student_missing_attendance sma
    JOIN course_section cs
        ON cs.tenant_id = sma.tenant_id AND cs.school_id = sma.school_id
        AND cs.course_section_id = sma.course_section_id
    WHERE sma.course_section_id IS NOT NULL
        -- Course section has a marking period assigned
        AND (cs.yr_marking_period_id IS NOT NULL
            OR cs.smstr_marking_period_id IS NOT NULL
            OR cs.qtr_marking_period_id IS NOT NULL
            OR cs.prgrsprd_marking_period_id IS NOT NULL)
        -- Has valid ranges resolved
        AND EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sma.school_id AND vr.course_section_id = sma.course_section_id
        )
        -- Date does NOT fall within any valid range
        AND NOT EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sma.school_id AND vr.course_section_id = sma.course_section_id
            AND sma.missing_attendance_date >= vr.range_start
            AND sma.missing_attendance_date <= vr.range_end
        )
    ORDER BY sma.school_id, sma.course_section_id, sma.missing_attendance_date;

    -- 2b: Actual attendance records outside marking periods (for MANUAL REVIEW only)
    SELECT
        sa.school_id,
        sa.course_section_id,
        sa.attendance_date,
        sa.student_id,
        sa.staff_id,
        sa.attendance_code
    FROM student_attendance sa
    JOIN course_section cs
        ON cs.tenant_id = sa.tenant_id AND cs.school_id = sa.school_id
        AND cs.course_section_id = sa.course_section_id
    WHERE (cs.yr_marking_period_id IS NOT NULL
            OR cs.smstr_marking_period_id IS NOT NULL
            OR cs.qtr_marking_period_id IS NOT NULL
            OR cs.prgrsprd_marking_period_id IS NOT NULL)
        AND EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sa.school_id AND vr.course_section_id = sa.course_section_id
        )
        AND NOT EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sa.school_id AND vr.course_section_id = sa.course_section_id
            AND sa.attendance_date >= vr.range_start
            AND sa.attendance_date <= vr.range_end
        )
    ORDER BY sa.school_id, sa.course_section_id, sa.attendance_date;

    -- =============================================================================
    -- Step 3: DELETE records outside marking periods.
    --
    -- Strategy: collect PKs into helper tables first, then delete by PK.
    -- This avoids timeouts from complex JOINs + subqueries during DELETE.
    -- =============================================================================

    -- 3a: Collect missing attendance PKs to delete
    DROP TABLE IF EXISTS _cleanup_delete_missing;
    CREATE TABLE _cleanup_delete_missing (
        tenant_id CHAR(36),
        school_id INT,
        missing_attendance_id INT,
        PRIMARY KEY (tenant_id, school_id, missing_attendance_id)
    );

    INSERT INTO _cleanup_delete_missing (tenant_id, school_id, missing_attendance_id)
    SELECT DISTINCT sma.tenant_id, sma.school_id, sma.missing_attendance_id
    FROM student_missing_attendance sma
    JOIN course_section cs
        ON cs.tenant_id = sma.tenant_id AND cs.school_id = sma.school_id
        AND cs.course_section_id = sma.course_section_id
    WHERE sma.course_section_id IS NOT NULL
        AND (cs.yr_marking_period_id IS NOT NULL
            OR cs.smstr_marking_period_id IS NOT NULL
            OR cs.qtr_marking_period_id IS NOT NULL
            OR cs.prgrsprd_marking_period_id IS NOT NULL)
        AND EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sma.school_id AND vr.course_section_id = sma.course_section_id
        )
        AND NOT EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sma.school_id AND vr.course_section_id = sma.course_section_id
            AND sma.missing_attendance_date >= vr.range_start
            AND sma.missing_attendance_date <= vr.range_end
        );

    SELECT COUNT(*) AS missing_attendance_to_delete FROM _cleanup_delete_missing;

    -- DELETE by PK
    DELETE sma
    FROM student_missing_attendance sma
    JOIN _cleanup_delete_missing d
        ON d.tenant_id = sma.tenant_id AND d.school_id = sma.school_id
        AND d.missing_attendance_id = sma.missing_attendance_id;

    DROP TABLE IF EXISTS _cleanup_delete_missing;

    -- 3b: Collect actual attendance PKs to delete
    DROP TABLE IF EXISTS _cleanup_delete_attendance;
    CREATE TABLE _cleanup_delete_attendance (
        tenant_id CHAR(36),
        school_id INT,
        student_id INT,
        staff_id INT,
        course_id INT,
        course_section_id INT,
        attendance_date DATE,
        block_id INT,
        period_id INT,
        PRIMARY KEY (tenant_id, school_id, student_id, staff_id, course_id, course_section_id, attendance_date, block_id, period_id)
    );

    INSERT INTO _cleanup_delete_attendance
        (tenant_id, school_id, student_id, staff_id, course_id, course_section_id, attendance_date, block_id, period_id)
    SELECT DISTINCT sa.tenant_id, sa.school_id, sa.student_id, sa.staff_id, sa.course_id,
        sa.course_section_id, sa.attendance_date, sa.block_id, sa.period_id
    FROM student_attendance sa
    JOIN course_section cs
        ON cs.tenant_id = sa.tenant_id AND cs.school_id = sa.school_id
        AND cs.course_section_id = sa.course_section_id
    WHERE (cs.yr_marking_period_id IS NOT NULL
            OR cs.smstr_marking_period_id IS NOT NULL
            OR cs.qtr_marking_period_id IS NOT NULL
            OR cs.prgrsprd_marking_period_id IS NOT NULL)
        AND EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sa.school_id AND vr.course_section_id = sa.course_section_id
        )
        AND NOT EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sa.school_id AND vr.course_section_id = sa.course_section_id
            AND sa.attendance_date >= vr.range_start
            AND sa.attendance_date <= vr.range_end
        );

    SELECT COUNT(*) AS attendance_to_delete FROM _cleanup_delete_attendance;

    -- Review: show only non-null/non-empty comments on attendance to be deleted
    SELECT
        sac.school_id,
        sac.student_id,
        sa.attendance_date,
        sa.course_section_id,
        sa.attendance_code,
        sac.comment
    FROM student_attendance_comments sac
    JOIN student_attendance sa
        ON sa.tenant_id = sac.tenant_id AND sa.school_id = sac.school_id
        AND sa.student_id = sac.student_id AND sa.student_attendance_id = sac.student_attendance_id
    JOIN _cleanup_delete_attendance d
        ON d.tenant_id = sa.tenant_id AND d.school_id = sa.school_id
        AND d.student_id = sa.student_id AND d.staff_id = sa.staff_id
        AND d.course_id = sa.course_id AND d.course_section_id = sa.course_section_id
        AND d.attendance_date = sa.attendance_date AND d.block_id = sa.block_id
        AND d.period_id = sa.period_id
    WHERE sac.comment IS NOT NULL AND TRIM(sac.comment) != ''
    ORDER BY sa.attendance_date, sac.school_id, sac.student_id;

    -- DELETE child comments first (FK constraint)
    DELETE sac
    FROM student_attendance_comments sac
    JOIN student_attendance sa
        ON sa.tenant_id = sac.tenant_id AND sa.school_id = sac.school_id
        AND sa.student_id = sac.student_id AND sa.student_attendance_id = sac.student_attendance_id
    JOIN _cleanup_delete_attendance d
        ON d.tenant_id = sa.tenant_id AND d.school_id = sa.school_id
        AND d.student_id = sa.student_id AND d.staff_id = sa.staff_id
        AND d.course_id = sa.course_id AND d.course_section_id = sa.course_section_id
        AND d.attendance_date = sa.attendance_date AND d.block_id = sa.block_id
        AND d.period_id = sa.period_id;

    -- DELETE attendance by PK
    DELETE sa
    FROM student_attendance sa
    JOIN _cleanup_delete_attendance d
        ON d.tenant_id = sa.tenant_id AND d.school_id = sa.school_id
        AND d.student_id = sa.student_id AND d.staff_id = sa.staff_id
        AND d.course_id = sa.course_id AND d.course_section_id = sa.course_section_id
        AND d.attendance_date = sa.attendance_date AND d.block_id = sa.block_id
        AND d.period_id = sa.period_id;

    DROP TABLE IF EXISTS _cleanup_delete_attendance;

    -- =============================================================================
    -- Step 4: Verify — both should return 0 after deletes.
    -- =============================================================================

    SELECT COUNT(*) AS remaining_missing_outside_mp FROM student_missing_attendance sma
    JOIN course_section cs
        ON cs.tenant_id = sma.tenant_id AND cs.school_id = sma.school_id
        AND cs.course_section_id = sma.course_section_id
    WHERE sma.course_section_id IS NOT NULL
        AND (cs.yr_marking_period_id IS NOT NULL
            OR cs.smstr_marking_period_id IS NOT NULL
            OR cs.qtr_marking_period_id IS NOT NULL
            OR cs.prgrsprd_marking_period_id IS NOT NULL)
        AND EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sma.school_id AND vr.course_section_id = sma.course_section_id
        )
        AND NOT EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sma.school_id AND vr.course_section_id = sma.course_section_id
            AND sma.missing_attendance_date >= vr.range_start
            AND sma.missing_attendance_date <= vr.range_end
        );

    SELECT COUNT(*) AS remaining_attendance_outside_mp FROM student_attendance sa
    JOIN course_section cs
        ON cs.tenant_id = sa.tenant_id AND cs.school_id = sa.school_id
        AND cs.course_section_id = sa.course_section_id
    WHERE (cs.yr_marking_period_id IS NOT NULL
            OR cs.smstr_marking_period_id IS NOT NULL
            OR cs.qtr_marking_period_id IS NOT NULL
            OR cs.prgrsprd_marking_period_id IS NOT NULL)
        AND EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sa.school_id AND vr.course_section_id = sa.course_section_id
        )
        AND NOT EXISTS (
            SELECT 1 FROM _cleanup_valid_ranges vr
            WHERE vr.school_id = sa.school_id AND vr.course_section_id = sa.course_section_id
            AND sa.attendance_date >= vr.range_start
            AND sa.attendance_date <= vr.range_end
        );

    DROP TABLE IF EXISTS _cleanup_valid_ranges;

    -- =============================================================================
    -- Step 5: Post-cleanup summary — sanity check remaining data.
    -- =============================================================================

    -- Remaining totals
    SELECT 'missing_attendance' AS record_type, COUNT(*) AS total
    FROM student_missing_attendance
    UNION ALL
    SELECT 'actual_attendance', COUNT(*)
    FROM student_attendance;

    -- Remaining attendance by school, course section (students x days = records)
    SELECT
        sa.school_id,
        sm.school_name,
        sa.course_section_id,
        cs.course_section_name,
        COUNT(*) AS records,
        COUNT(DISTINCT sa.student_id) AS students,
        COUNT(DISTINCT sa.attendance_date) AS days
    FROM student_attendance sa
    JOIN course_section cs
        ON cs.tenant_id = sa.tenant_id AND cs.school_id = sa.school_id
        AND cs.course_section_id = sa.course_section_id
    JOIN school_master sm
        ON sm.tenant_id = sa.tenant_id AND sm.school_id = sa.school_id
    GROUP BY sa.school_id, sm.school_name, sa.course_section_id, cs.course_section_name
    ORDER BY sa.school_id, sa.course_section_id;

    -- Remaining attendance by month (spot-check: no records in break months)
    SELECT
        YEAR(sa.attendance_date) AS yr,
        MONTH(sa.attendance_date) AS mo,
        COUNT(*) AS records,
        COUNT(DISTINCT sa.student_id) AS students
    FROM student_attendance sa
    GROUP BY YEAR(sa.attendance_date), MONTH(sa.attendance_date)
    ORDER BY yr, mo;

    -- Remaining missing attendance by school
    SELECT
        sma.school_id,
        sm.school_name,
        COUNT(*) AS missing_records,
        MIN(sma.missing_attendance_date) AS earliest,
        MAX(sma.missing_attendance_date) AS latest
    FROM student_missing_attendance sma
    JOIN school_master sm
        ON sm.tenant_id = sma.tenant_id AND sm.school_id = sma.school_id
    GROUP BY sma.school_id, sm.school_name
    ORDER BY sma.school_id;
