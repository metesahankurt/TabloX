using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabloX2.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryToArtist_Fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collection",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "Material",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "OriginalUrl",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "Technique",
                table: "Artworks");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Artworks");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Artists",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Artists");

            migrationBuilder.AddColumn<string>(
                name: "Collection",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Material",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OriginalUrl",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Technique",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
