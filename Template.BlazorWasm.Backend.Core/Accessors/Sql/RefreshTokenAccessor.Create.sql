CREATE TABLE IF NOT EXISTS RefreshToken (
    Id           INTEGER  NOT NULL,
    TokenHash    BLOB     NOT NULL,
    AccountName  TEXT     NOT NULL,
    ExpireAt     TEXT     NOT NULL,
    CreatedAt    TEXT     NOT NULL,
    PRIMARY KEY (Id AUTOINCREMENT),
    UNIQUE (TokenHash)
);
