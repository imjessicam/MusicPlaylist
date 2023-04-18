using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Mapping
{
    public class PlaylistSongBuildingConfiguration : IEntityTypeConfiguration<PlaylistSong>
    {
        public void Configure(EntityTypeBuilder<PlaylistSong> builder)
        {
            // Playlist
            builder
                .HasOne(x => x.Playlist)
                .WithMany(x => x.PlaylistsSongs)
                .HasForeignKey(x => x.PlaylistId);


            // Song
            builder
                .HasOne(x => x.Song)
                .WithMany(x => x.PlaylistsSongs)
                .HasForeignKey(x => x.SongId);


        }
    }
}
