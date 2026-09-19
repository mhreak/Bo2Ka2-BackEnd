using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bodokado.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductCatalogTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductCategory_Files_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductCategory_ProductCategory_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductProperty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductProperty_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Product_ProductProperty",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_ProductProperty", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductProperty_ProductProperty_ProductPropertyId",
                        column: x => x.ProductPropertyId,
                        principalTable: "ProductProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Product_ProductProperty_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPropertyValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductPropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPropertyValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPropertyValue_ProductProperty_ProductPropertyId",
                        column: x => x.ProductPropertyId,
                        principalTable: "ProductProperty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductProperty_ProductId",
                table: "Product_ProductProperty",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductProperty_ProductId_ProductPropertyId",
                table: "Product_ProductProperty",
                columns: new[] { "ProductId", "ProductPropertyId" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductProperty_ProductPropertyId",
                table: "Product_ProductProperty",
                column: "ProductPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_ImageId",
                table: "ProductCategory",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_IsActive",
                table: "ProductCategory",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_Name",
                table: "ProductCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ProductCategory_ParentCategoryId",
                table: "ProductCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperty_ProductCategoryId",
                table: "ProductProperty",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductProperty_SortOrder",
                table: "ProductProperty",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPropertyValue_IsActive",
                table: "ProductPropertyValue",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPropertyValue_ProductPropertyId",
                table: "ProductPropertyValue",
                column: "ProductPropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPropertyValue_SortOrder",
                table: "ProductPropertyValue",
                column: "SortOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product_ProductProperty");

            migrationBuilder.DropTable(
                name: "ProductPropertyValue");

            migrationBuilder.DropTable(
                name: "ProductProperty");

            migrationBuilder.DropTable(
                name: "ProductCategory");
        }
    }
}
