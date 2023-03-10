using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YomLog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Author",
                columns: table => new
                {
                    PK = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedByName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Author", x => x.PK);
                });

            migrationBuilder.CreateTable(
                name: "Book",
                columns: table => new
                {
                    PK = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GoogleBooksId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    GoogleBooksUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Isbn = table.Column<string>(type: "TEXT", nullable: true),
                    BookType = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalPage = table.Column<int>(type: "INTEGER", nullable: true),
                    TotalKindleLocation = table.Column<int>(type: "INTEGER", nullable: true),
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedById = table.Column<Guid>(type: "TEXT", nullable: false),
                    CreatedByName = table.Column<string>(type: "TEXT", nullable: false),
                    UpdatedById = table.Column<Guid>(type: "TEXT", nullable: true),
                    UpdatedByName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Book", x => x.PK);
                });

            migrationBuilder.CreateTable(
                name: "BookAuthorDataModel",
                columns: table => new
                {
                    PK = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FKBook = table.Column<long>(type: "INTEGER", nullable: false),
                    FKAuthor = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthorDataModel", x => x.PK);
                    table.ForeignKey(
                        name: "FK_BookAuthorDataModel_Author_FKAuthor",
                        column: x => x.FKAuthor,
                        principalTable: "Author",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthorDataModel_Book_FKBook",
                        column: x => x.FKBook,
                        principalTable: "Book",
                        principalColumn: "PK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Author_Id",
                table: "Author",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Book_Id",
                table: "Book",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthorDataModel_FKAuthor",
                table: "BookAuthorDataModel",
                column: "FKAuthor");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthorDataModel_FKBook",
                table: "BookAuthorDataModel",
                column: "FKBook");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthorDataModel");

            migrationBuilder.DropTable(
                name: "Author");

            migrationBuilder.DropTable(
                name: "Book");
        }
    }
}
