using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GDemoExpress.DataBase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "playerinfos",
                schema: "dbo",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    last_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    full_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    nick_name = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    mailbox = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("playerinfos_pk", x => x.player_id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                schema: "dbo",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    password = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValueSql: "1"),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.player_id);
                });

            migrationBuilder.CreateIndex(
                name: "players_account_idx",
                schema: "dbo",
                table: "players",
                column: "account",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "playerinfos",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "players",
                schema: "dbo");
        }
    }
}
