
namespace DTO.Models.Song
{
    public class UpdateSongModel
    {
        public string Title { get; set; }
        public Guid AlbumId { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid> ArtistsIds { get; set; }

        // UpdateMany
        public Guid ExternalId { get; set; }

    }
}
