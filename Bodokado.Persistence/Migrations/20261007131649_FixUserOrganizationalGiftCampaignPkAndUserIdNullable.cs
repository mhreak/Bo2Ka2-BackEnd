using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bodokado.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixUserOrganizationalGiftCampaignPkAndUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_OrganizationalGiftCampaign_AspNetUsers_UserId",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_OrganizationalGiftCampaign",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "User_OrganizationalGiftCampaign",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "User_OrganizationalGiftCampaign",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_OrganizationalGiftCampaign",
                table: "User_OrganizationalGiftCampaign",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_OrganizationalGiftCampaign_UserId",
                table: "User_OrganizationalGiftCampaign",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_OrganizationalGiftCampaign_AspNetUsers_UserId",
                table: "User_OrganizationalGiftCampaign",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_OrganizationalGiftCampaign_AspNetUsers_UserId",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User_OrganizationalGiftCampaign",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropIndex(
                name: "IX_User_OrganizationalGiftCampaign_UserId",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "User_OrganizationalGiftCampaign",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "User_OrganizationalGiftCampaign",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User_OrganizationalGiftCampaign",
                table: "User_OrganizationalGiftCampaign",
                columns: new[] { "UserId", "OrganizationalGiftCampaignId" });

            migrationBuilder.AddForeignKey(
                name: "FK_User_OrganizationalGiftCampaign_AspNetUsers_UserId",
                table: "User_OrganizationalGiftCampaign",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
