﻿CREATE SCHEMA dbo;

CREATE TABLE dbo.shortenedurls (
	"Id" serial PRIMARY KEY,
    "LongUrl" text,
    "ShortUrl" text,
	"HitCount" numeric,
	"Code" VARCHAR ( 10 ) UNIQUE NOT NULL,
	"CreatedDate" TIMESTAMP NOT NULL
);
