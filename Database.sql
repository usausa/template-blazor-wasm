-- SQLite
CREATE TABLE IF NOT EXISTS Data (
    Id         INTEGER  NOT NULL,
    Name       TEXT     NOT NULL,
    Value      INTEGER  NOT NULL,
    CreatedAt  TEXT     NOT NULL,
    PRIMARY KEY (Id AUTOINCREMENT),
    UNIQUE (Name)
);

CREATE TABLE IF NOT EXISTS Account (
    Id         INTEGER  NOT NULL,
    Name       TEXT     NOT NULL,
    Password   BLOB     NOT NULL,
    Role       TEXT     NOT NULL,
    CreatedAt  TEXT     NOT NULL,
    PRIMARY KEY (Id AUTOINCREMENT),
    UNIQUE (Name)
);

CREATE TABLE IF NOT EXISTS RefreshToken (
    Id           INTEGER  NOT NULL,
    TokenHash    BLOB     NOT NULL,
    AccountName  TEXT     NOT NULL,
    ExpireAt     TEXT     NOT NULL,
    CreatedAt    TEXT     NOT NULL,
    PRIMARY KEY (Id AUTOINCREMENT),
    UNIQUE (TokenHash)
);
