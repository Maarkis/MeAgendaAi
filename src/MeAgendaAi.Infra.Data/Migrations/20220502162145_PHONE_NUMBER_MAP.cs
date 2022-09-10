

#nullable disable

using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;
namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class PHONE_NUMBER_MAP : Migration
    {
        [ExcludeFromCodeCoverage]
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_PHONE_NUMBERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NM_CONTACT = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true),
                    NUM_COUNTRY_CODE = table.Column<decimal>(type: "NUMERIC(3)", nullable: false),
                    NUM_DIAL_CODE = table.Column<decimal>(type: "NUMERIC(2)", nullable: false),
                    NUM_NUMBER = table.Column<string>(type: "varchar(9)", nullable: false),
                    NM_TYPE = table.Column<decimal>(type: "NUMERIC(1)", nullable: false),
                    DT_CREATED_AT = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DT_LAST_UPDATED_AT = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PHONE_NUMBERS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_PHONE_NUMBERS_TB_USERS_Id",
                        column: x => x.Id,
                        principalTable: "TB_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_PHONE_NUMBERS");
        }
    }
}
