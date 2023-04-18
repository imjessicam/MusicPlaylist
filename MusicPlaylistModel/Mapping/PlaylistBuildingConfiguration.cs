using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Mapping
{
    public class PlaylistBuildingConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.Id);

            
        }
    }
}
