using System.ComponentModel.DataAnnotations;

namespace Finance.Model
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;

        public double SumAmount { get; set; }

        public int TransCount { get; set; }

        [Required]
        public double Amount { get; set; }

        public DateTime CreatedAt { get; set; }
        public Category Category { get; set; } = new();
    }
}
