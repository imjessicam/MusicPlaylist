using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicPlaylistModel.Migrations
{
    public partial class AddDbSetArtistsSongsAndPlaylistsSongs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSong_Artists_ArtistId",
                table: "ArtistSong");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistSong_Songs_SongId",
                table: "ArtistSong");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSong_Playlists_PlaylistId",
                table: "PlaylistSong");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistSong_Songs_SongId",
                table: "PlaylistSong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistSong",
                table: "PlaylistSong");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtistSong",
                table: "ArtistSong");

            migrationBuilder.RenameTable(
                name: "PlaylistSong",
                newName: "PlaylistsSongs");

            migrationBuilder.RenameTable(
                name: "ArtistSong",
                newName: "ArtistsSongs");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistSong_SongId",
                table: "PlaylistsSongs",
                newName: "IX_PlaylistsSongs_SongId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistSong_PlaylistId",
                table: "PlaylistsSongs",
                newName: "IX_PlaylistsSongs_PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistSong_SongId",
                table: "ArtistsSongs",
                newName: "IX_ArtistsSongs_SongId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistSong_ArtistId",
                table: "ArtistsSongs",
                newName: "IX_ArtistsSongs_ArtistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistsSongs",
                table: "PlaylistsSongs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtistsSongs",
                table: "ArtistsSongs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistsSongs_Artists_ArtistId",
                table: "ArtistsSongs",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistsSongs_Songs_SongId",
                table: "ArtistsSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistsSongs_Playlists_PlaylistId",
                table: "PlaylistsSongs",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistsSongs_Songs_SongId",
                table: "PlaylistsSongs",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArtistsSongs_Artists_ArtistId",
                table: "ArtistsSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_ArtistsSongs_Songs_SongId",
                table: "ArtistsSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistsSongs_Playlists_PlaylistId",
                table: "PlaylistsSongs");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistsSongs_Songs_SongId",
                table: "PlaylistsSongs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaylistsSongs",
                table: "PlaylistsSongs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArtistsSongs",
                table: "ArtistsSongs");

            migrationBuilder.RenameTable(
                name: "PlaylistsSongs",
                newName: "PlaylistSong");

            migrationBuilder.RenameTable(
                name: "ArtistsSongs",
                newName: "ArtistSong");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistsSongs_SongId",
                table: "PlaylistSong",
                newName: "IX_PlaylistSong_SongId");

            migrationBuilder.RenameIndex(
                name: "IX_PlaylistsSongs_PlaylistId",
                table: "PlaylistSong",
                newName: "IX_PlaylistSong_PlaylistId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistsSongs_SongId",
                table: "ArtistSong",
                newName: "IX_ArtistSong_SongId");

            migrationBuilder.RenameIndex(
                name: "IX_ArtistsSongs_ArtistId",
                table: "ArtistSong",
                newName: "IX_ArtistSong_ArtistId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaylistSong",
                table: "PlaylistSong",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArtistSong",
                table: "ArtistSong",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSong_Artists_ArtistId",
                table: "ArtistSong",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArtistSong_Songs_SongId",
                table: "ArtistSong",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSong_Playlists_PlaylistId",
                table: "PlaylistSong",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistSong_Songs_SongId",
                table: "PlaylistSong",
                column: "SongId",
                principalTable: "Songs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
