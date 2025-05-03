using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEventModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Planners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planners", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Weddings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    BrideName = table.Column<string>(type: "longtext", nullable: false),
                    GroomName = table.Column<string>(type: "longtext", nullable: false),
                    BrideVows = table.Column<string>(type: "longtext", nullable: false),
                    HowWeMet = table.Column<string>(type: "longtext", nullable: false),
                    GroomVows = table.Column<string>(type: "longtext", nullable: false),
                    Proposal = table.Column<string>(type: "longtext", nullable: false),
                    ThankYouMessage = table.Column<string>(type: "longtext", nullable: false),
                    PlannerId = table.Column<Guid>(type: "char(36)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weddings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Weddings_Planners_PlannerId",
                        column: x => x.PlannerId,
                        principalTable: "Planners",
                        principalColumn: "Id");
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "GuestMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    WeddingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: false),
                    SenderName = table.Column<string>(type: "longtext", nullable: false),
                    RelationToCouple = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuestMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GuestMessages_Weddings_WeddingId",
                        column: x => x.WeddingId,
                        principalTable: "Weddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Media",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    WeddingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Url = table.Column<string>(type: "longtext", nullable: false),
                    Type = table.Column<string>(type: "longtext", nullable: false),
                    IsCoverImage = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Media", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Media_Weddings_WeddingId",
                        column: x => x.WeddingId,
                        principalTable: "Weddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "QRCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    WeddingId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Url = table.Column<string>(type: "longtext", nullable: false),
                    AssetUrl = table.Column<string>(type: "longtext", nullable: false),
                    Scans = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QRCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QRCodes_Weddings_WeddingId",
                        column: x => x.WeddingId,
                        principalTable: "Weddings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_GuestMessages_WeddingId",
                table: "GuestMessages",
                column: "WeddingId");

            migrationBuilder.CreateIndex(
                name: "IX_Media_WeddingId",
                table: "Media",
                column: "WeddingId");

            migrationBuilder.CreateIndex(
                name: "IX_QRCodes_WeddingId",
                table: "QRCodes",
                column: "WeddingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Weddings_PlannerId",
                table: "Weddings",
                column: "PlannerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GuestMessages");

            migrationBuilder.DropTable(
                name: "Media");

            migrationBuilder.DropTable(
                name: "QRCodes");

            migrationBuilder.DropTable(
                name: "Weddings");

            migrationBuilder.DropTable(
                name: "Planners");
        }
    }
}
