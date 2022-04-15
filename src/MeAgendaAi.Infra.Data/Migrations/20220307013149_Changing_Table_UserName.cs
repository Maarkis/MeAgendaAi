using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class Changing_Table_UserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NM_NAME",
                table: "TB_PHYSICAL_PERSON");

            migrationBuilder.DropColumn(
                name: "NM_SURNAME",
                table: "TB_PHYSICAL_PERSON");

            migrationBuilder.DropColumn(
                name: "NM_NAME",
                table: "TB_COMPANY");

            migrationBuilder.AddColumn<string>(
                name: "NM_FIRST_NAME",
                table: "TB_USERS",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NM_LAST_NAME",
                table: "TB_USERS",
                type: "varchar(80)",
                maxLength: 80,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NM_FIRST_NAME",
                table: "TB_USERS");

            migrationBuilder.DropColumn(
                name: "NM_LAST_NAME",
                table: "TB_USERS");

            migrationBuilder.AddColumn<string>(
                name: "NM_NAME",
                table: "TB_PHYSICAL_PERSON",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NM_SURNAME",
                table: "TB_PHYSICAL_PERSON",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NM_NAME",
                table: "TB_COMPANY",
                type: "varchar(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }
    }
}