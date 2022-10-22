using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class alterDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Owners",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Owners",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConfirmPassword",
                table: "Owners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Owners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Owners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Owners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Owners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Owners_ApplicationUserId",
                table: "Owners",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Owners_ApplicationUser_ApplicationUserId",
                table: "Owners",
                column: "ApplicationUserId",
                principalTable: "ApplicationUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Owners_ApplicationUser_ApplicationUserId",
                table: "Owners");

            migrationBuilder.DropIndex(
                name: "IX_Owners_ApplicationUserId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "ConfirmPassword",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Owners");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "ApplicationUser");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Owners",
                newName: "FullName");
        }
    }
}
