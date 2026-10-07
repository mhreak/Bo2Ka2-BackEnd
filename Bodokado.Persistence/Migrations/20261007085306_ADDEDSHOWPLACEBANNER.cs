using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bodokado.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADDEDSHOWPLACEBANNER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "ShowPlace",
                table: "Banner",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_Banner_ShowPlace",
                table: "Banner",
                column: "ShowPlace");

            migrationBuilder.CreateIndex(
                name: "IX_Banner_ShowPlace_IsActive_ShowOrder",
                table: "Banner",
                columns: new[] { "ShowPlace", "IsActive", "ShowOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Banner_ShowPlace",
                table: "Banner");

            migrationBuilder.DropIndex(
                name: "IX_Banner_ShowPlace_IsActive_ShowOrder",
                table: "Banner");

            migrationBuilder.DropColumn(
                name: "ShowPlace",
                table: "Banner");
        }
    }
}
