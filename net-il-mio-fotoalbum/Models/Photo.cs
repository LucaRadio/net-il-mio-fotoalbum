using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_il_mio_fotoalbum.Models
{
    [Table("photos")]
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[]? Content { get; set; }
        public bool Visibility { get; set; }


        public List<Category>? Category { get; set; }
        [NotMapped]
        public IFormFile Upload { get; set; }

        [NotMapped]
        public string? ImageData { get => Content == null ? "" : Convert.ToBase64String(Content); }
        public Photo() { }

    }
}
