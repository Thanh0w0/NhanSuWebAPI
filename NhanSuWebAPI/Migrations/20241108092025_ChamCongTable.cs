using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NhanSuWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChamCongTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChamCong",
                columns: table => new
                {
                    MaCC = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNV = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NgayChamCong = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianVao = table.Column<TimeSpan>(type: "time", nullable: true),
                    ThoiGianRa = table.Column<TimeSpan>(type: "time", nullable: true),
                    SoGioLam = table.Column<float>(type: "real", nullable: false),
                    SoGioTC = table.Column<float>(type: "real", nullable: false),
                    TrangThaiCC = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCong", x => x.MaCC);
                    table.ForeignKey(
                        name: "FK_ChamCong_NhanVien_MaNV",
                        column: x => x.MaNV,
                        principalTable: "NhanVien",
                        principalColumn: "MaNV",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChamCong_MaNV",
                table: "ChamCong",
                column: "MaNV");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChamCong");
        }
    }
}
