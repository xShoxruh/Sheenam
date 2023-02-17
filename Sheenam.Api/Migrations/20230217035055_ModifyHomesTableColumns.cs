//==================================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free To Use To Find Comfort and Peace
//==================================================

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sheenam.Api.Migrations
{
    /// <inheritdoc />
    public partial class ModifyHomesTableColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumOfBedrooms",
                table: "Homes",
                newName: "NumberOfBedrooms");

            migrationBuilder.RenameColumn(
                name: "NumOfBathrooms",
                table: "Homes",
                newName: "NumberOfBathrooms");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfBedrooms",
                table: "Homes",
                newName: "NumOfBedrooms");

            migrationBuilder.RenameColumn(
                name: "NumberOfBathrooms",
                table: "Homes",
                newName: "NumOfBathrooms");
        }
    }
}
