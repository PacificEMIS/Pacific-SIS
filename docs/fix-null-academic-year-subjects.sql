-- fix-null-academic-year-subjects.sql
--
-- Background:
-- A bug in CourseManagerRepository.AddEditSubject() caused SetValues() to
-- overwrite academic_year with NULL whenever a subject was edited/renamed.
-- The code fix preserves academic_year on update (see commit for details).
--
-- This script detects and repairs orphaned subjects left behind by that bug.
-- It is tenant-agnostic and safe to run on any deployment.
--
-- Usage:
--   1. Run Step 1 (diagnostic) — if it returns no rows, stop. No damage.
--   2. Review Step 2 output to verify inferred years look correct.
--   3. Run Step 3 to apply the fix.
--   4. Re-run Step 1 to confirm zero rows.

-- ============================================================================
-- STEP 1: Detect — are there any subjects with NULL academic_year?
-- If this returns no rows, this deployment is clean. Stop here.
-- ============================================================================

SELECT s.tenant_id, s.school_id, sm.school_name,
       s.subject_id, s.subject_name, s.academic_year
FROM subject s
JOIN school_master sm ON s.tenant_id = sm.tenant_id AND s.school_id = sm.school_id
WHERE s.academic_year IS NULL
ORDER BY s.tenant_id, s.school_id, s.subject_id;

-- ============================================================================
-- STEP 2: Review — show each orphan with its inferred academic_year.
-- The year is inferred from the nearest neighbor with a known year:
--   - If both neighbors share the same year, use that (certain).
--   - If neighbors differ (boundary), use the previous subject's year.
--   - If no previous subject exists, use the next subject's year.
-- Review the output before running Step 3.
-- ============================================================================

SELECT o.school_id, sm.school_name,
       o.subject_id, o.subject_name AS orphan_name,
       prev.subject_id AS prev_id, prev.academic_year AS prev_year,
       nxt.subject_id  AS next_id, nxt.academic_year  AS next_year,
       COALESCE(prev.academic_year, nxt.academic_year) AS inferred_year
FROM subject o
JOIN school_master sm ON o.tenant_id = sm.tenant_id AND o.school_id = sm.school_id
LEFT JOIN LATERAL (
    SELECT subject_id, academic_year FROM subject s2
    WHERE s2.tenant_id = o.tenant_id AND s2.school_id = o.school_id
      AND s2.subject_id < o.subject_id AND s2.academic_year IS NOT NULL
    ORDER BY s2.subject_id DESC LIMIT 1
) prev ON TRUE
LEFT JOIN LATERAL (
    SELECT subject_id, academic_year FROM subject s3
    WHERE s3.tenant_id = o.tenant_id AND s3.school_id = o.school_id
      AND s3.subject_id > o.subject_id AND s3.academic_year IS NOT NULL
    ORDER BY s3.subject_id ASC LIMIT 1
) nxt ON TRUE
WHERE o.academic_year IS NULL
ORDER BY o.tenant_id, o.school_id, o.subject_id;

-- ============================================================================
-- STEP 3: Fix — set academic_year to the inferred value.
-- Uses the same logic as Step 2: previous neighbor's year, falling back to next.
-- ============================================================================

UPDATE subject o
JOIN LATERAL (
    SELECT academic_year FROM subject s2
    WHERE s2.tenant_id = o.tenant_id AND s2.school_id = o.school_id
      AND s2.subject_id < o.subject_id AND s2.academic_year IS NOT NULL
    ORDER BY s2.subject_id DESC LIMIT 1
) prev ON TRUE
SET o.academic_year = prev.academic_year
WHERE o.academic_year IS NULL;

-- Handle edge case: orphan is the first subject in the school (no previous neighbor)
UPDATE subject o
JOIN LATERAL (
    SELECT academic_year FROM subject s3
    WHERE s3.tenant_id = o.tenant_id AND s3.school_id = o.school_id
      AND s3.subject_id > o.subject_id AND s3.academic_year IS NOT NULL
    ORDER BY s3.subject_id ASC LIMIT 1
) nxt ON TRUE
SET o.academic_year = nxt.academic_year
WHERE o.academic_year IS NULL;
