namespace DTO.Models.Playlist
{
    public class UpdatePlaylistModel
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> SongsIds { get; set; }
        public Guid ExternalId { get; set; }
    }
}
