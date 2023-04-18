using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models.Playlist
{
    public class CreateManyPlaylistModel
    {
        public List<string> Name { get; set; }
        public Guid UserId { get; set; }
        public List<Guid> SongsIds { get; set; }
    }
}
