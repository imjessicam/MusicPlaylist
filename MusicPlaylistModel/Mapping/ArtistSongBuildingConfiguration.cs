using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Mapping
{
    public class ArtistSongBuildingConfiguration : IEntityTypeConfiguration<ArtistSong>
    {
        public void Configure(EntityTypeBuilder<ArtistSong> builder)
        {
            // Artist
            builder
                .HasOne(x => x.Artist)
                .WithMany(x => x.ArtistsSongs)
                .HasForeignKey(x => x.ArtistId);

            // Song
            builder
                .HasOne(x => x.Song)
                .WithMany(x => x.ArtistsSongs)
                .HasForeignKey(x => x.SongId);
        }
    }
}
