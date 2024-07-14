using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSanta.Models
{
    public class Participant
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Wish { get; set; }
        public int? RecipientId { get; set; }
        [ForeignKey("RecipientId")]
        public Participant? Recipient { get; set; }
        public int GroupId { get; set; }
    }
}