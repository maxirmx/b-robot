--
-- docker exec -it db psql -U postgres
--

START TRANSACTION;

INSERT INTO "versions" ("version", "date") VALUES
('0.1.2', '2023-11-28');

COMMIT;
