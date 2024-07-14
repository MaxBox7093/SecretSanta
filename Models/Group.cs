using System.ComponentModel.DataAnnotations.Schema;

namespace SecretSanta.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [ForeignKey("GroupId")]
        public List<Participant>? Participants { get; set; }
    }
}
