# Voice Backend API

## Technologies

- Language: C#
- Framework: .Net Core 5.0, Entity Framework
- Database: PostgreSQL 13.5
- Driver: Npgsql.EntityFrameworkCore.PostgreSQL
- Deployment: Heroku

## Pre-reqs

- Install [.NET 5.0](https://dotnet.microsoft.com/en-us/download/dotnet/5.0)
- Install [Postgresql 13.5](https://www.postgresqltutorial.com/install-postgresql/)

## Getting started

- Clone the repository

```
git clone https://github.com/LeDuyTuanVu/FIDOU_BE.git
```

- Go to ./VoiceAPI folder, run the project in development

```
dotnet run
```

Navigate to `http://localhost:5000` and enjoy!

## Useful Commands

| Command                                                      | Description                                   |
| ------------------------------------------------------------ | --------------------------------------------- |
| `dotnet run`                            | Run project in develop enviroment             |
| `dotnet run watch`                      | Run project in watch-mode                     |
| `dotnet ef migrations add <message>`    | Add new migration                             |
| `dotnet ef database update`             | Update database based on migration history    |
