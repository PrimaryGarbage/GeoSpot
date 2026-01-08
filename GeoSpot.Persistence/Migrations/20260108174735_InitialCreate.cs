using System;
using GeoSpot.Common.Enums;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GeoSpot.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "geospot");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:geospot.account_type", "user,business")
                .Annotation("Npgsql:Enum:geospot.gender", "not_specified,male,female,other")
                .Annotation("Npgsql:Enum:geospot.platform", "android,ios")
                .Annotation("Npgsql:Enum:geospot.spot_type", "promo,event,news,meetup")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "categories",
                schema: "geospot",
                columns: table => new
                {
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    icon_data = table.Column<byte[]>(type: "bytea", nullable: true),
                    color = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "reaction_types",
                schema: "geospot",
                columns: table => new
                {
                    reaction_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    emoji = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reaction_types", x => x.reaction_type_id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "geospot",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    password_hash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    password_salt = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    account_type = table.Column<AccountType>(type: "geospot.account_type", nullable: false),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    detection_radius = table.Column<int>(type: "integer", nullable: false, defaultValue: 500),
                    is_premium = table.Column<bool>(type: "boolean", nullable: false),
                    last_latitude = table.Column<double>(type: "double precision", precision: 10, scale: 8, nullable: false),
                    last_longitude = table.Column<double>(type: "double precision", precision: 11, scale: 8, nullable: false),
                    location_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    display_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    birth_year = table.Column<int>(type: "integer", nullable: false),
                    gender = table.Column<Gender>(type: "geospot.gender", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "verification_codes",
                schema: "geospot",
                columns: table => new
                {
                    verification_code_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    verification_code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_codes", x => x.verification_code_id);
                });

            migrationBuilder.CreateTable(
                name: "business_profiles",
                schema: "geospot",
                columns: table => new
                {
                    business_profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", maxLength: -1, nullable: true),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    latitude = table.Column<double>(type: "double precision", precision: 10, scale: 8, nullable: false),
                    longitude = table.Column<double>(type: "double precision", precision: 11, scale: 8, nullable: false),
                    radius = table.Column<int>(type: "integer", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    website_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    notification_balance = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_profiles", x => x.business_profile_id);
                    table.ForeignKey(
                        name: "fk_business_profiles_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "geospot",
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_business_profiles_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_tokens",
                schema: "geospot",
                columns: table => new
                {
                    device_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    platform = table.Column<Platform>(type: "geospot.platform", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_tokens", x => x.device_token_id);
                    table.ForeignKey(
                        name: "fk_device_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                schema: "geospot",
                columns: table => new
                {
                    refresh_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character varying(44)", maxLength: 44, nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revoked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.refresh_token_id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_categories",
                schema: "geospot",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_categories", x => new { x.user_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_user_categories_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "geospot",
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_categories_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spots",
                schema: "geospot",
                columns: table => new
                {
                    spot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    business_profile_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", maxLength: -1, nullable: true),
                    spot_type = table.Column<SpotType>(type: "geospot.spot_type", nullable: false),
                    image_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    latitude = table.Column<double>(type: "double precision", precision: 10, scale: 8, nullable: false),
                    longitude = table.Column<double>(type: "double precision", precision: 11, scale: 8, nullable: false),
                    position = table.Column<Point>(type: "geography(Point,4326)", nullable: false),
                    radius = table.Column<int>(type: "integer", nullable: false),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ends_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    views_count = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spots", x => x.spot_id);
                    table.ForeignKey(
                        name: "fk_spots_business_profiles_business_profile_id",
                        column: x => x.business_profile_id,
                        principalSchema: "geospot",
                        principalTable: "business_profiles",
                        principalColumn: "business_profile_id");
                    table.ForeignKey(
                        name: "fk_spots_users_creator_id",
                        column: x => x.creator_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spot_categories",
                schema: "geospot",
                columns: table => new
                {
                    spot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    categories_category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    spot_entity_spot_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spot_categories", x => new { x.spot_id, x.category_id });
                    table.ForeignKey(
                        name: "fk_spot_categories_categories_categories_category_id",
                        column: x => x.categories_category_id,
                        principalSchema: "geospot",
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spot_categories_categories_category_id",
                        column: x => x.category_id,
                        principalSchema: "geospot",
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spot_categories_spots_spot_entity_spot_id",
                        column: x => x.spot_entity_spot_id,
                        principalSchema: "geospot",
                        principalTable: "spots",
                        principalColumn: "spot_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spot_categories_spots_spot_id",
                        column: x => x.spot_id,
                        principalSchema: "geospot",
                        principalTable: "spots",
                        principalColumn: "spot_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spot_comments",
                schema: "geospot",
                columns: table => new
                {
                    spot_comment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    spot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", maxLength: -1, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spot_comments", x => x.spot_comment_id);
                    table.ForeignKey(
                        name: "fk_spot_comments_spots_spot_id",
                        column: x => x.spot_id,
                        principalSchema: "geospot",
                        principalTable: "spots",
                        principalColumn: "spot_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spot_comments_users_creator_id",
                        column: x => x.creator_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "spot_reactions",
                schema: "geospot",
                columns: table => new
                {
                    spot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reaction_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spot_reactions", x => new { x.spot_id, x.creator_id });
                    table.ForeignKey(
                        name: "fk_spot_reactions_reaction_types_reaction_type_id",
                        column: x => x.reaction_type_id,
                        principalSchema: "geospot",
                        principalTable: "reaction_types",
                        principalColumn: "reaction_type_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spot_reactions_spots_spot_id",
                        column: x => x.spot_id,
                        principalSchema: "geospot",
                        principalTable: "spots",
                        principalColumn: "spot_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_spot_reactions_users_creator_id",
                        column: x => x.creator_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_spot_views",
                schema: "geospot",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    spot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    viewed = table.Column<bool>(type: "boolean", nullable: false),
                    viewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_spot_views", x => new { x.user_id, x.spot_id });
                    table.ForeignKey(
                        name: "fk_user_spot_views_spots_spot_id",
                        column: x => x.spot_id,
                        principalSchema: "geospot",
                        principalTable: "spots",
                        principalColumn: "spot_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_spot_views_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "geospot",
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "geospot",
                table: "categories",
                columns: new[] { "category_id", "color", "icon_data", "name", "sort_order" },
                values: new object[,]
                {
                    { new Guid("0ac876bb-7930-4024-8810-a5b6f5941f94"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Шоппинг и скидки", 4 },
                    { new Guid("14dc1c16-9cb8-46fb-bd37-03ef4dfa57a6"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Вечеринки", 5 },
                    { new Guid("1ac211dc-97e0-409d-955a-6bb33c09b60f"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Кино", 3 },
                    { new Guid("2a58b9ee-7f22-4df8-b5de-e38f3c3b5a09"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Еда и рестораны", 0 },
                    { new Guid("6a5346dc-4606-4142-a217-d603dc687305"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Другое", 9 },
                    { new Guid("96eb53bd-bbe3-4b4f-a05a-8c26e29f0dfd"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Игры", 6 },
                    { new Guid("cad72e62-7293-4bf9-8a2d-178c020031e5"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Спорт", 1 },
                    { new Guid("d7005c8c-19c8-4ec6-b9d8-1e7e31fb869c"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Образование", 8 },
                    { new Guid("da1cf683-2dd8-4d88-bc9c-1f0a3cff6920"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Музыка и концерты", 2 },
                    { new Guid("e74b5890-12db-4d0b-8db2-dbeeb8dd7f6b"), "#FFFFFF", new byte[] { 0, 1, 2, 4, 5 }, "Искусство", 7 }
                });

            migrationBuilder.CreateIndex(
                name: "ix_business_profiles_category_id",
                schema: "geospot",
                table: "business_profiles",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_business_profiles_user_id",
                schema: "geospot",
                table: "business_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_tokens_user_id",
                schema: "geospot",
                table: "device_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_refresh_token_token_hash",
                schema: "geospot",
                table: "refresh_tokens",
                column: "token_hash");

            migrationBuilder.CreateIndex(
                name: "idx_refresh_token_user_id",
                schema: "geospot",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_categories_categories_category_id",
                schema: "geospot",
                table: "spot_categories",
                column: "categories_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_categories_category_id",
                schema: "geospot",
                table: "spot_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_categories_spot_entity_spot_id",
                schema: "geospot",
                table: "spot_categories",
                column: "spot_entity_spot_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_comments_creator_id",
                schema: "geospot",
                table: "spot_comments",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_comments_spot_id",
                schema: "geospot",
                table: "spot_comments",
                column: "spot_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_reactions_creator_id",
                schema: "geospot",
                table: "spot_reactions",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_spot_reactions_reaction_type_id",
                schema: "geospot",
                table: "spot_reactions",
                column: "reaction_type_id");

            migrationBuilder.CreateIndex(
                name: "idx_spot_position",
                schema: "geospot",
                table: "spots",
                column: "position")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_spots_business_profile_id",
                schema: "geospot",
                table: "spots",
                column: "business_profile_id");

            migrationBuilder.CreateIndex(
                name: "ix_spots_creator_id",
                schema: "geospot",
                table: "spots",
                column: "creator_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_categories_category_id",
                schema: "geospot",
                table: "user_categories",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_spot_views_spot_id",
                schema: "geospot",
                table: "user_spot_views",
                column: "spot_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_phone_number",
                schema: "geospot",
                table: "users",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_verification_codes_verification_code",
                schema: "geospot",
                table: "verification_codes",
                column: "verification_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_tokens",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "refresh_tokens",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "spot_categories",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "spot_comments",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "spot_reactions",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "user_categories",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "user_spot_views",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "verification_codes",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "reaction_types",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "spots",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "business_profiles",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "categories",
                schema: "geospot");

            migrationBuilder.DropTable(
                name: "users",
                schema: "geospot");
        }
    }
}
