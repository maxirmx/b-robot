--
-- docker exec -it db psql -U postgres
--

START TRANSACTION;

DROP TABLE IF EXISTS "btasks";
DROP TABLE IF EXISTS "users";

CREATE TABLE "users" (
  "id"              SERIAL PRIMARY KEY,
  "first_name"      VARCHAR(16) NOT NULL,
  "last_name"       VARCHAR(16) NOT NULL,
  "patronimic"      VARCHAR(16) NOT NULL,
  "email"           VARCHAR(64) NOT NULL,
  "password"        VARCHAR(64) NOT NULL,
  "api_key"         VARCHAR(64) NOT NULL,
  "api_secret"      VARCHAR(64) NOT NULL,
  "is_enabled"      BOOLEAN NOT NULL DEFAULT TRUE,
  "is_admin"        BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE UNIQUE INDEX "idx_users_email" ON "users" ("email");

INSERT INTO "users" ("first_name", "patronimic", "last_name", "email", "password", "api_key", "api_secret", "is_enabled", "is_admin") VALUES
('Максим', 'Станиславович', 'Самсонов', 'maxirmx@sw.consulting', '$2a$11$PUWwhEUzqrusmtrDsH4wguSDVx1kmGcksoU1rOKjAcWkGKdGA55ZK', '', '', TRUE, TRUE);

CREATE TABLE "btasks" (
  "id"         SERIAL PRIMARY KEY,
  "user_id"    INTEGER NOT NULL REFERENCES "users" ("id") ON DELETE CASCADE,
  "strategy"   VARCHAR(64) NOT NULL,
  "symbol1"    VARCHAR(16) NOT NULL,
  "symbol2"    VARCHAR(16) NOT NULL,
  "threshold"  VARCHAR(32) NOT NULL,
  "is_running" BOOLEAN NOT NULL DEFAULT FALSE
);

CREATE INDEX "idx_btasks_user_id" ON "btasks" ("user_id");

DROP TABLE IF EXISTS "versions";

CREATE TABLE "versions" (
  "id"      SERIAL PRIMARY KEY,
  "version" VARCHAR(16) NOT NULL,
  "date"    DATE NOT NULL
);

INSERT INTO "versions" ("version", "date") VALUES
('0.1.0', '2023-12-02');

COMMIT;
