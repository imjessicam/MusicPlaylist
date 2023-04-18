namespace MusicPlaylistModel.Models
{
    public class User
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        // Playlist. ONE to MANY
        public virtual List<Playlist> Playlists { get; set; }
    }
}
