namespace MusicPlaylistModel.Models
{
    public class Song
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Title { get; set; }

        // Artist. MANY to MANY
        public virtual List<ArtistSong> ArtistsSongs { get; set; }

        // Album. ONE to ONE
        public int AlbumId { get; set; }
        public virtual Album Album { get; set; }

        // Category. ONE to ONE
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        // Playlist. MANY to MANY
        public virtual List<PlaylistSong> PlaylistsSongs { get; set; }
    }
}
