using System.ComponentModel.DataAnnotations;

namespace Finance.Model
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Type { get; set; } = null!;
        public string IconColorClass { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public Icon Icon { get; set; } = null!;
        public Account Account { get; set; } = null!;


    }
}
