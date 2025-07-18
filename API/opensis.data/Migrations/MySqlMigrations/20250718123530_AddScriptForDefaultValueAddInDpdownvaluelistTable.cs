using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddScriptForDefaultValueAddInDpdownvaluelistTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Delete existing null school_id records for that tenant
            migrationBuilder.Sql(@"
        DELETE FROM dpdown_valuelist 
        WHERE tenant_id = '1e93c7bf-0fae-42bb-9e09-a1cedc8c0355' AND school_id IS NULL;
    ");

            // 2. Initialize @NextId
            migrationBuilder.Sql(@"
        SET @NextId := (SELECT IFNULL(MAX(id), 0) + 1 FROM dpdown_valuelist);
    ");

            // 3. Insert 7 rows per school
            migrationBuilder.Sql(@"
        INSERT INTO dpdown_valuelist (id, tenant_id, school_id, lov_name, lov_column_value, created_on)
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Level', 'Pre-K', UTC_TIMESTAMP() FROM school_master s
        UNION ALL
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Level', 'Primary', UTC_TIMESTAMP() FROM school_master s
        UNION ALL
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Level', 'Middle', UTC_TIMESTAMP() FROM school_master s
        UNION ALL
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Level', 'Secondary', UTC_TIMESTAMP() FROM school_master s
        UNION ALL
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Classification', 'Public', UTC_TIMESTAMP() FROM school_master s
        UNION ALL
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Classification', 'Private', UTC_TIMESTAMP() FROM school_master s
        UNION ALL
        SELECT @NextId := @NextId + 1, s.tenant_id, s.school_id, 'School Classification', 'Charter', UTC_TIMESTAMP() FROM school_master s;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
