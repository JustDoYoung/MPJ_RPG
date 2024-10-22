using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountDB.Migrations
{
    /// <inheritdoc />
    public partial class Rank : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    AccountDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoginProviderUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProviderType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account", x => x.AccountDbId);
                });

            migrationBuilder.CreateTable(
                name: "Ranking",
                columns: table => new
                {
                    RankingDbId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranking", x => x.RankingDbId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_LoginProviderUserId",
                table: "Account",
                column: "LoginProviderUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ranking_Username",
                table: "Ranking",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "Ranking");
        }
    }
}
