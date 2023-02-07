﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cloudweather.Report.Migrations
{
    public partial class initialmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WeatherReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AverageHighF = table.Column<decimal>(type: "numeric", nullable: false),
                    AverageLowF = table.Column<decimal>(type: "numeric", nullable: false),
                    RainfallTotalInches = table.Column<decimal>(type: "numeric", nullable: false),
                    snowfallTotalInches = table.Column<decimal>(type: "numeric", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherReport", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherReport");
        }
    }
}
