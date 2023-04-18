using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Mapping
{
    public class SongBuildingConfiguration : IEntityTypeConfiguration<Song>
    {
        public void Configure(EntityTypeBuilder<Song> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.Id);
        }
    }
}
