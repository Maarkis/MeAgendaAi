using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class Map_User_PhysicalPerson_Company : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_USERS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EMAIL = table.Column<string>(type: "text", nullable: false),
                    PASS_PASSWORD = table.Column<string>(type: "text", nullable: false),
                    DT_CREATED_AT = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    DT_LAST_UPDATED_AT = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USERS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_COMPANY",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NM_NAME = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    CNPJ = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    DSC_DESCRIPTION = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false),
                    TIME_LIMITCANCELHOURS = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_COMPANY", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_COMPANY_TB_USERS_Id",
                        column: x => x.Id,
                        principalTable: "TB_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_PHYSICAL_PERSON",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NM_NAME = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    NM_SURNAME = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    CPF = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false),
                    RG = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PHYSICAL_PERSON", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_PHYSICAL_PERSON_TB_USERS_Id",
                        column: x => x.Id,
                        principalTable: "TB_USERS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IN_COMPANY_CNPJ",
                table: "TB_COMPANY",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IN_PHYSICAL_PERSON_CPF",
                table: "TB_PHYSICAL_PERSON",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IN_USERS_EMAIL",
                table: "TB_USERS",
                column: "EMAIL",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_COMPANY");

            migrationBuilder.DropTable(
                name: "TB_PHYSICAL_PERSON");

            migrationBuilder.DropTable(
                name: "TB_USERS");
        }
    }
}
