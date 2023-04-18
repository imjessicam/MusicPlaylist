namespace MusicPlaylistModel.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        // User. ONE to ONE
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Song. MANY to MANY
        public virtual List<PlaylistSong> PlaylistsSongs { get; set; }
    }
}
