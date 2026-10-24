using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    public partial class AddTelegramIdUniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM "Telegrams" t
                USING "Telegrams" other
                WHERE t."TelegramId" = other."TelegramId" AND t."Id" > other."Id";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Telegrams_TelegramId",
                table: "Telegrams",
                column: "TelegramId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Telegrams_TelegramId",
                table: "Telegrams");
        }
    }
}
