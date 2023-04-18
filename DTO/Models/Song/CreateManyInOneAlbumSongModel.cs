using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models.Song
{
    public class CreateManyInOneAlbumSongModel
    {
        public List<string> Titles { get; set; }
        public Guid AlbumId { get; set; }
        public Guid CategoryId { get; set; }
        public List<Guid> ArtistsIds { get; set; }
    }
}
