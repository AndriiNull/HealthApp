using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixIssueColumnMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IssueId",
                table: "Analyses",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Analyses",
                keyColumn: "Id",
                keyValue: 1,
                column: "IssueId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Analyses",
                keyColumn: "Id",
                keyValue: 2,
                column: "IssueId",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Analyses_IssueId",
                table: "Analyses",
                column: "IssueId");

            migrationBuilder.AddForeignKey(
                name: "FK_Analyses_Issues_IssueId",
                table: "Analyses",
                column: "IssueId",
                principalTable: "Issues",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Analyses_Issues_IssueId",
                table: "Analyses");

            migrationBuilder.DropIndex(
                name: "IX_Analyses_IssueId",
                table: "Analyses");

            migrationBuilder.DropColumn(
                name: "IssueId",
                table: "Analyses");
        }
    }
}
