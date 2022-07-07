using Microsoft.EntityFrameworkCore.Migrations;

namespace FinalCode.Migrations
{
    public partial class LastMIg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Baskets",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_SizeId",
                table: "Baskets",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Baskets_Sizes_SizeId",
                table: "Baskets",
                column: "SizeId",
                principalTable: "Sizes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Baskets_Sizes_SizeId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_SizeId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Baskets");
        }
    }
}
