
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using MusicPlaylistModel.Mapping;
using MusicPlaylistModel.Models;

namespace MusicPlaylistModel.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ArtistSong> ArtistsSongs { get; set; }
        public DbSet<PlaylistSong> PlaylistsSongs { get; set; }

        public DatabaseContext(DbContextOptions databaseContextOptions) : base(databaseContextOptions)
        {
            // OnModelCreating(builder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserBuildingConfiguration());
            builder.ApplyConfiguration(new PlaylistBuildingConfiguration());
            builder.ApplyConfiguration(new SongBuildingConfiguration());
            builder.ApplyConfiguration(new ArtistBuildingConfiguration());
            builder.ApplyConfiguration(new AlbumBuildingConfiguration());
            builder.ApplyConfiguration(new CategoryBuildingConfiguration());

            builder.ApplyConfiguration(new ArtistSongBuildingConfiguration());
            builder.ApplyConfiguration(new PlaylistSongBuildingConfiguration());

        }
    }
}
