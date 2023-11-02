--
-- docker exec -it db psql -U postgres
--

START TRANSACTION;

INSERT INTO "versions" ("version", "date") VALUES
('0.1.1', '2023-12-02');

COMMIT;
