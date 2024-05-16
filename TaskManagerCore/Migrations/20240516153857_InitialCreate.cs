using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskManagerCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    TaskFolderEntityId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.TaskFolderEntityId);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskDataEntityId = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    RepeatingInterval = table.Column<long>(type: "INTEGER", nullable: false),
                    Repetitions = table.Column<long>(type: "INTEGER", nullable: false),
                    Streak = table.Column<long>(type: "INTEGER", nullable: false),
                    TaskFolderEntityId = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskDataEntityId);
                    table.ForeignKey(
                        name: "FK_Tasks_Folders_TaskFolderEntityId",
                        column: x => x.TaskFolderEntityId,
                        principalTable: "Folders",
                        principalColumn: "TaskFolderEntityId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskFolderEntityId",
                table: "Tasks",
                column: "TaskFolderEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}
