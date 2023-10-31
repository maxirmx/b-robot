using System.ComponentModel.DataAnnotations.Schema;

namespace b_robot_api.Models;

[Table("btasks")]
public class BTask
{
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("strategy")]
    public required string Strategy { get; set; }

    [Column("symbol1")]
    public required string Symbol1 { get; set; }

    [Column("symbol2")]
    public required string Symbol2 { get; set; }

    [Column("threshold")]
    public required string Threshold { get; set; }
}
