using System.ComponentModel.DataAnnotations.Schema;

namespace b_robot_api.Models;
public class Credentials
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
