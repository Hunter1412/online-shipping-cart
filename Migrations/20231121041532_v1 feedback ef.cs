using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingCart.Migrations
{
    /// <inheritdoc />
    public partial class v1feedbackef : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Feedbacks_FeedbackId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_FeedbackId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "FeedbackId",
                table: "Images");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Feedbacks");

            migrationBuilder.AddColumn<Guid>(
                name: "FeedbackId",
                table: "Images",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_FeedbackId",
                table: "Images",
                column: "FeedbackId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Feedbacks_FeedbackId",
                table: "Images",
                column: "FeedbackId",
                principalTable: "Feedbacks",
                principalColumn: "Id");
        }
    }
}
