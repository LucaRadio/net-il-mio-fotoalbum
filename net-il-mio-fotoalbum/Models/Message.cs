using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace net_il_mio_fotoalbum.Models
{
    [Table("messages")]
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string emailSender { get; set; }

        public string messageContent { get; set; }
        public DateTime? created_at { get; set; }

    }
}
