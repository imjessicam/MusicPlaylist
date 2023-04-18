namespace MusicPlaylistModel.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        // Song. MANY to MANY
        public virtual List<ArtistSong> ArtistsSongs { get; set; }
    }
}
