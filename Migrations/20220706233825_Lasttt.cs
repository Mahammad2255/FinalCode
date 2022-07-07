using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalCode.Migrations
{
    public partial class Lasttt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "OrderItems",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_SizeId",
                table: "OrderItems",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Sizes_SizeId",
                table: "OrderItems",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Sizes_SizeId",
                table: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_SizeId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "OrderItems");
        }
    }
}
