
namespace DTO.Models.Song
{
    public class CreateSongModel
    {
        public string Title { get; set; }
        public Guid AlbumId { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid> ArtistsIds { get; set; }
    }
}
