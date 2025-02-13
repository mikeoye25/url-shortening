# A simple URL shortening service

## Two main features provided:
### 1. Shorten a URL: POST /shorten – Accepts a long URL and returns a shortened URL.
### 2. Retrieve a URL: GET /{shortUrl} – Accepts a short URL and redirects to the original long URL.

## Bonus feature added:
### 1. Add a GET /stats/{shortUrl} endpoint to show how many times a URL has been accessed

## Performance considerations:
### 1. Caching included

## Built with:
### Csharp
### .NET 8
### PostgreSQL - persistent data store

## Getting Started
### How to set up your project on Docker - [Medium article](https://medium.com/@michaeloyelami/build-a-web-api-project-using-net-8-in-docker-af057f2c30f4)
### Create database schema and table - [URL shortening DB](https://github.com/mikeoye25/url-shortening/blob/main/url-shortening-db.sql)
