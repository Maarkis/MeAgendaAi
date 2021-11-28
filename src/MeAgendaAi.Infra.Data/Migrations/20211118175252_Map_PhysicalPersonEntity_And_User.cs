using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class Map_PhysicalPersonEntity_And_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_USERS",
                columns: table => new
                {
                    PK_TB_PHYSICAL_PERSON = table.Column<Guid>(type: "uuid", nullable: false),
                    EMAIL = table.Column<string>(type: "text", nullable: false),
                    PASS_PASSWORD = table.Column<string>(type: "text", nullable: false),
                    DT_CREATED_AT = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    DT_LAST_UPDATED_AT = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_USERS", x => x.PK_TB_PHYSICAL_PERSON);
                });

            migrationBuilder.CreateTable(
                name: "TB_PHYSICAL_PERSON",
                columns: table => new
                {
                    PK_TB_PHYSICAL_PERSON = table.Column<Guid>(type: "uuid", nullable: false),
                    NM_NAME = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    NM_SURNAME = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    CPF = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: false),
                    RG = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PHYSICAL_PERSON", x => x.PK_TB_PHYSICAL_PERSON);
                    table.ForeignKey(
                        name: "FK_TB_PHYSICAL_PERSON_TB_USERS_PK_TB_PHYSICAL_PERSON",
                        column: x => x.PK_TB_PHYSICAL_PERSON,
                        principalTable: "TB_USERS",
                        principalColumn: "PK_TB_PHYSICAL_PERSON",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IN_USERS_EMAIL",
                table: "TB_USERS",
                column: "EMAIL",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_PHYSICAL_PERSON");

            migrationBuilder.DropTable(
                name: "TB_USERS");
        }
    }
}
