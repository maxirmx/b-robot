using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography.X509Certificates;

namespace b_robot_api.Models;

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

    [Column("is_enabled")]
    public required bool IsEnabled { get; set; }

    [Column("is_admin")]
    public required bool IsAdmin { get; set; }
}

public class UserViewItem
{
    public UserViewItem(User user)
    {
        Id = user.Id;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Patronimic = user.Patronimic;
        Email = user.Email;
        ApiKey = user.ApiKey;
        IsEnabled = user.IsEnabled;
        IsAdmin = user.IsAdmin;
    }

    public int Id { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Patronimic { get; set; } = "";
    public string Email { get; set; } = "";
    public string ApiKey { get; set; }  = "";
    public bool IsEnabled { get; set; }
    public bool IsAdmin { get; set; }
}

public class UserViewItemWithJWT :  UserViewItem
{
    public UserViewItemWithJWT(User user) : base(user)
    {
        Token = "";
    }
    public string Token { get; set; } = "";
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
    public bool IsEnabled { get; set; }
    public bool IsAdmin { get; set; }
}
