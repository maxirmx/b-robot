using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Table("btasks")]
    public class BTask
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("api_key")]
        public string ApiKey { get; set; }

        [Column("strategy")]
        public string Strategy { get; set; }
    }
}


/*
 CREATE TABLE "btasks" (
  "id" SERIAL PRIMARY KEY,
  "api_key"  VARCHAR(64) NOT NULL,
  "strategy" VARCHAR(64) NOT NULL
);
*/
