using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeAgendaAi.Infra.Data.Migrations
{
    public partial class Phone_Number_Mapping_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_PHONE_NUMBERS_TB_USERS_Id",
                table: "TB_PHONE_NUMBERS");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TB_PHONE_NUMBERS",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TB_PHONE_NUMBERS_UserId",
                table: "TB_PHONE_NUMBERS",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PHONE_NUMBERS_TB_USERS_UserId",
                table: "TB_PHONE_NUMBERS",
                column: "UserId",
                principalTable: "TB_USERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TB_PHONE_NUMBERS_TB_USERS_UserId",
                table: "TB_PHONE_NUMBERS");

            migrationBuilder.DropIndex(
                name: "IX_TB_PHONE_NUMBERS_UserId",
                table: "TB_PHONE_NUMBERS");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TB_PHONE_NUMBERS");

            migrationBuilder.AddForeignKey(
                name: "FK_TB_PHONE_NUMBERS_TB_USERS_Id",
                table: "TB_PHONE_NUMBERS",
                column: "Id",
                principalTable: "TB_USERS",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
