using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class AddPunctuantionInTheCPFFieldWithTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "TB_PHYSICAL_PERSON",
                type: "varchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CPF",
                table: "TB_PHYSICAL_PERSON",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(15)",
                oldMaxLength: 15);
        }
    }
}
