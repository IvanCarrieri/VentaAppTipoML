using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaInventario.AccesoDatos.Migrations
{
    /// <inheritdoc />
    public partial class empresaMigracionn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Pais = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Ciudad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    DepositoVentaId = table.Column<int>(type: "int", nullable: false),
                    CreadoPorUsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ActualizadoPorUsuarioId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Empresas_AspNetUsers_ActualizadoPorUsuarioId",
                        column: x => x.ActualizadoPorUsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Empresas_AspNetUsers_CreadoPorUsuarioId",
                        column: x => x.CreadoPorUsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Empresas_Depositos_DepositoVentaId",
                        column: x => x.DepositoVentaId,
                        principalTable: "Depositos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_ActualizadoPorUsuarioId",
                table: "Empresas",
                column: "ActualizadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_CreadoPorUsuarioId",
                table: "Empresas",
                column: "CreadoPorUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_DepositoVentaId",
                table: "Empresas",
                column: "DepositoVentaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
