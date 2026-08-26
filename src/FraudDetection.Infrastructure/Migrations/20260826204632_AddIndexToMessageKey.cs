using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudDetection.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexToMessageKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Message_MessageKey",
                table: "Message",
                column: "MessageKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Message_MessageKey",
                table: "Message");
        }
    }
}
