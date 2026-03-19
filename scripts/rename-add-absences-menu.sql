-- =============================================================================
-- Rename "Add Absences" menu item to "Enter/Override Attendance"
--
-- Run against each tenant database.
--
-- Context: The "Add Absences" tool actually applies any attendance code (not
-- just absences) and overwrites existing records. Renamed to accurately
-- reflect what it does. See commit for full UI/description changes.
--
-- Safe: permission_category_name and title are display-only labels.
-- All permission enforcement uses the numeric permission_category_id (30)
-- and permission_group_id (9), which are unchanged.
-- =============================================================================

UPDATE permission_category
SET permission_category_name = 'Enter/Override Attendance',
    title = 'Enter/Override Attendance'
WHERE permission_category_name = 'Add Absences';
