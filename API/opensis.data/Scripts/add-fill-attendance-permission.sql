-- Add "Fill Attendance as Present" permission for existing tenant databases
-- Run this once per tenant database after deploying the new code.
-- Safe to re-run: uses NOT EXISTS guards.

-- 1. Insert the permission category under the Attendance group (permission_group_id = 9)
INSERT INTO permission_category
    (tenant_id, school_id, permission_category_id, permission_group_id, permission_category_name, short_code, path, title, type, enable_view, enable_add, enable_edit, enable_delete, is_active, sort_order, created_on)
SELECT DISTINCT
    pc.tenant_id, pc.school_id, 95, 9,
    'Fill Attendance as Present', 'flatp',
    '/school/attendance/fill-attendance',
    'Fill Attendance as Present', 'link',
    1, 1, 1, 1, 1, 3, UTC_TIMESTAMP()
FROM permission_category pc
WHERE pc.permission_group_id = 9
  AND pc.permission_category_id = 30
  AND NOT EXISTS (
    SELECT 1 FROM permission_category pc2
    WHERE pc2.tenant_id = pc.tenant_id
      AND pc2.school_id = pc.school_id
      AND pc2.permission_category_id = 95
  );

-- 2. Insert role_permission for Super Admin (membership_id = 1) only
INSERT INTO role_permission
    (tenant_id, school_id, role_permission_id, permission_group_id, permission_category_id, permission_subcategory_id, can_view, can_add, can_edit, can_delete, membership_id, created_on)
SELECT
    pc.tenant_id, pc.school_id,
    COALESCE((SELECT MAX(rp2.role_permission_id) FROM role_permission rp2 WHERE rp2.tenant_id = pc.tenant_id AND rp2.school_id = pc.school_id), 0) + 1,
    NULL, 95, NULL, 1, 1, 1, 1, 1, UTC_TIMESTAMP()
FROM permission_category pc
WHERE pc.permission_category_id = 95
  AND NOT EXISTS (
    SELECT 1 FROM role_permission rp
    WHERE rp.tenant_id = pc.tenant_id
      AND rp.school_id = pc.school_id
      AND rp.permission_category_id = 95
      AND rp.membership_id = 1
  );
