-- =============================================================================
-- Add "Fill Attendance as Present" menu item to existing schools
--
-- Run against each tenant database.
--
-- Context: The feature was added in code (route, controller, static-data seed)
-- but existing schools don't get new permission_category rows automatically.
-- New schools created after the deploy will get it from static-data.ts.
--
-- This inserts the menu item for every school that doesn't already have it,
-- and copies role_permission grants from the sibling "Enter/Override Attendance"
-- (permission_category_id 30) so the same roles get access.
-- =============================================================================

START TRANSACTION;

-- Step 1: Preview — schools that are missing the menu item
SELECT sm.school_id, sm.school_name, 'MISSING' AS status
FROM school_master sm
WHERE NOT EXISTS (
    SELECT 1 FROM permission_category pc
    WHERE pc.tenant_id = sm.tenant_id AND pc.school_id = sm.school_id
    AND pc.permission_category_id = 95 AND pc.permission_group_id = 9
)
ORDER BY sm.school_id;

-- Step 2: Insert permission_category for all schools missing it
INSERT INTO permission_category (
    tenant_id, school_id, permission_category_id, permission_group_id,
    permission_category_name, short_code, path, title, type,
    enable_view, enable_add, enable_edit, enable_delete, is_active, sort_order
)
SELECT
    sm.tenant_id, sm.school_id, 95, 9,
    'Fill Attendance as Present', 'flatp', '/school/attendance/fill-attendance',
    'Fill Attendance as Present', 'link',
    1, 1, 1, 1, 1, 5
FROM school_master sm
WHERE NOT EXISTS (
    SELECT 1 FROM permission_category pc
    WHERE pc.tenant_id = sm.tenant_id AND pc.school_id = sm.school_id
    AND pc.permission_category_id = 95 AND pc.permission_group_id = 9
);

-- Fix rows already inserted without is_active or sort_order
UPDATE permission_category
SET is_active = 1, sort_order = 5
WHERE permission_category_id = 95 AND permission_group_id = 9
AND (is_active IS NULL OR is_active = 0 OR sort_order IS NULL);

-- Step 3: Copy role grants from "Enter/Override Attendance" (category 30)
-- Any role that can use Enter/Override should also see Fill Attendance.
-- Generates new role_permission_id values by offsetting from each school's max.
INSERT INTO role_permission (
    tenant_id, school_id, role_permission_id, membership_id,
    permission_category_id, can_view, can_add, can_edit, can_delete
)
SELECT
    rp.tenant_id, rp.school_id,
    (SELECT COALESCE(MAX(rp2.role_permission_id), 0) FROM role_permission rp2
     WHERE rp2.tenant_id = rp.tenant_id AND rp2.school_id = rp.school_id)
    + ROW_NUMBER() OVER (PARTITION BY rp.tenant_id, rp.school_id ORDER BY rp.membership_id),
    rp.membership_id,
    95, -- permission_category_id for Fill Attendance
    rp.can_view, rp.can_add, rp.can_edit, rp.can_delete
FROM role_permission rp
WHERE rp.permission_category_id = 30  -- Enter/Override Attendance
AND NOT EXISTS (
    SELECT 1 FROM role_permission rp3
    WHERE rp3.tenant_id = rp.tenant_id AND rp3.school_id = rp.school_id
    AND rp3.permission_category_id = 95 AND rp3.membership_id = rp.membership_id
);

-- Step 4: Verify — show all schools and which roles got the grant
SELECT
    sm.school_id,
    sm.school_name,
    pc.permission_category_name,
    rp.membership_id,
    rp.can_view, rp.can_add, rp.can_edit, rp.can_delete
FROM school_master sm
LEFT JOIN permission_category pc
    ON pc.tenant_id = sm.tenant_id AND pc.school_id = sm.school_id
    AND pc.permission_category_id = 95 AND pc.permission_group_id = 9
LEFT JOIN role_permission rp
    ON rp.tenant_id = sm.tenant_id AND rp.school_id = sm.school_id
    AND rp.permission_category_id = 95
ORDER BY sm.school_id, rp.membership_id;

-- =============================================================================
-- Review results above. If everything looks correct:
COMMIT;
-- If something looks wrong:
--   ROLLBACK;
-- =============================================================================
