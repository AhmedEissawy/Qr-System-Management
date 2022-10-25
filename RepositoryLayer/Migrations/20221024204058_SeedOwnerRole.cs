using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace RepositoryLayer.Migrations
{
    public partial class SeedOwnerRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
               table: "Roles",
               columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
               values: new object[] { Guid.NewGuid().ToString(), "Owner", "Owner".ToUpper(), Guid.NewGuid().ToString() }
               );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
