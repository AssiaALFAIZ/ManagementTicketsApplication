using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ManagementTicketsApplication.Migrations
{
    public partial class ModifyTicketSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Ajouter une nouvelle colonne temporaire "StatusTemp" de type integer
            migrationBuilder.AddColumn<int>(
                name: "StatusTemp",
                table: "Tickets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            // 2. Mettre à jour la colonne "StatusTemp" avec les valeurs basées sur l'énumération
            migrationBuilder.Sql(
                @"
                UPDATE ""Tickets""
                SET ""StatusTemp"" = CASE 
                    WHEN ""Status"" = 'Open' THEN 0
                    WHEN ""Status"" = 'Closed' THEN 1
                END;
                ");

            // 3. Supprimer l'ancienne colonne "Status" de type string
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tickets");

            // 4. Renommer la colonne temporaire "StatusTemp" en "Status"
            migrationBuilder.RenameColumn(
                name: "StatusTemp",
                table: "Tickets",
                newName: "Status");

            // 5. Assurez-vous que l'ID est mis à jour correctement aussi
            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Tickets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Inverser les modifications si nécessaire

            // Ajouter la colonne d'origine "Status" (texte)
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            // Remettre à jour la colonne Status avec les valeurs originales
            migrationBuilder.Sql(
                @"
                UPDATE ""Tickets""
                SET ""Status"" = CASE 
                    WHEN ""Status"" = 0 THEN 'Open'
                    WHEN ""Status"" = 1 THEN 'Closed'
                END;
                ");

            // Supprimer la colonne temporaire
            migrationBuilder.DropColumn(
                name: "StatusTemp",
                table: "Tickets");
        }
    }
}
