using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Credentials
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
