using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineShoppingCart.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 1; i < 150; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[]{
                        "Id",
                        "FirstName",
                        "LastName",
                        "Birthday",
                        "Avatar",
                        "Gender",
                        "CreateAt",
                        "UserName",
                        "Email",
                        "EmailConfirmed",
                        "SecurityStamp",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount"
                    },
                    values: new object[]{
                        Guid.NewGuid().ToString(),
                        "FirstName-"+i.ToString("D3"),
                        "LastName-"+i.ToString("D3"),
                        DateTime.Now,
                        "default-avatar-image.png",
                        true,
                        DateTime.Now,
                        $"email{i.ToString("D3")}.@email.com",
                        $"email{i.ToString("D3")}.@email.com",
                        true,
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        0
                    }
                );
            }
            migrationBuilder.InsertData(
                "Roles",
                columns: new[]{
                    "Id",
                    "Name",
                    "NormalizedName"
                },
                values: new[]{
                    Guid.NewGuid().ToString(),
                    "admin",
                    "ADMIN"
                }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users", true);
            migrationBuilder.Sql("DELETE FROM Roles", true);
        }
    }
}
