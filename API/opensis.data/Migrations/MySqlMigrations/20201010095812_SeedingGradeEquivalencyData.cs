using Microsoft.EntityFrameworkCore.Migrations;

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class SeedingGradeEquivalencyData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
           table: "grade_equivalency",
           columns: new[] { "country", "isced_grade_level", "grade_description", "age_range", "educational_stage" },
           values: new object[,]
           {
                { "US",0, "01 Early childhood educational development","1-3", "None" },
                { "US",0, "02 Pre-primary education","3-5", "Pre-primary education" },
                { "US",1, "Primary education","5-6", "Primary education" },
                { "US",2, "Lower secondary education","5-6", "Lower secondary education" },
                { "US",3, "Upper secondary education","5-6", "Upper secondary education" },
                { "US",4, "Post-secondary non-tertiary education","5-6", "Post-secondary non-tertiary education" },
                { "US",5, "Short-cycle tertiary education","5-6", "Short-cycle tertiary education" },
                { "US",6, "Bachelor or equivalent","5-6", "Bachelor or equivalent" },
                { "US",7, "Master or equivalent","5-6", "Master or equivalent" }


           });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
