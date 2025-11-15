using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zmm.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AbpBackgroundJobs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ApplicationName = table.Column<string>(type: "TEXT", maxLength: 96, nullable: true),
                    JobName = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    JobArgs = table.Column<string>(type: "TEXT", maxLength: 1048576, nullable: false),
                    TryCount = table.Column<short>(type: "INTEGER", nullable: false, defaultValue: (short)0),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextTryTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastTryTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsAbandoned = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Priority = table.Column<byte>(type: "INTEGER", nullable: false, defaultValue: (byte)15),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpBackgroundJobs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpEventInbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    MessageId = table.Column<string>(type: "TEXT", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Processed = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProcessedTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEventInbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbpEventOutbox",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    EventName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    EventData = table.Column<byte[]>(type: "BLOB", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpEventOutbox", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZipmodInfo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Identifier = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Version = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Author = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Game = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    IsCharaMod = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsStudioMod = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMapMod = table.Column<bool>(type: "INTEGER", nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipmodInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZipmodFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Path = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    InfoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipmodFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZipmodFile_ZipmodInfo_InfoId",
                        column: x => x.InfoId,
                        principalTable: "ZipmodInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ZipmodLink",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DownloadUri = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    Size = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    UploadTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsInvalid = table.Column<bool>(type: "INTEGER", nullable: false),
                    InfoId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZipmodLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZipmodLink_ZipmodInfo_InfoId",
                        column: x => x.InfoId,
                        principalTable: "ZipmodInfo",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpBackgroundJobs_IsAbandoned_NextTryTime",
                table: "AbpBackgroundJobs",
                columns: new[] { "IsAbandoned", "NextTryTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEventInbox_MessageId",
                table: "AbpEventInbox",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_AbpEventInbox_Processed_CreationTime",
                table: "AbpEventInbox",
                columns: new[] { "Processed", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpEventOutbox_CreationTime",
                table: "AbpEventOutbox",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_ZipmodFile_InfoId",
                table: "ZipmodFile",
                column: "InfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ZipmodLink_InfoId",
                table: "ZipmodLink",
                column: "InfoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpBackgroundJobs");

            migrationBuilder.DropTable(
                name: "AbpEventInbox");

            migrationBuilder.DropTable(
                name: "AbpEventOutbox");

            migrationBuilder.DropTable(
                name: "ZipmodFile");

            migrationBuilder.DropTable(
                name: "ZipmodLink");

            migrationBuilder.DropTable(
                name: "ZipmodInfo");
        }
    }
}
