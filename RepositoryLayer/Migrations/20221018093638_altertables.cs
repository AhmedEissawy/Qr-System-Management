using Microsoft.EntityFrameworkCore.Migrations;

namespace RepositoryLayer.Migrations
{
    public partial class altertables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "ApplicationUser");

            migrationBuilder.DropColumn(
                name: "UnitName",
                table: "ApplicationUser");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Units",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Owners",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "VisitorSSN",
                table: "Invitaions",
                newName: "VisitorIdentifier");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Invitaions",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Invitaions",
                newName: "EndDate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Units",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "Owners",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "VisitorIdentifier",
                table: "Invitaions",
                newName: "VisitorSSN");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Invitaions",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Invitaions",
                newName: "End");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitName",
                table: "ApplicationUser",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
