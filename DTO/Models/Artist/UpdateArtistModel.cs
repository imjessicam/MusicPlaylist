using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.Models.Artist
{
    public class UpdateArtistModel
    {
        public string Name { get; set; }

        public Guid ExternalId { get; set; }
    }
}
