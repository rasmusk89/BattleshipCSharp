using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boats",
                columns: table => new
                {
                    ShipId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boats", x => x.ShipId);
                });

            migrationBuilder.CreateTable(
                name: "GameOptions",
                columns: table => new
                {
                    GameOptionId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    BoardWidth = table.Column<int>(type: "INTEGER", nullable: false),
                    BoardHeight = table.Column<int>(type: "INTEGER", nullable: false),
                    EShipsCanTouch = table.Column<int>(type: "INTEGER", nullable: false),
                    ENextMoveAfterHit = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOptions", x => x.GameOptionId);
                });

            migrationBuilder.CreateTable(
                name: "GameOptionBoats",
                columns: table => new
                {
                    GameOptionShipId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Amount = table.Column<int>(type: "INTEGER", nullable: false),
                    ShipId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameOptionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameOptionBoats", x => x.GameOptionShipId);
                    table.ForeignKey(
                        name: "FK_GameOptionBoats_Boats_ShipId",
                        column: x => x.ShipId,
                        principalTable: "Boats",
                        principalColumn: "ShipId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameOptionBoats_GameOptions_GameOptionId",
                        column: x => x.GameOptionId,
                        principalTable: "GameOptions",
                        principalColumn: "GameOptionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameOptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerAId = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerBId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_GameOptions_GameOptionId",
                        column: x => x.GameOptionId,
                        principalTable: "GameOptions",
                        principalColumn: "GameOptionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    EPlayerType = table.Column<int>(type: "INTEGER", nullable: false),
                    GameAId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameBId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerId);
                    table.ForeignKey(
                        name: "FK_Players_Games_GameAId",
                        column: x => x.GameAId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Players_Games_GameBId",
                        column: x => x.GameBId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameBoats",
                columns: table => new
                {
                    GameShipId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Size = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    IsSunk = table.Column<bool>(type: "INTEGER", nullable: false),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameBoats", x => x.GameShipId);
                    table.ForeignKey(
                        name: "FK_GameBoats_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerBoardState",
                columns: table => new
                {
                    PlayerBoardStateId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PlayerId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    BoardState = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerBoardState", x => x.PlayerBoardStateId);
                    table.ForeignKey(
                        name: "FK_PlayerBoardState_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameBoats_PlayerId",
                table: "GameBoats",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOptionBoats_GameOptionId",
                table: "GameOptionBoats",
                column: "GameOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameOptionBoats_ShipId",
                table: "GameOptionBoats",
                column: "ShipId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameOptionId",
                table: "Games",
                column: "GameOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PlayerAId",
                table: "Games",
                column: "PlayerAId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_PlayerBId",
                table: "Games",
                column: "PlayerBId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlayerBoardState_PlayerId",
                table: "PlayerBoardState",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameAId",
                table: "Players",
                column: "GameAId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_GameBId",
                table: "Players",
                column: "GameBId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_PlayerAId",
                table: "Games",
                column: "PlayerAId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Games_Players_PlayerBId",
                table: "Games",
                column: "PlayerBId",
                principalTable: "Players",
                principalColumn: "PlayerId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_PlayerAId",
                table: "Games");

            migrationBuilder.DropForeignKey(
                name: "FK_Games_Players_PlayerBId",
                table: "Games");

            migrationBuilder.DropTable(
                name: "GameBoats");

            migrationBuilder.DropTable(
                name: "GameOptionBoats");

            migrationBuilder.DropTable(
                name: "PlayerBoardState");

            migrationBuilder.DropTable(
                name: "Boats");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GameOptions");
        }
    }
}
