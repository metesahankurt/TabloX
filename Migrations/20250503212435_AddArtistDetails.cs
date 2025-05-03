using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabloX2.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArtMovement",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Artists",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeathDate",
                table: "Artists",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FunFact",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Influences",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Legacy",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NotableWorks",
                table: "Artists",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArtMovement",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "DeathDate",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "FunFact",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "Influences",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "Legacy",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "NotableWorks",
                table: "Artists");
        }
    }
}
