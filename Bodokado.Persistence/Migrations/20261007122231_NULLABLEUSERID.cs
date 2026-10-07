using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bodokado.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NULLABLEUSERID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "User_OrganizationalGiftCampaign",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "User_OrganizationalGiftCampaign",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "User_OrganizationalGiftCampaign",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "User_OrganizationalGiftCampaign",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedAt",
                table: "User_OrganizationalGiftCampaign",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropColumn(
                name: "UsedAt",
                table: "User_OrganizationalGiftCampaign");
        }
    }
}
