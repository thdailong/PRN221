using System.ComponentModel.DataAnnotations;

namespace Finance.Model
{
    public class Feedback
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Detail { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public Account Account { get; set; } = null!;
    }
}
