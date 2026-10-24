using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    public partial class FixSeedSequences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                SELECT setval(pg_get_serial_sequence('"CoupleTimes"', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "CoupleTimes"), 1));
                """);

            migrationBuilder.Sql("""
                SELECT setval(pg_get_serial_sequence('"DayOfWeeks"', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "DayOfWeeks"), 1));
                """);

            migrationBuilder.Sql("""
                SELECT setval(pg_get_serial_sequence('"SubjectTypes"', 'Id'), GREATEST((SELECT COALESCE(MAX("Id"), 0) FROM "SubjectTypes"), 1));
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
