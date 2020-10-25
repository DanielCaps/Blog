using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Blog.Migrations
{
    public partial class CommentSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainComments",
                columns: table => new
                {
                    Comment_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Post_Id = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainComments", x => x.Comment_ID);
                    table.ForeignKey(
                        name: "FK_MainComments_Posts_Post_Id",
                        column: x => x.Post_Id,
                        principalTable: "Posts",
                        principalColumn: "Post_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubComments",
                columns: table => new
                {
                    Comment_ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    MainCommentID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubComments", x => x.Comment_ID);
                    table.ForeignKey(
                        name: "FK_SubComments_MainComments_MainCommentID",
                        column: x => x.MainCommentID,
                        principalTable: "MainComments",
                        principalColumn: "Comment_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainComments_Post_Id",
                table: "MainComments",
                column: "Post_Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubComments_MainCommentID",
                table: "SubComments",
                column: "MainCommentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubComments");

            migrationBuilder.DropTable(
                name: "MainComments");
        }
    }
}
