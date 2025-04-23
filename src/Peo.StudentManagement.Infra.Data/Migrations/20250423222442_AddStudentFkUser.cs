using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peo.StudentManagement.Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentFkUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          

            migrationBuilder.AddForeignKey(
                name: "FK_Student_User_UserId",
                table: "Student",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_User_UserId",
                table: "Student");

            
        }
    }
}
