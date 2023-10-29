using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("users")]
    public class User
    {

        [Column("id")]
        public int Id { get; set; }

        [Column("first_name")]
        public required string FirstName { get; set; }

        [Column("last_name")]
        public required string LastName { get; set; }

        [Column("patronimic")]
        public string Patronimic { get; set; } = "";

        [Column("email")]
        public required string Email { get; set; }

        [Column("password")]
        public required string Password { get; set; }

        [Column("api_key")]
        public required string ApiKey { get; set; }

        [Column("api_secret")]
        public required string ApiSecret { get; set; }
    }

    public class UserViewItem
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Patronimic { get; set; }
        public required string Email { get; set; }
        public required string ApiKey { get; set; }
    }

    public class UserUpdateItem
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string Patronimic { get; set; } = "";
        public required string Email { get; set; }
        public string? Password { get; set; }
        public required string ApiKey { get; set; }
        public string? ApiSecret { get; set; }
    }

}
