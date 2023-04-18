namespace MusicPlaylistModel.Models
{
    public class Category
    {
        public int Id { get; set; }
        public Guid ExternalId { get; set; }
        public string Name { get; set; }

        // Song. ONE to MANY
        public virtual List<Song> Songs { get; set; }
    }
}
