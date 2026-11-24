using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FIAP.CloudGames.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePaidToUserLibrary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePaid",
                table: "UserLibrary",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            // O valor pago das aquisições anteriores não foi registrado; o preço atual do jogo
            // é a melhor aproximação disponível e evita que esses itens apareçam como gratuitos.
            migrationBuilder.Sql("""
                UPDATE ul
                SET ul.PricePaid = g.Price
                FROM UserLibrary ul
                INNER JOIN Game g ON g.Id = ul.GameId;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePaid",
                table: "UserLibrary");
        }
    }
}
