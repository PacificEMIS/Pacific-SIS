-- Add the "Super Administrators" page (Settings > Administration) for existing
-- tenant databases. New schools get it from SubCategory.json / RolePermission.json
-- at creation time; run this once per existing tenant database after deploying.
-- Safe to re-run: uses NOT EXISTS guards.

-- 1. Insert the permission subcategory under Settings > Administration
--    (permission_group_id = 12, permission_category_id = 27), next to
--    "Profiles & Permissions" (permission_subcategory_id = 32).
INSERT INTO permission_subcategory
    (tenant_id, school_id, permission_subcategory_id, permission_category_id, permission_group_id,
     permission_subcategory_name, short_code, path, title, type,
     enable_view, enable_add, enable_edit, enable_delete, is_system, is_active, sort_order, created_on)
SELECT DISTINCT
    ps.tenant_id, ps.school_id, 100, 27, 12,
    'Super Administrators', 'sa',
    '/school/settings/administration-settings/super-administrators',
    'Super Administrators', '',
    1, 1, 1, 1, 1, 1, 2, UTC_TIMESTAMP()
FROM permission_subcategory ps
WHERE ps.permission_category_id = 27
  AND ps.permission_subcategory_id = 32
  AND NOT EXISTS (
    SELECT 1 FROM permission_subcategory ps2
    WHERE ps2.tenant_id = ps.tenant_id
      AND ps2.school_id = ps.school_id
      AND ps2.permission_subcategory_id = 100
  );

-- 2. Grant it to Super Administrator (membership_id = 1) only. Other profiles
--    never see the page; the API also refuses any caller who is not an active
--    Super Administrator.
INSERT INTO role_permission
    (tenant_id, school_id, role_permission_id, permission_group_id, permission_category_id, permission_subcategory_id,
     can_view, can_add, can_edit, can_delete, membership_id, created_on)
SELECT
    ps.tenant_id, ps.school_id,
    COALESCE((SELECT MAX(rp2.role_permission_id) FROM role_permission rp2 WHERE rp2.tenant_id = ps.tenant_id AND rp2.school_id = ps.school_id), 0) + 1,
    NULL, NULL, 100, 1, 1, 1, 1, 1, UTC_TIMESTAMP()
FROM permission_subcategory ps
WHERE ps.permission_subcategory_id = 100
  AND NOT EXISTS (
    SELECT 1 FROM role_permission rp
    WHERE rp.tenant_id = ps.tenant_id
      AND rp.school_id = ps.school_id
      AND rp.permission_subcategory_id = 100
      AND rp.membership_id = 1
  );
