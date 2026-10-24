using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814

namespace IpDeputyApi.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoupleTimes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    TimeStart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    TimeEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupleTimes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayOfWeeks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayOfWeeks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subgroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subgroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    LaboratoryCount = table.Column<int>(type: "integer", nullable: false),
                    PracticalCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    FitEmail = table.Column<string>(type: "text", nullable: true),
                    TelegramNickname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubgroupId = table.Column<int>(type: "integer", nullable: true),
                    Index = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Patronymic = table.Column<string>(type: "text", nullable: false),
                    TelegramPhone = table.Column<string>(type: "text", nullable: false),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FitEmail = table.Column<string>(type: "text", nullable: false),
                    TelegramNickname = table.Column<string>(type: "text", nullable: true),
                    Birthday = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Students_Subgroups_SubgroupId",
                        column: x => x.SubgroupId,
                        principalTable: "Subgroups",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionsConfigs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubgroupId = table.Column<int>(type: "integer", nullable: true),
                    SubjectId = table.Column<int>(type: "integer", nullable: true),
                    SubjectTypeId = table.Column<int>(type: "integer", nullable: true),
                    CustomType = table.Column<string>(type: "text", nullable: true),
                    CustomName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionsConfigs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionsConfigs_Subgroups_SubgroupId",
                        column: x => x.SubgroupId,
                        principalTable: "Subgroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubmissionsConfigs_SubjectTypes_SubjectTypeId",
                        column: x => x.SubjectTypeId,
                        principalTable: "SubjectTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubmissionsConfigs_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AdditionalCouples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    SubjectTypeId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeekId = table.Column<int>(type: "integer", nullable: false),
                    CoupleTimeId = table.Column<int>(type: "integer", nullable: true),
                    SubgroupId = table.Column<int>(type: "integer", nullable: true),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Time = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Cabinet = table.Column<string>(type: "text", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalCouples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalCouples_CoupleTimes_CoupleTimeId",
                        column: x => x.CoupleTimeId,
                        principalTable: "CoupleTimes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdditionalCouples_DayOfWeeks_DayOfWeekId",
                        column: x => x.DayOfWeekId,
                        principalTable: "DayOfWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalCouples_Subgroups_SubgroupId",
                        column: x => x.SubgroupId,
                        principalTable: "Subgroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AdditionalCouples_SubjectTypes_SubjectTypeId",
                        column: x => x.SubjectTypeId,
                        principalTable: "SubjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalCouples_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalCouples_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Couples",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<int>(type: "integer", nullable: false),
                    SubjectTypeId = table.Column<int>(type: "integer", nullable: false),
                    DayOfWeekId = table.Column<int>(type: "integer", nullable: false),
                    CoupleTimeId = table.Column<int>(type: "integer", nullable: false),
                    SubgroupId = table.Column<int>(type: "integer", nullable: true),
                    TeacherId = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    IsRolling = table.Column<bool>(type: "boolean", nullable: false),
                    Cabinet = table.Column<string>(type: "text", nullable: true),
                    AdditionalInformation = table.Column<string>(type: "text", nullable: true),
                    Link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Couples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Couples_CoupleTimes_CoupleTimeId",
                        column: x => x.CoupleTimeId,
                        principalTable: "CoupleTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Couples_DayOfWeeks_DayOfWeekId",
                        column: x => x.DayOfWeekId,
                        principalTable: "DayOfWeeks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Couples_Subgroups_SubgroupId",
                        column: x => x.SubgroupId,
                        principalTable: "Subgroups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Couples_SubjectTypes_SubjectTypeId",
                        column: x => x.SubjectTypeId,
                        principalTable: "SubjectTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Couples_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Couples_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Telegrams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    TelegramId = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    ScheduleCompact = table.Column<bool>(type: "boolean", nullable: false),
                    LastCongratulations = table.Column<DateOnly>(type: "date", nullable: true),
                    LastActivity = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telegrams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Telegrams_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "SubmissionWorks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubmissionsConfigId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Index = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionWorks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionWorks_SubmissionsConfigs_SubmissionsConfigId",
                        column: x => x.SubmissionsConfigId,
                        principalTable: "SubmissionsConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoupleDates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CoupleId = table.Column<int>(type: "integer", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsRemovedDate = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoupleDates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoupleDates_Couples_CoupleId",
                        column: x => x.CoupleId,
                        principalTable: "Couples",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionStudents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubmissionWorkId = table.Column<int>(type: "integer", nullable: false),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    SubmissionsConfigId = table.Column<int>(type: "integer", nullable: false),
                    PreferredPosition = table.Column<int>(type: "integer", nullable: false),
                    SubmittedAt = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionStudents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionStudents_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionStudents_SubmissionWorks_SubmissionWorkId",
                        column: x => x.SubmissionWorkId,
                        principalTable: "SubmissionWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubmissionStudents_SubmissionsConfigs_SubmissionsConfigId",
                        column: x => x.SubmissionsConfigId,
                        principalTable: "SubmissionsConfigs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CoupleTimes",
                columns: new[] { "Id", "Index", "TimeEnd", "TimeStart" },
                values: new object[,]
                {
                    { 1, 1, new TimeOnly(10, 20, 0), new TimeOnly(9, 0, 0) },
                    { 2, 2, new TimeOnly(11, 50, 0), new TimeOnly(10, 30, 0) },
                    { 3, 3, new TimeOnly(13, 30, 0), new TimeOnly(12, 10, 0) },
                    { 4, 4, new TimeOnly(15, 0, 0), new TimeOnly(13, 40, 0) },
                    { 5, 5, new TimeOnly(16, 30, 0), new TimeOnly(15, 10, 0) },
                    { 6, 6, new TimeOnly(18, 0, 0), new TimeOnly(16, 40, 0) },
                    { 7, 7, new TimeOnly(19, 30, 0), new TimeOnly(18, 10, 0) }
                });

            migrationBuilder.InsertData(
                table: "DayOfWeeks",
                columns: new[] { "Id", "Index", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Понеділок" },
                    { 2, 2, "Вівторок" },
                    { 3, 3, "Середа" },
                    { 4, 4, "Четвер" },
                    { 5, 5, "П'ятниця" },
                    { 6, 6, "Субота" },
                    { 7, 0, "Неділя" }
                });

            migrationBuilder.InsertData(
                table: "SubjectTypes",
                columns: new[] { "Id", "Name", "ShortName" },
                values: new object[,]
                {
                    { 1, "Лабораторне", "Лаб." },
                    { 2, "Практика", "Пр." },
                    { 3, "Лекція", "Лек." },
                    { 4, "Семінар", "Сем." },
                    { 5, "Консультація", "Конс." },
                    { 6, "Залік", "Зал." },
                    { 7, "Екзамен", "Екз." },
                    { 8, "Додаткова", "Дод." }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCouples_CoupleTimeId",
                table: "AdditionalCouples",
                column: "CoupleTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCouples_DayOfWeekId",
                table: "AdditionalCouples",
                column: "DayOfWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCouples_SubgroupId",
                table: "AdditionalCouples",
                column: "SubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCouples_SubjectId",
                table: "AdditionalCouples",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCouples_SubjectTypeId",
                table: "AdditionalCouples",
                column: "SubjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCouples_TeacherId",
                table: "AdditionalCouples",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_CoupleDates_CoupleId",
                table: "CoupleDates",
                column: "CoupleId");

            migrationBuilder.CreateIndex(
                name: "IX_Couples_CoupleTimeId",
                table: "Couples",
                column: "CoupleTimeId");

            migrationBuilder.CreateIndex(
                name: "IX_Couples_DayOfWeekId",
                table: "Couples",
                column: "DayOfWeekId");

            migrationBuilder.CreateIndex(
                name: "IX_Couples_SubgroupId",
                table: "Couples",
                column: "SubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Couples_SubjectId",
                table: "Couples",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Couples_SubjectTypeId",
                table: "Couples",
                column: "SubjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Couples_TeacherId",
                table: "Couples",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Students_SubgroupId",
                table: "Students",
                column: "SubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionsConfigs_SubgroupId",
                table: "SubmissionsConfigs",
                column: "SubgroupId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionsConfigs_SubjectId",
                table: "SubmissionsConfigs",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionsConfigs_SubjectTypeId",
                table: "SubmissionsConfigs",
                column: "SubjectTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStudents_StudentId",
                table: "SubmissionStudents",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStudents_SubmissionsConfigId",
                table: "SubmissionStudents",
                column: "SubmissionsConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionStudents_SubmissionWorkId",
                table: "SubmissionStudents",
                column: "SubmissionWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionWorks_SubmissionsConfigId",
                table: "SubmissionWorks",
                column: "SubmissionsConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Telegrams_StudentId",
                table: "Telegrams",
                column: "StudentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WebAuthentications_StudentId",
                table: "WebAuthentications",
                column: "StudentId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalCouples");

            migrationBuilder.DropTable(
                name: "CoupleDates");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "SubmissionStudents");

            migrationBuilder.DropTable(
                name: "Telegrams");

            migrationBuilder.DropTable(
                name: "WebAuthentications");

            migrationBuilder.DropTable(
                name: "Couples");

            migrationBuilder.DropTable(
                name: "SubmissionWorks");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "CoupleTimes");

            migrationBuilder.DropTable(
                name: "DayOfWeeks");

            migrationBuilder.DropTable(
                name: "Teachers");

            migrationBuilder.DropTable(
                name: "SubmissionsConfigs");

            migrationBuilder.DropTable(
                name: "Subgroups");

            migrationBuilder.DropTable(
                name: "SubjectTypes");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
