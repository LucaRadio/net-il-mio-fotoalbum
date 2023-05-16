using Microsoft.AspNetCore.Components.Forms;

namespace net_il_mio_fotoalbum.Models
{
    public class PhotoData
    {
        public Photo Photo { get; set; }
        public List<Category>? Categories { get; set; }
        public List<int>? SelectedCategory { get; set; }
    }
}
