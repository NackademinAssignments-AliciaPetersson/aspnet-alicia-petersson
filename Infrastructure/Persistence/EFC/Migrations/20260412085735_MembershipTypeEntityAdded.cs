using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.EFC.Migrations
{
    /// <inheritdoc />
    public partial class MembershipTypeEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MembershipTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipTypes", x => x.Id);
                    table.CheckConstraint("CK_MembershipTypes_BasePriceNotNegative", "[BasePrice] >= 0");
                    table.CheckConstraint("CK_MembershipTypes_NameNotEmpty", "LTRIM(RTRIM([Name])) <> ''");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembershipTypes_IsActive",
                table: "MembershipTypes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "UQ_MembershipTypes_Name",
                table: "MembershipTypes",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembershipTypes");
        }
    }
}
