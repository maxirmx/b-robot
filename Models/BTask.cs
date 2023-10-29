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

        [Column("secret")]
        public string Secret { get; set; }

        [Column("strategy")]
        public string Strategy { get; set; }

        [Column("symbol1")]
        public string Symbol1 { get; set; }

        [Column("symbol2")]
        public string Symbol2 { get; set; }

        [Column("threshold")]
        public string Threshold { get; set; }
    }
}


/*

 docker exec -it db psql -U postgres

 CREATE TABLE "btasks" (
  "id" SERIAL PRIMARY KEY,
  "api_key"    VARCHAR(64) NOT NULL,
  "secret"     VARCHAR(64) NOT NULL,
  "strategy"   VARCHAR(64) NOT NULL,
  "symbol1"    VARCHAR(16) NOT NULL,
  "symbol2"    VARCHAR(16) NOT NULL,
  "threshold"  VARCHAR(32) NOT NULL
);
*/
