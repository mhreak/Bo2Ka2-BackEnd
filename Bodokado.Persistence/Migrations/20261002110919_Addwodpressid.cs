using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bodokado.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addwodpressid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "ShopProduct",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ShopId_ExternalId",
                table: "ShopProduct",
                columns: new[] { "ShopId", "ExternalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShopProduct_ShopId_ExternalId",
                table: "ShopProduct");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "ShopProduct");
        }
    }
}
