using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class FieldNameAdjustedFromTIME_LIMITCANCELHOURSToTIME_LIMIT_CANCEL_HOURS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TIME_LIMITCANCELHOURS",
                table: "TB_COMPANY",
                newName: "TIME_LIMIT_CANCEL_HOURS");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TIME_LIMIT_CANCEL_HOURS",
                table: "TB_COMPANY",
                newName: "TIME_LIMITCANCELHOURS");
        }
    }
}
