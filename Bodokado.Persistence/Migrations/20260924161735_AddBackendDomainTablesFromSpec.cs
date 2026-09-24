using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bodokado.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBackendDomainTablesFromSpec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Product_ProductId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Product_Shop_ShopId",
                table: "Product");

            migrationBuilder.DropTable(
                name: "ProductColor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Product",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "ImageFileIds",
                table: "Product");

            migrationBuilder.RenameTable(
                name: "Product",
                newName: "ShopProduct");

            migrationBuilder.RenameIndex(
                name: "IX_Product_Status",
                table: "ShopProduct",
                newName: "IX_ShopProduct_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Product_SoldCount",
                table: "ShopProduct",
                newName: "IX_ShopProduct_SoldCount");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ShopId_Name",
                table: "ShopProduct",
                newName: "IX_ShopProduct_ShopId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Product_ShopId",
                table: "ShopProduct",
                newName: "IX_ShopProduct_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_Product_IsSpecial",
                table: "ShopProduct",
                newName: "IX_ShopProduct_IsSpecial");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShopCategory",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<Guid>(
                name: "ShopCategoryId",
                table: "Shop",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableStories",
                table: "Shop",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ManagerUserId",
                table: "Shop",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DiscountCodeId",
                table: "Order",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Gender",
                table: "AspNetUsers",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mobile",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShamsiBirthDate",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WalletCredit",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsActiveByAdmin",
                table: "ShopProduct",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MainImageFileId",
                table: "ShopProduct",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<short>(
                name: "ProductType",
                table: "ShopProduct",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShopProduct",
                table: "ShopProduct",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DeliveryServiceProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceProviderName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IncludedCityIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IncludedProvinceIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedCityIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedProvinceIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LogoFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryServiceProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryServiceProvider_Files_LogoFileId",
                        column: x => x.LogoFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderCustomizationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ShowOrder = table.Column<short>(type: "smallint", nullable: false),
                    ShopCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomizationType", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCustomizationType_ShopCategory_ShopCategoryId",
                        column: x => x.ShopCategoryId,
                        principalTable: "ShopCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LogoFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Organization_Files_LogoFileId",
                        column: x => x.LogoFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ShopProduct_ProductCategory",
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
                    table.PrimaryKey("PK_ShopProduct_ProductCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopProduct_ProductCategory_Files_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ShopProduct_ProductCategory_ShopProduct_ProductCategory_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "ShopProduct_ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShopProductImage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopProductImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopProductImage_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopProductImage_ShopProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ShopProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopProductVariation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopProductVariation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopProductVariation_ShopProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ShopProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopRatingOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ShowOrder = table.Column<short>(type: "smallint", nullable: false),
                    ShopCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopRatingOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopRatingOption_ShopCategory_ShopCategoryId",
                        column: x => x.ShopCategoryId,
                        principalTable: "ShopCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Story",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    PublishDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisabledByAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MediaFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoryButtonText = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    StoryButtonClickEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StoryButtonClickActionType = table.Column<short>(type: "smallint", nullable: false, defaultValue: (short)0),
                    ShowOrder = table.Column<short>(type: "smallint", nullable: false),
                    ShowPlace = table.Column<short>(type: "smallint", nullable: false),
                    IsActiveByAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Story", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Story_Files_MediaFileId",
                        column: x => x.MediaFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Story_Shop_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactionLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    TransactionDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactionLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactionLog_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ShopOrderDeliveryRule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryServiceProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IncludedCityIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IncludedProvinceIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedCityIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedProvinceIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopOrderDeliveryRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopOrderDeliveryRule_DeliveryServiceProvider_DeliveryServiceProviderId",
                        column: x => x.DeliveryServiceProviderId,
                        principalTable: "DeliveryServiceProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopOrderDeliveryRule_Shop_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscountCodeType = table.Column<short>(type: "smallint", nullable: false),
                    DiscountType = table.Column<short>(type: "smallint", nullable: false),
                    Discount = table.Column<int>(type: "int", nullable: true),
                    IncludedOrganizationIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedOrganizationIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IncludedShopIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedShopIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IncludedProductCategoryIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedProductCategoryIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IncludedProductIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ExcludedProductIds = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderCustomizationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    IsActiveByAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountCode_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DiscountCode_OrderCustomizationType_OrderCustomizationTypeId",
                        column: x => x.OrderCustomizationTypeId,
                        principalTable: "OrderCustomizationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OrderCustomizationTypeOption",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OptionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ImageFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThumbnailImageFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShowOrder = table.Column<short>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OrderCustomizationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderCustomizationTypeOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderCustomizationTypeOption_Files_ImageFileId",
                        column: x => x.ImageFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomizationTypeOption_Files_ThumbnailImageFileId",
                        column: x => x.ThumbnailImageFileId,
                        principalTable: "Files",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderCustomizationTypeOption_OrderCustomizationType_OrderCustomizationTypeId",
                        column: x => x.OrderCustomizationTypeId,
                        principalTable: "OrderCustomizationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shop_OrderCustomizationType",
                columns: table => new
                {
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderCustomizationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop_OrderCustomizationType", x => new { x.ShopId, x.OrderCustomizationTypeId });
                    table.ForeignKey(
                        name: "FK_Shop_OrderCustomizationType_OrderCustomizationType_OrderCustomizationTypeId",
                        column: x => x.OrderCustomizationTypeId,
                        principalTable: "OrderCustomizationType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shop_OrderCustomizationType_Shop_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalGiftCampaign",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CampaignName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FinishDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OccasionType = table.Column<short>(type: "smallint", nullable: false),
                    OrganizationalMessageEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    MessageType = table.Column<short>(type: "smallint", nullable: true),
                    OrganizationalMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OrganizationalMessageFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActiveByAdmin = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalGiftCampaign", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalGiftCampaign_Files_OrganizationalMessageFileId",
                        column: x => x.OrganizationalMessageFileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_OrganizationalGiftCampaign_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationPersonnelCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationPersonnelCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationPersonnelCategory_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ProductCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UseForProductVariants = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttribute_ShopProduct_ProductCategory_ProductCategoryId",
                        column: x => x.ProductCategoryId,
                        principalTable: "ShopProduct_ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "EntityRating",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RatingOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    InsertDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityRating", x => new { x.EntityId, x.RatingOptionId });
                    table.ForeignKey(
                        name: "FK_EntityRating_ShopRatingOption_RatingOptionId",
                        column: x => x.RatingOptionId,
                        principalTable: "ShopRatingOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Shop_OrderCustomizationTypeOption",
                columns: table => new
                {
                    ShopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderCustomizationTypeOptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shop_OrderCustomizationTypeOption", x => new { x.ShopId, x.OrderCustomizationTypeOptionId });
                    table.ForeignKey(
                        name: "FK_Shop_OrderCustomizationTypeOption_OrderCustomizationTypeOption_OrderCustomizationTypeOptionId",
                        column: x => x.OrderCustomizationTypeOptionId,
                        principalTable: "OrderCustomizationTypeOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Shop_OrderCustomizationTypeOption_Shop_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shop",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User_OrganizationalGiftCampaign",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationalGiftCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GiftCode = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_OrganizationalGiftCampaign", x => new { x.UserId, x.OrganizationalGiftCampaignId });
                    table.ForeignKey(
                        name: "FK_User_OrganizationalGiftCampaign_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_OrganizationalGiftCampaign_OrganizationalGiftCampaign_OrganizationalGiftCampaignId",
                        column: x => x.OrganizationalGiftCampaignId,
                        principalTable: "OrganizationalGiftCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalGiftCampaignConstraint",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationalGiftCampaignId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationPersonnelCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConstraintType = table.Column<short>(type: "smallint", nullable: false),
                    Constraint = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalGiftCampaignConstraint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalGiftCampaignConstraint_OrganizationPersonnelCategory_OrganizationPersonnelCategoryId",
                        column: x => x.OrganizationPersonnelCategoryId,
                        principalTable: "OrganizationPersonnelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrganizationalGiftCampaignConstraint_OrganizationalGiftCampaign_OrganizationalGiftCampaignId",
                        column: x => x.OrganizationalGiftCampaignId,
                        principalTable: "OrganizationalGiftCampaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User_Organization",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    OrganizationPersonnelCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Organization", x => new { x.UserId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_User_Organization_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Organization_OrganizationPersonnelCategory_OrganizationPersonnelCategoryId",
                        column: x => x.OrganizationPersonnelCategoryId,
                        principalTable: "OrganizationPersonnelCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_User_Organization_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product_ProductAttribute",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_ProductAttribute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductAttribute_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Product_ProductAttribute_ShopProduct_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ShopProduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductAttributeValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductAttributeValue_ProductAttribute_ProductAttributeId",
                        column: x => x.ProductAttributeId,
                        principalTable: "ProductAttribute",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopProductVariation_ProductAttributeValue",
                columns: table => new
                {
                    ShopProductVariationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductAttributeValueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopProductVariation_ProductAttributeValue", x => new { x.ShopProductVariationId, x.ProductAttributeValueId });
                    table.ForeignKey(
                        name: "FK_ShopProductVariation_ProductAttributeValue_ProductAttributeValue_ProductAttributeValueId",
                        column: x => x.ProductAttributeValueId,
                        principalTable: "ProductAttributeValue",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShopProductVariation_ProductAttributeValue_ShopProductVariation_ShopProductVariationId",
                        column: x => x.ShopProductVariationId,
                        principalTable: "ShopProductVariation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shop_ManagerUserId",
                table: "Shop",
                column: "ManagerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DiscountCodeId",
                table: "Order",
                column: "DiscountCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Mobile",
                table: "AspNetUsers",
                column: "Mobile");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_IsActiveByAdmin",
                table: "ShopProduct",
                column: "IsActiveByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_MainImageFileId",
                table: "ShopProduct",
                column: "MainImageFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryServiceProvider_IsActive",
                table: "DeliveryServiceProvider",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryServiceProvider_LogoFileId",
                table: "DeliveryServiceProvider",
                column: "LogoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_Code",
                table: "DiscountCode",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_FinishDateTime",
                table: "DiscountCode",
                column: "FinishDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_IsActive",
                table: "DiscountCode",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_IsActiveByAdmin",
                table: "DiscountCode",
                column: "IsActiveByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_OrderCustomizationTypeId",
                table: "DiscountCode",
                column: "OrderCustomizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_StartDateTime",
                table: "DiscountCode",
                column: "StartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountCode_UserId",
                table: "DiscountCode",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRating_EntityId",
                table: "EntityRating",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRating_InsertDateTime",
                table: "EntityRating",
                column: "InsertDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_EntityRating_RatingOptionId",
                table: "EntityRating",
                column: "RatingOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationType_IsActive",
                table: "OrderCustomizationType",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationType_ShopCategoryId",
                table: "OrderCustomizationType",
                column: "ShopCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationType_ShowOrder",
                table: "OrderCustomizationType",
                column: "ShowOrder");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationTypeOption_ImageFileId",
                table: "OrderCustomizationTypeOption",
                column: "ImageFileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationTypeOption_IsActive",
                table: "OrderCustomizationTypeOption",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationTypeOption_OrderCustomizationTypeId",
                table: "OrderCustomizationTypeOption",
                column: "OrderCustomizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationTypeOption_ShowOrder",
                table: "OrderCustomizationTypeOption",
                column: "ShowOrder");

            migrationBuilder.CreateIndex(
                name: "IX_OrderCustomizationTypeOption_ThumbnailImageFileId",
                table: "OrderCustomizationTypeOption",
                column: "ThumbnailImageFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_IsActive",
                table: "Organization",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_LogoFileId",
                table: "Organization",
                column: "LogoFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Organization_OrganizationName",
                table: "Organization",
                column: "OrganizationName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaign_FinishDateTime",
                table: "OrganizationalGiftCampaign",
                column: "FinishDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaign_IsActiveByAdmin",
                table: "OrganizationalGiftCampaign",
                column: "IsActiveByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaign_OccasionType",
                table: "OrganizationalGiftCampaign",
                column: "OccasionType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaign_OrganizationalMessageFileId",
                table: "OrganizationalGiftCampaign",
                column: "OrganizationalMessageFileId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaign_OrganizationId",
                table: "OrganizationalGiftCampaign",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaign_StartDateTime",
                table: "OrganizationalGiftCampaign",
                column: "StartDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaignConstraint_ConstraintType",
                table: "OrganizationalGiftCampaignConstraint",
                column: "ConstraintType");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaignConstraint_OrganizationalGiftCampaignId",
                table: "OrganizationalGiftCampaignConstraint",
                column: "OrganizationalGiftCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalGiftCampaignConstraint_OrganizationPersonnelCategoryId",
                table: "OrganizationalGiftCampaignConstraint",
                column: "OrganizationPersonnelCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPersonnelCategory_IsActive",
                table: "OrganizationPersonnelCategory",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationPersonnelCategory_OrganizationId",
                table: "OrganizationPersonnelCategory",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductAttribute_ProductAttributeId",
                table: "Product_ProductAttribute",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductAttribute_ProductId",
                table: "Product_ProductAttribute",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductAttribute_ProductId_ProductAttributeId",
                table: "Product_ProductAttribute",
                columns: new[] { "ProductId", "ProductAttributeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_ProductCategoryId",
                table: "ProductAttribute",
                column: "ProductCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttribute_SortOrder",
                table: "ProductAttribute",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_IsActive",
                table: "ProductAttributeValue",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_ProductAttributeId",
                table: "ProductAttributeValue",
                column: "ProductAttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValue_SortOrder",
                table: "ProductAttributeValue",
                column: "SortOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_OrderCustomizationType_IsActive",
                table: "Shop_OrderCustomizationType",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_OrderCustomizationType_OrderCustomizationTypeId",
                table: "Shop_OrderCustomizationType",
                column: "OrderCustomizationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Shop_OrderCustomizationTypeOption_OrderCustomizationTypeOptionId",
                table: "Shop_OrderCustomizationTypeOption",
                column: "OrderCustomizationTypeOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderDeliveryRule_DeliveryServiceProviderId",
                table: "ShopOrderDeliveryRule",
                column: "DeliveryServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderDeliveryRule_IsActive",
                table: "ShopOrderDeliveryRule",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrderDeliveryRule_ShopId",
                table: "ShopOrderDeliveryRule",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ProductCategory_ImageId",
                table: "ShopProduct_ProductCategory",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ProductCategory_IsActive",
                table: "ShopProduct_ProductCategory",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ProductCategory_Name",
                table: "ShopProduct_ProductCategory",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProduct_ProductCategory_ParentCategoryId",
                table: "ShopProduct_ProductCategory",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProductImage_FileId",
                table: "ShopProductImage",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProductImage_ProductId",
                table: "ShopProductImage",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProductVariation_ProductId",
                table: "ShopProductVariation",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopProductVariation_ProductAttributeValue_ProductAttributeValueId",
                table: "ShopProductVariation_ProductAttributeValue",
                column: "ProductAttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopRatingOption_ShopCategoryId",
                table: "ShopRatingOption",
                column: "ShopCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Story_DisabledByAdmin",
                table: "Story",
                column: "DisabledByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Story_IsActiveByAdmin",
                table: "Story",
                column: "IsActiveByAdmin");

            migrationBuilder.CreateIndex(
                name: "IX_Story_IsPublished",
                table: "Story",
                column: "IsPublished");

            migrationBuilder.CreateIndex(
                name: "IX_Story_MediaFileId",
                table: "Story",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Story_PublishDateTime",
                table: "Story",
                column: "PublishDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_Story_ShopId",
                table: "Story",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_Story_ShowOrder",
                table: "Story",
                column: "ShowOrder");

            migrationBuilder.CreateIndex(
                name: "IX_Story_ShowPlace",
                table: "Story",
                column: "ShowPlace");

            migrationBuilder.CreateIndex(
                name: "IX_User_Organization_IsActive",
                table: "User_Organization",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_User_Organization_OrganizationId",
                table: "User_Organization",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Organization_OrganizationPersonnelCategoryId",
                table: "User_Organization",
                column: "OrganizationPersonnelCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_User_OrganizationalGiftCampaign_GiftCode",
                table: "User_OrganizationalGiftCampaign",
                column: "GiftCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_OrganizationalGiftCampaign_OrganizationalGiftCampaignId",
                table: "User_OrganizationalGiftCampaign",
                column: "OrganizationalGiftCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactionLog_TransactionDateTime",
                table: "WalletTransactionLog",
                column: "TransactionDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactionLog_UserId",
                table: "WalletTransactionLog",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DiscountCode_DiscountCodeId",
                table: "Order",
                column: "DiscountCodeId",
                principalTable: "DiscountCode",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_ShopProduct_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "ShopProduct",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Shop_AspNetUsers_ManagerUserId",
                table: "Shop",
                column: "ManagerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopProduct_Files_MainImageFileId",
                table: "ShopProduct",
                column: "MainImageFileId",
                principalTable: "Files",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ShopProduct_Shop_ShopId",
                table: "ShopProduct",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_DiscountCode_DiscountCodeId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_ShopProduct_ProductId",
                table: "OrderItem");

            migrationBuilder.DropForeignKey(
                name: "FK_Shop_AspNetUsers_ManagerUserId",
                table: "Shop");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopProduct_Files_MainImageFileId",
                table: "ShopProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopProduct_Shop_ShopId",
                table: "ShopProduct");

            migrationBuilder.DropTable(
                name: "DiscountCode");

            migrationBuilder.DropTable(
                name: "EntityRating");

            migrationBuilder.DropTable(
                name: "OrganizationalGiftCampaignConstraint");

            migrationBuilder.DropTable(
                name: "Product_ProductAttribute");

            migrationBuilder.DropTable(
                name: "Shop_OrderCustomizationType");

            migrationBuilder.DropTable(
                name: "Shop_OrderCustomizationTypeOption");

            migrationBuilder.DropTable(
                name: "ShopOrderDeliveryRule");

            migrationBuilder.DropTable(
                name: "ShopProductImage");

            migrationBuilder.DropTable(
                name: "ShopProductVariation_ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "Story");

            migrationBuilder.DropTable(
                name: "User_Organization");

            migrationBuilder.DropTable(
                name: "User_OrganizationalGiftCampaign");

            migrationBuilder.DropTable(
                name: "WalletTransactionLog");

            migrationBuilder.DropTable(
                name: "ShopRatingOption");

            migrationBuilder.DropTable(
                name: "OrderCustomizationTypeOption");

            migrationBuilder.DropTable(
                name: "DeliveryServiceProvider");

            migrationBuilder.DropTable(
                name: "ProductAttributeValue");

            migrationBuilder.DropTable(
                name: "ShopProductVariation");

            migrationBuilder.DropTable(
                name: "OrganizationPersonnelCategory");

            migrationBuilder.DropTable(
                name: "OrganizationalGiftCampaign");

            migrationBuilder.DropTable(
                name: "OrderCustomizationType");

            migrationBuilder.DropTable(
                name: "ProductAttribute");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "ShopProduct_ProductCategory");

            migrationBuilder.DropIndex(
                name: "IX_Shop_ManagerUserId",
                table: "Shop");

            migrationBuilder.DropIndex(
                name: "IX_Order_DiscountCodeId",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_IsDeleted",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Mobile",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShopProduct",
                table: "ShopProduct");

            migrationBuilder.DropIndex(
                name: "IX_ShopProduct_IsActiveByAdmin",
                table: "ShopProduct");

            migrationBuilder.DropIndex(
                name: "IX_ShopProduct_MainImageFileId",
                table: "ShopProduct");

            migrationBuilder.DropColumn(
                name: "EnableStories",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "ManagerUserId",
                table: "Shop");

            migrationBuilder.DropColumn(
                name: "DiscountCodeId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShamsiBirthDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "WalletCredit",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActiveByAdmin",
                table: "ShopProduct");

            migrationBuilder.DropColumn(
                name: "MainImageFileId",
                table: "ShopProduct");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "ShopProduct");

            migrationBuilder.RenameTable(
                name: "ShopProduct",
                newName: "Product");

            migrationBuilder.RenameIndex(
                name: "IX_ShopProduct_Status",
                table: "Product",
                newName: "IX_Product_Status");

            migrationBuilder.RenameIndex(
                name: "IX_ShopProduct_SoldCount",
                table: "Product",
                newName: "IX_Product_SoldCount");

            migrationBuilder.RenameIndex(
                name: "IX_ShopProduct_ShopId_Name",
                table: "Product",
                newName: "IX_Product_ShopId_Name");

            migrationBuilder.RenameIndex(
                name: "IX_ShopProduct_ShopId",
                table: "Product",
                newName: "IX_Product_ShopId");

            migrationBuilder.RenameIndex(
                name: "IX_ShopProduct_IsSpecial",
                table: "Product",
                newName: "IX_Product_IsSpecial");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "ShopCategory",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "ShopCategoryId",
                table: "Shop",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageFileIds",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Product",
                table: "Product",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ProductColor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HexCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColor_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductColor_ProductId",
                table: "ProductColor",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Product_ProductId",
                table: "OrderItem",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Shop_ShopId",
                table: "Product",
                column: "ShopId",
                principalTable: "Shop",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
