
namespace DTO.Models.Playlist
{
    public class CreatePlaylistModel
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> SongsIds { get; set; }
    }
}
