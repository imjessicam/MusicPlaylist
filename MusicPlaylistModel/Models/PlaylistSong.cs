namespace MusicPlaylistModel.Models
{
    public class PlaylistSong
    {
        public int Id { get; set; }

        // Playlist
        public int PlaylistId { get; set; }
        public virtual Playlist Playlist { get; set; }

        // Song
        public int SongId { get; set; }
        public virtual Song Song { get; set; }  

        
    }
}
