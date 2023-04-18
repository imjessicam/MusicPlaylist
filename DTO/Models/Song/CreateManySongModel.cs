using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models.Song
{
    public class CreateManySongModel
    {
        public List<string> Titles { get; set; }
        public List<Guid> AlbumsIds { get; set; }
        public List<Guid> CategorysIds { get; set; }
        public List<Guid> ArtistsIds { get; set; }       

    }
}
