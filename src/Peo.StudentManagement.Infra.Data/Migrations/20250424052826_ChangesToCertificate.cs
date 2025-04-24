using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.StudentManagement.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangesToCertificate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificate_EnrollmentId",
                table: "Certificate");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_EnrollmentId",
                table: "Certificate",
                column: "EnrollmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificate_EnrollmentId",
                table: "Certificate");

            migrationBuilder.CreateIndex(
                name: "IX_Certificate_EnrollmentId",
                table: "Certificate",
                column: "EnrollmentId",
                unique: true);
        }
    }
}
