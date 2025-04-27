using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TabloX2.Migrations
{
    /// <inheritdoc />
    public partial class AddHighResImageUrlToArtwork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HighResImageUrl",
                table: "Artworks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HighResImageUrl",
                table: "Artworks");
        }
    }
}
