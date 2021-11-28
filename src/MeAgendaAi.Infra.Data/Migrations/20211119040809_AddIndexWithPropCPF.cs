using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class AddIndexWithPropCPF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IN_PHYSICAL_PERSON_CPF",
                table: "TB_PHYSICAL_PERSON",
                column: "CPF",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IN_PHYSICAL_PERSON_CPF",
                table: "TB_PHYSICAL_PERSON");
        }
    }
}
