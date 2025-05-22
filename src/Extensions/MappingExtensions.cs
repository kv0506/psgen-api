using PsGenApi.DataModel;
using PsGenApi.Document;
using System;

namespace PsGenApi.Extensions;

public static class MappingExtensions
{
    // User mapping extensions
    public static DbUser ToEntity(this User document)
    {
        if (document == null) return null!;

        return new DbUser
        {
            Id = string.IsNullOrEmpty(document.Id) ? Guid.NewGuid() : Guid.Parse(document.Id),
            Username = document.Username,
            Email = document.Email,
            Mobile = document.Mobile,
            Salt = document.Salt,
            Hash = document.Hash
        };
    }

    public static User ToDocument(this DbUser entity)
    {
        if (entity == null) return null!;

        return new User
        {
            Id = entity.Id.ToString(),
            Username = entity.Username ?? string.Empty,
            Email = entity.Email ?? string.Empty,
            Mobile = entity.Mobile ?? string.Empty,
            Salt = entity.Salt ?? string.Empty,
            Hash = entity.Hash ?? string.Empty
        };
    }

    // Account mapping extensions
    public static DbAccount ToEntity(this Account document)
    {
        if (document == null) return null!;

        return new DbAccount
        {
            Id = string.IsNullOrEmpty(document.Id) ? Guid.NewGuid() : Guid.Parse(document.Id),
            UserId = Guid.Parse(document.UserId),
            Category = document.Category,
            Name = document.Name,
            Username = document.Username,
            Pattern = document.Pattern,
            Length = document.Length,
            IncludeSpecialCharacter = document.IncludeSpecialCharacter,
            UseCustomSpecialCharacter = document.UseCustomSpecialCharacter,
            CustomSpecialCharacter = document.CustomSpecialCharacter,
            Notes = document.Notes,
            IsFavorite = document.IsFavorite
        };
    }

    public static Account ToDocument(this DbAccount entity)
    {
        if (entity == null) return null!;

        return new Account
        {
            Id = entity.Id.ToString(),
            UserId = entity.UserId.ToString(),
            Category = entity.Category ?? string.Empty,
            Name = entity.Name ?? string.Empty,
            Username = entity.Username ?? string.Empty,
            Pattern = entity.Pattern ?? string.Empty,
            Length = entity.Length,
            IncludeSpecialCharacter = entity.IncludeSpecialCharacter,
            UseCustomSpecialCharacter = entity.UseCustomSpecialCharacter,
            CustomSpecialCharacter = entity.CustomSpecialCharacter ?? string.Empty,
            Notes = entity.Notes ?? string.Empty,
            IsFavorite = entity.IsFavorite
        };
    }

    // Token mapping extensions
    public static DbToken ToEntity(this Token document)
    {
        if (document == null) return null!;

        return new DbToken
        {
            Id = string.IsNullOrEmpty(document.Id) ? Guid.NewGuid() : Guid.Parse(document.Id),
            UserId = Guid.Parse(document.UserId),
            Secret = document.Secret,
            ExpiresAt = document.ExpiresAt
        };
    }

    public static Token ToDocument(this DbToken entity)
    {
        if (entity == null) return null!;

        return new Token
        {
            Id = entity.Id.ToString(),
            UserId = entity.UserId.ToString(),
            Secret = entity.Secret ?? string.Empty,
            ExpiresAt = entity.ExpiresAt
        };
    }
}
