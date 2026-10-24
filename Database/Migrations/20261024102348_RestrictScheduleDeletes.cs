using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IpDeputyApi.Database.Migrations
{
    public partial class RestrictScheduleDeletes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_DayOfWeeks_DayOfWeekId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_SubjectTypes_SubjectTypeId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_Subjects_SubjectId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_Teachers_TeacherId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_CoupleTimes_CoupleTimeId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_DayOfWeeks_DayOfWeekId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_SubjectTypes_SubjectTypeId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_Subjects_SubjectId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_Teachers_TeacherId",
                table: "Couples");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_DayOfWeeks_DayOfWeekId",
                table: "AdditionalCouples",
                column: "DayOfWeekId",
                principalTable: "DayOfWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_SubjectTypes_SubjectTypeId",
                table: "AdditionalCouples",
                column: "SubjectTypeId",
                principalTable: "SubjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_Subjects_SubjectId",
                table: "AdditionalCouples",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_Teachers_TeacherId",
                table: "AdditionalCouples",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_CoupleTimes_CoupleTimeId",
                table: "Couples",
                column: "CoupleTimeId",
                principalTable: "CoupleTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_DayOfWeeks_DayOfWeekId",
                table: "Couples",
                column: "DayOfWeekId",
                principalTable: "DayOfWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_SubjectTypes_SubjectTypeId",
                table: "Couples",
                column: "SubjectTypeId",
                principalTable: "SubjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_Subjects_SubjectId",
                table: "Couples",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_Teachers_TeacherId",
                table: "Couples",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_DayOfWeeks_DayOfWeekId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_SubjectTypes_SubjectTypeId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_Subjects_SubjectId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_AdditionalCouples_Teachers_TeacherId",
                table: "AdditionalCouples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_CoupleTimes_CoupleTimeId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_DayOfWeeks_DayOfWeekId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_SubjectTypes_SubjectTypeId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_Subjects_SubjectId",
                table: "Couples");

            migrationBuilder.DropForeignKey(
                name: "FK_Couples_Teachers_TeacherId",
                table: "Couples");

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_DayOfWeeks_DayOfWeekId",
                table: "AdditionalCouples",
                column: "DayOfWeekId",
                principalTable: "DayOfWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_SubjectTypes_SubjectTypeId",
                table: "AdditionalCouples",
                column: "SubjectTypeId",
                principalTable: "SubjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_Subjects_SubjectId",
                table: "AdditionalCouples",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdditionalCouples_Teachers_TeacherId",
                table: "AdditionalCouples",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_CoupleTimes_CoupleTimeId",
                table: "Couples",
                column: "CoupleTimeId",
                principalTable: "CoupleTimes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_DayOfWeeks_DayOfWeekId",
                table: "Couples",
                column: "DayOfWeekId",
                principalTable: "DayOfWeeks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_SubjectTypes_SubjectTypeId",
                table: "Couples",
                column: "SubjectTypeId",
                principalTable: "SubjectTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_Subjects_SubjectId",
                table: "Couples",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Couples_Teachers_TeacherId",
                table: "Couples",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
