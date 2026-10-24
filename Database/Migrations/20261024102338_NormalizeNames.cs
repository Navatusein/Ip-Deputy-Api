using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    public partial class NormalizeNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE "Teachers" SET
                    "Name" = regexp_replace(btrim("Name"), '\s+', ' ', 'g'),
                    "Surname" = regexp_replace(btrim("Surname"), '\s+', ' ', 'g'),
                    "Patronymic" = regexp_replace(btrim("Patronymic"), '\s+', ' ', 'g'),
                    "ContactPhone" = regexp_replace(btrim("ContactPhone"), '\s+', ' ', 'g'),
                    "Email" = regexp_replace(btrim("Email"), '\s+', ' ', 'g'),
                    "FitEmail" = regexp_replace(btrim("FitEmail"), '\s+', ' ', 'g'),
                    "TelegramNickname" = regexp_replace(btrim("TelegramNickname"), '\s+', ' ', 'g');
                """);

            migrationBuilder.Sql("""
                UPDATE "Students" SET
                    "Name" = regexp_replace(btrim("Name"), '\s+', ' ', 'g'),
                    "Surname" = regexp_replace(btrim("Surname"), '\s+', ' ', 'g'),
                    "Patronymic" = regexp_replace(btrim("Patronymic"), '\s+', ' ', 'g');
                """);

            migrationBuilder.Sql("""
                UPDATE "Students" SET
                    "TelegramPhone" = btrim("TelegramPhone"),
                    "Email" = btrim("Email"),
                    "FitEmail" = btrim("FitEmail");
                """);

            migrationBuilder.Sql("""
                UPDATE "Students" SET "ContactPhone" = NULLIF(btrim("ContactPhone"), ''), "TelegramNickname" = NULLIF(btrim("TelegramNickname"), '');
                """);

            migrationBuilder.Sql("""
                UPDATE "Teachers" SET "ContactPhone" = NULLIF("ContactPhone", ''), "Email" = NULLIF("Email", ''), "FitEmail" = NULLIF("FitEmail", ''), "TelegramNickname" = NULLIF("TelegramNickname", '');
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
