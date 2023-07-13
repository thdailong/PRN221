using System.ComponentModel.DataAnnotations;

namespace Finance.Model
{
    public class Account
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, StringLength(60, MinimumLength = 3, ErrorMessage = "Name length must be between 3 and 60")]
        public string DisplayName { get; set; } = null!;

        public string AvatarUrl { get; set; } = "";

        public int active { get; set; } = 0;

        public DateTime CreatedAt { get; set; }

        public DateTime LastActive { get; set; }
    }
}
