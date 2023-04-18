
namespace MusicPlaylistModel.Models
{
    public class ArtistSong
    {
        public int Id { get; set; }

        // Artist
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }

        // Song
        public int SongId { get; set; }
        public virtual Song Song { get; set; }

    }
}
