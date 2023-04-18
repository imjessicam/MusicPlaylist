namespace MusicPlaylistModel.Models
{
    public class Album
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Title { get; set; }

        // Song. ONE to MANY
        public virtual List<Song> Songs { get; set; }
                
    }
}
