namespace DTO.Models.Album
{
    public class UpdateAlbumModel
    {
        public string Title { get; set; } = null!; 

        public Guid ExternalId { get; set; }
    }
}
