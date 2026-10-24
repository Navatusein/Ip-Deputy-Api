using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    public partial class AddUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Login = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    LegacyPasswordSalt = table.Column<string>(type: "text", nullable: true),
                    Permissions = table.Column<List<string>>(type: "text[]", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    TokenVersion = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Login",
                table: "Users",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentId",
                table: "Users",
                column: "StudentId");

            migrationBuilder.Sql("""
                INSERT INTO "Users" ("Login", "DisplayName", "StudentId", "PasswordHash", "LegacyPasswordSalt", "Permissions", "IsActive", "TokenVersion", "CreatedAt")
                SELECT DISTINCT ON (lower(btrim(w."Login")))
                    left(lower(btrim(w."Login")), 64),
                    left(COALESCE(NULLIF(regexp_replace(btrim(concat_ws(' ', s."Surname", s."Name")), '\s+', ' ', 'g'), ''), btrim(w."Login")), 256),
                    w."StudentId",
                    w."PasswordHash",
                    w."PasswordSalt",
                    ARRAY['manage-students', 'manage-teachers', 'manage-subjects', 'manage-schedule', 'manage-submissions', 'manage-links', 'manage-users']::text[],
                    TRUE,
                    0,
                    now()
                FROM "WebAuthentications" w
                LEFT JOIN "Students" s ON s."Id" = w."StudentId"
                ORDER BY lower(btrim(w."Login")), w."Id";
                """);

            migrationBuilder.DropTable(
                name: "WebAuthentications");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebAuthentications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    Login = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PasswordSalt = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebAuthentications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebAuthentications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebAuthentications_StudentId",
                table: "WebAuthentications",
                column: "StudentId",
                unique: true);

            migrationBuilder.Sql("""
                INSERT INTO "WebAuthentications" ("StudentId", "Login", "PasswordHash", "PasswordSalt")
                SELECT DISTINCT ON (u."StudentId") u."StudentId", u."Login", u."PasswordHash", u."LegacyPasswordSalt"
                FROM "Users" u
                WHERE u."StudentId" IS NOT NULL AND u."LegacyPasswordSalt" IS NOT NULL
                ORDER BY u."StudentId", u."Id";
                """);

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
