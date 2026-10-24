using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    public partial class AlignCoupleDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "Couples"
                SET "StartDate" = "StartDate" + ((("DayOfWeekId" - EXTRACT(ISODOW FROM "StartDate")::int) + 7) % 7)
                WHERE "StartDate" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE "Couples"
                SET "EndDate" = "EndDate" - (((EXTRACT(ISODOW FROM "EndDate")::int - "DayOfWeekId") + 7) % 7)
                WHERE "EndDate" IS NOT NULL;
                """);

            migrationBuilder.Sql("""
                UPDATE "Couples"
                SET "StartDate" = NULL, "EndDate" = NULL
                WHERE "StartDate" IS NOT NULL AND "EndDate" IS NOT NULL AND "EndDate" < "StartDate";
                """);

            migrationBuilder.Sql("""
                DELETE FROM "CoupleDates" d
                USING "CoupleDates" other
                WHERE d."CoupleId" = other."CoupleId" AND d."Date" = other."Date" AND d."IsRemovedDate" = other."IsRemovedDate" AND d."Id" > other."Id";
                """);

            migrationBuilder.Sql("""
                DELETE FROM "CoupleDates" removed
                USING "CoupleDates" added
                WHERE removed."CoupleId" = added."CoupleId" AND removed."Date" = added."Date" AND removed."IsRemovedDate" AND NOT added."IsRemovedDate";
                """);

            migrationBuilder.Sql("""
                INSERT INTO "AdditionalCouples" ("SubjectId", "SubjectTypeId", "DayOfWeekId", "CoupleTimeId", "SubgroupId", "TeacherId", "Date", "Time", "Cabinet", "AdditionalInformation", "Link")
                SELECT c."SubjectId", c."SubjectTypeId", EXTRACT(ISODOW FROM d."Date")::int, c."CoupleTimeId", c."SubgroupId", c."TeacherId", d."Date", NULL, c."Cabinet", c."AdditionalInformation", c."Link"
                FROM "CoupleDates" d
                JOIN "Couples" c ON c."Id" = d."CoupleId"
                WHERE NOT d."IsRemovedDate" AND EXTRACT(ISODOW FROM d."Date")::int <> c."DayOfWeekId";
                """);

            migrationBuilder.Sql("""
                DELETE FROM "CoupleDates" d
                USING "Couples" c
                WHERE c."Id" = d."CoupleId" AND EXTRACT(ISODOW FROM d."Date")::int <> c."DayOfWeekId";
                """);

            migrationBuilder.Sql("""
                UPDATE "AdditionalCouples"
                SET "DayOfWeekId" = EXTRACT(ISODOW FROM "Date")::int;
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
